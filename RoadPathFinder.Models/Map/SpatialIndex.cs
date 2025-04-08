using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using MinimalLock;

using SmallGeometry.Euclidean;
using SmallGeometry.Primitives;

using RoadPathFinder.Models.Elements;
using RoadPathFinder.Models.Utils;

namespace RoadPathFinder.Models.Map
{
    internal class SpatialIndex
    {
        public Guid MapID { get; }

        public double TileSideLength
        {
            get => _tileSideLength;
            set => _tileSideLength = Math.Max(double.Epsilon, value);
        }
        private double _tileSideLength;

        public bool IsInitDone { get; private set; } = false;
        public bool IsInitInProgress => _initMutex.IsLocked();
        public bool IsInitFail { get; private set; } = false;

        private IReadOnlyDictionary<string, GraphLink> _graph { get; }
        private Dictionary<string, HashSet<string>> _tile { get; set; } = new();
        private MutexSingle _initMutex { get; } = new();


        public SpatialIndex(
            Guid mapID,
            IReadOnlyDictionary<string, GraphLink> graph,
            double tileSideLength = 100)
        {
            MapID = mapID;

            ArgumentNullException.ThrowIfNull(graph, nameof(graph));
            _graph = graph;

            TileSideLength = tileSideLength;

            _initMutex.PollingIntervalMs = 10;
            _initMutex.DefaultWaitTimeoutMs = 30000;
        }


        /// <summary>
        /// Mutex-managed init
        /// </summary>
        /// <param name="refresh"></param>
        /// <param name="maxThreads"></param>
        /// <param name="logger"></param>
        /// <returns>true when success</returns>
        public async Task<bool> Init(bool refresh, int maxThreads, ILogger? logger)
        {
            var initID = new Guid();
            object[] initLoggerInfo = [MapID, "Spatial Index Init", initID];

            logger?.LogInformation("Starting", initLoggerInfo);

            if (IsInitDone
                && !refresh)
            {
                logger?.LogInformation("Init already done.", initLoggerInfo);
            }
            else if (await _initMutex.TryAcquireAfterWait())
            {
                IsInitDone = false;
                
                DateTime startTime = DateTime.UtcNow;
                BuildIndex(maxThreads, logger);
                DateTime endTime = DateTime.UtcNow;

                IsInitDone = true;

                logger?.LogInformation($"Elapsed {(endTime - startTime).TotalMilliseconds} ms.", initLoggerInfo);

                if (!_initMutex.TryRelease())
                {
                    logger?.LogError("Init returned without mutex released.", initLoggerInfo);
                }
            }
            else
            {
                logger?.LogInformation("Init already in progress.", initLoggerInfo);
            }

            return IsInitDone && !IsInitFail;
        }

        /// <summary>
        /// From the center, get the links within maxDistance
        /// </summary>
        /// <param name="center"></param>
        /// <param name="maxDistance"></param>
        /// <returns>key: distance from center</returns>
        public SortedDictionary<double, List<GraphLink>> SearchLinksWithinDistance(FlatPoint center, double maxDistance)
        {
            int tileRange = (int)(maxDistance / TileSideLength) + 1;
            var tileSearchResult = SearchLinksNear(center, tileRange);

            var result = new SortedDictionary<double, List<GraphLink>>();
            foreach (var kv in tileSearchResult)
            {
                if (kv.Key <= maxDistance)
                {
                    result.TryAdd(kv.Key, kv.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// From the center, get the links within surrounding (2*range+1)^2 tiles.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="tileRange"></param>
        /// <returns>key: distance from center</returns>
        public SortedDictionary<double, List<GraphLink>> SearchLinksNear(FlatPoint center, int tileRange)
        {
            IEnumerable<string> tileKeys = GetSurroundingTileIndexKeys(center, tileRange);

            var resultLinkIDs = new HashSet<string>();
            foreach (string tileKey in tileKeys)
            {
                if (_tile.TryGetValue(tileKey, out HashSet<string>? tileLinkIDs))
                {
                    foreach (var linkID in tileLinkIDs)
                    {
                        resultLinkIDs.Add(linkID);
                    }
                }
            }

            var result = new SortedDictionary<double, List<GraphLink>>();
            foreach (var linkID in resultLinkIDs)
            {
                if (_graph.TryGetValue(linkID, out GraphLink? link))
                {
                    FlatPoint nearestPoint = link.Geometry.GetNearestPoints(center).First().Value;
                    double distance = center.GetDistance(nearestPoint);

                    result.TryAdd(distance, new List<GraphLink>(2));

                    if (result.TryGetValue(distance, out List<GraphLink>? linksOfDistance))
                    {
                        linksOfDistance.Add(link);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// fills <see cref="_tile"/>
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private void BuildIndex(int maxThreads, ILogger? logger)
        {
            var tempTile = new ConcurrentDictionary<string, ConcurrentBag<string>>();
            object[] buildLoggerInfo = [MapID, "Building index"];

            // build tempTile
            if (maxThreads > 1)
            {
                Parallel.ForEach(
                    _graph.Values,
                    new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
                    AddToTempTile);
            }
            else
            {
                foreach (var link in _graph.Values)
                {
                    AddToTempTile(link);
                }
            }

            _tile.Clear();

            // move to _tile
            foreach (var kv in tempTile)
            {
                var linkIDs = kv.Value.ToHashSet();
                if (!_tile.TryAdd(kv.Key, linkIDs))
                {
                    logger?.LogError($"Tile {kv.Key} confirm failed.", buildLoggerInfo);
                    IsInitFail = true;
                }
            }

            ///////////////////////////////////////////////////////////////////
            void AddToTempTile(GraphLink link)
            {
                if (link == null)
                {
                    logger?.LogWarning("null link is given to tempTile", buildLoggerInfo);
                    return;
                }

                FlatLine l = link.Geometry.Interpolate(TileSideLength);

                foreach (FlatPoint fp in l)
                {
                    string key = GetIndexKey(fp);

                    tempTile.TryAdd(key, new ConcurrentBag<string>());

                    if (tempTile.TryGetValue(
                        key,
                        out ConcurrentBag<string>? linkIDs))
                    {
                        linkIDs.Add(link.ID);
                    }
                    else
                    {
                        logger?.LogError($"Tile {key} init failed.", buildLoggerInfo);
                        IsInitFail = true;
                    }
                }
            }
        }

        /// <summary>
        /// From the tile containing center, get tile index keys of surrounding (2*range+1)^2 tiles;
        /// </summary>
        /// <param name="center"></param>
        /// <param name="tileRange"></param>
        /// <returns></returns>
        private IEnumerable<string> GetSurroundingTileIndexKeys(FlatPoint center, int tileRange)
        {
            tileRange = Math.Max(0, tileRange);

            var eastWestVectors = new List<Vector2D>(2 * tileRange + 1);
            var northSouthVectors = new List<Vector2D>(2 * tileRange + 1);

            for (int i = 0; i <= tileRange; ++i)
            {
                var east = new Vector2D(tileRange * TileSideLength, 0);
                eastWestVectors.Add(east);
                eastWestVectors.Add(-east);

                var north = new Vector2D(0, tileRange * TileSideLength);
                northSouthVectors.Add(north);
                northSouthVectors.Add(-north);
            }

            var resultSet = new HashSet<string>(eastWestVectors.Count * northSouthVectors.Count + 1);

            foreach (Vector2D northSouthVec in northSouthVectors)
            {
                foreach (Vector2D eastWestVec in eastWestVectors)
                {
                    FlatPoint keyPoint = center + northSouthVec + eastWestVec;
                    resultSet.Add(GetIndexKey(keyPoint));
                }
            }

            return resultSet;
        }

        private string GetIndexKey(FlatPoint fp)
        {
            return GetIndexKey(fp.X, fp.Y);
        }

        private string GetIndexKey(double x, double y)
        {
            long roundedX = (long)(x / TileSideLength);
            long roundedY = (long)(y / TileSideLength);
            return $"{roundedX},{roundedY}";
        }
    }
}

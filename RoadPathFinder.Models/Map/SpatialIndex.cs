using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinimalLock;

using SmallGeometry.Euclidean;
using SmallGeometry.Primitives;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Map
{
    internal class SpatialIndex
    {
        public double TileSideLength
        {
            get => _tileSideLength;
            set => _tileSideLength = Math.Max(double.Epsilon, value);
        }
        private double _tileSideLength;

        public bool IsInitDone { get; private set; } = false;
        public bool IsInitInProgress => _initMutex.IsLocked();
        public bool IsInitFail { get; private set; } = false;

        private static string ReportTitle => "SpatialIndex";
        private IReadOnlyDictionary<long, GraphLink> _graph { get; }
        private Dictionary<string, HashSet<long>> _tile { get; set; } = new();
        private MutexSingle _initMutex { get; } = new();


        public SpatialIndex(
            IReadOnlyDictionary<long, GraphLink> graph,
            double tileSideLength = 100)
        {
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
        /// <returns></returns>
        public async Task<Report> Init(bool refresh, int maxThreads)
        {
            var initReport = new Report(ReportTitle);

            if (IsInitDone
                && !refresh)
            {
                initReport.Add(Report.ReportMessageType.Info, "Init already done.");
            }
            else if (await _initMutex.TryAcquireAfterWait())
            {
                IsInitDone = false;
                
                DateTime startTime = DateTime.UtcNow;
                var buildReport = BuildIndex(maxThreads);
                DateTime endTime = DateTime.UtcNow;

                IsInitDone = true;

                initReport.Merge(buildReport);
                initReport.Add(Report.ReportMessageType.Info, $"elapsed time {(endTime - startTime).TotalSeconds} seconds.");

                if (_initMutex.TryRelease())
                {
                    initReport.Add(Report.ReportMessageType.Info, "OK");
                }
                else
                {
                    initReport.Add(Report.ReportMessageType.Error,
                        "Init exit without mutex released.");
                }
            }
            else
            {
                initReport.Add(Report.ReportMessageType.Info, "Init already in progress.");
                initReport.Add(Report.ReportMessageType.Warning, "Init waiting timeout.");
            }

            return initReport;
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

            var resultLinkIDs = new HashSet<long>();
            foreach (string tileKey in tileKeys)
            {
                if (_tile.TryGetValue(tileKey, out HashSet<long>? tileLinkIDs))
                {
                    foreach (long linkID in tileLinkIDs)
                    {
                        resultLinkIDs.Add(linkID);
                    }
                }
            }

            var result = new SortedDictionary<double, List<GraphLink>>();
            foreach (long linkID in resultLinkIDs)
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
        /// <returns></returns>
        private Report BuildIndex(int maxThreads)
        {
            var tempTile = new ConcurrentDictionary<string, ConcurrentBag<long>>();
            var report = new Report(ReportTitle);

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
                    report.Add(Report.ReportMessageType.Error,
                        $"Tile {kv.Key} confirm failed.");
                    IsInitFail = true;
                }
            }

            return report;

            ///////////////////////////////////////////////////////////////////
            void AddToTempTile(GraphLink link)
            {
                FlatLine l = link.Geometry.Interpolate(TileSideLength);

                foreach (FlatPoint fp in l)
                {
                    string key = GetIndexKey(fp);

                    tempTile.TryAdd(key, new ConcurrentBag<long>());

                    if (tempTile.TryGetValue(
                        key,
                        out ConcurrentBag<long>? linkIDs))
                    {
                        linkIDs.Add(link.ID);
                    }
                    else
                    {
                        report.Add(Report.ReportMessageType.Error,
                            $"Tile {key} init failed.");
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

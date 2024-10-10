using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinimalLock;

using SmallGeometry.Euclidean;

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
        /// 
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

                var buildReport = BuildIndex(maxThreads);
                initReport.Merge(buildReport);

                IsInitDone = true;

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
                    }
                }
            }
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

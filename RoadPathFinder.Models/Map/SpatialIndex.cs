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


        private IReadOnlyDictionary<long, GraphLink> _graph { get; }
        private Dictionary<string, HashSet<long>> _tile { get; set; } = new();
        private MutexSingle _initMutex { get; } = new();


        public SpatialIndex(IReadOnlyDictionary<long, GraphLink> graph, double tileSideLength = 100)
        {
            ArgumentNullException.ThrowIfNull(graph, nameof(graph));
            _graph = graph;

            TileSideLength = tileSideLength;

            _initMutex.PollingIntervalMs = 10;
            _initMutex.DefaultWaitTimeoutMs = 30000;
        }


        public async Task<ReportMessages> Init(bool refresh, int maxThreads)
        {
            var result = new ReportMessages();

            if (IsInitDone
                && !refresh)
            {
                result.Normal.Add("Init already done.");
            }
            else if (await _initMutex.TryAcquireAfterWait())
            {





                IsInitDone = true;

                if (_initMutex.TryRelease())
                {
                    result.Normal.Add("OK");
                }
                else
                {
                    result.Fatal.Add("Init exit incompletely.");
                }
            }
            else
            {
                result.Normal.Add("Init already in progress.");
                result.Warning.Add("Init waiting timeout.");
            }

            return result;
        }


        private Dictionary<string, HashSet<long>> BuildIndex(
            int maxThreads,
            out ReportMessages report)
        {
            var tempIndex = new ConcurrentDictionary<string, ConcurrentBag<long>>();

            if (maxThreads > 1)
            {
                Parallel.ForEach(_graph.Values, new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
                    link =>
                    {

                    });
            }
            else
            {

            }


            
        }

        private void AddToTile(
            GraphLink link,
            ref ConcurrentDictionary<string, ConcurrentBag<long>> tempIndex)
        {
            FlatLine l = link.Geometry.Interpolate(TileSideLength);
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

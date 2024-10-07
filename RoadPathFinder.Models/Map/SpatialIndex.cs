using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MinimalLock;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Map
{
    internal class SpatialIndex
    {
        public int TileSideLength { get; }
        public bool IsInitDone { get; private set; } = false;
        public bool IsInitInProgress => _initMutex.IsLocked();
        public bool IsInitFail { get; private set; } = false;


        private IReadOnlyDictionary<long, GraphLink> _graph { get; }
        private Dictionary<string, HashSet<long>> _tile { get; set; } = new();
        private MutexSingle _initMutex { get; } = new();


        public SpatialIndex(IReadOnlyDictionary<long, GraphLink> graph, int tileSideLength = 100)
        {
            ArgumentNullException.ThrowIfNull(graph, nameof(graph));
            _graph = graph;

            TileSideLength = tileSideLength;

            _initMutex.PollingIntervalMs = 10;
            _initMutex.DefaultWaitTimeoutMs = 30000;
        }
    }
}

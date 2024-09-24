using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Map
{
    public class GraphSet
    {
        public IReadOnlyDictionary<long, GraphLink> Graph => _graph;
        public GraphSetDetails MapInfo { get; private set; }
        public bool IsInitDone => _grid != null && _grid.IsInitDone;

        private Dictionary<long, GraphLink> _graph { get; set; }
        private SpatialIndex _grid { get; }


        public GraphSet(Dictionary<long, GraphLink> graph)
        {
            ArgumentNullException.ThrowIfNull(graph, nameof(graph));
            _graph = graph;
            _grid = new(graph);
        }
    }
}

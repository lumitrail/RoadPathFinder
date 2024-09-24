using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Map
{
    public class SpatialIndex
    {
        public bool IsInitDone { get; private set; } = false;


        private IReadOnlyDictionary<long, GraphLink> _graph { get; }

        public SpatialIndex(IReadOnlyDictionary<long, GraphLink> graph)
        {
            ArgumentNullException.ThrowIfNull(graph, nameof(graph));
            _graph = graph;
        }
    }
}

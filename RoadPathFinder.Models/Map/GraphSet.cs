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
        public GraphSetDetails MapInfo { get; private set; } = new GraphSetDetails { };
        public bool IsInitDone => _grid != null && _grid.IsInitDone;

        private Dictionary<long, GraphLink> _graph { get; set; }
        private SpatialIndex _grid { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="links"></param>
        public GraphSet(IEnumerable<GraphLink> links)
        {
            ArgumentNullException.ThrowIfNull(links, nameof(links));
            _graph = links.ToDictionary(l => l.ID);
            _grid = new(_graph);
        }

        public async Task<double> Init(int maxThreads = 4)
        {
            double elapsedTime = 0;

        }
    }
}

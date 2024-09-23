using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPathFinder.Models.Map
{
    public class GraphSet
    {

        public GraphSetDetails MapInfo { get; }

        private SpatialIndex Grid { get; }
    }
}

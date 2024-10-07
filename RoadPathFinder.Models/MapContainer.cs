using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models.Map;

namespace RoadPathFinder.Models
{
    public class MapContainer
    {
        public ConcurrentDictionary<string, Map.GraphSet> GraphSets { get; } = new();
    }
}

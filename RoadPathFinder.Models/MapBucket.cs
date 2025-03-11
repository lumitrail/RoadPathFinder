using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models.Map;

namespace RoadPathFinder.Models
{
    public class MapBucket
    {
        public ConcurrentDictionary<string, MapSpace> GraphSets { get; } = new();
    }
}

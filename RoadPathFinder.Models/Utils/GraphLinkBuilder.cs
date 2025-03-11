using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Utils
{
    public class GraphLinkBuilder
    {
        public ConcurrentDictionary<long, GraphNode> Nodes { get; } = new();


        public ConcurrentDictionary<long, EDirection> StartNodeConnectedLinks { get; } = new();
        public ConcurrentDictionary<long, EDirection> EndNodeConnectedLinks { get; } = new();




        public void Init()
        {

        }

        public bool TryBuild(ref GraphLink linkData)
        {
            if (linkData == null)
            {
                return false;
            }
        }
    }
}

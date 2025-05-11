using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using SmallGeometry.Euclidean;

using RoadPathFinder.Models;
using RoadPathFinder.Models.Elements;
using RoadPathFinder.Models.Map;

using RoadPathFinder.Agents.AStarIntermediates;

namespace RoadPathFinder.Agents
{
    public static class AStar
    {

        public static SearchResult FindPath(MapSpace map,
            long fromLinkID, EDirection fromLinkDirection,
            long toLinkID, EDirection toLinkDirection,
            FlatPoint destination, int maxIterations,
            ILogger? logger, object[]? loggerInfo)
        {
            Debug.Assert(map != null);
            Debug.Assert(fromLinkID != toLinkID);
            Debug.Assert(map.Graph.ContainsKey(fromLinkID));
            Debug.Assert(map.Graph.ContainsKey(toLinkID));
            Debug.Assert(map.CoordinateSystem == destination.CoordinateSystem);
            Debug.Assert(maxIterations > 0);

            var fromLink = map.Graph[fromLinkID];
            var toLink = map.Graph[toLinkID];

            var candidates = new AStarCandidates();
            var visited = new Dictionary<long, RouteTreeNode>();

            // initial conditions for searching
            if (fromLinkDirection == EDirection.Forward
                || fromLinkDirection == EDirection.Both)
            {
                var fromLinkForwardStart = new RouteTreeNode(fromLink, true, destination);
                candidates.Add(fromLinkForwardStart);
            }
            
            if (fromLinkDirection == EDirection.Backward
                || fromLinkDirection == EDirection.Both)
            {
                var fromLinkBackwardStart = new RouteTreeNode(fromLink, false, destination);
                candidates.Add(fromLinkBackwardStart);
            }

            // start searching!
            for (int i = 0; i < maxIterations; i++)
            {
                if (candidates.CheckArrival(toLinkID, toLinkDirection, out RouteTreeNode? goal))
                {
#error when arrived
                }

#error continue searching
            }
        }
    }
}

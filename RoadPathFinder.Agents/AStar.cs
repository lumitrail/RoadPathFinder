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

namespace RoadPathFinder.Agents
{
    public static class AStar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fromLinkID"></param>
        /// <param name="fromLinkDirection"></param>
        /// <param name="toLinkID"></param>
        /// <param name="toLinkDirection"></param>
        /// <param name="logger"></param>
        /// <param name="loggerInfo"></param>
        /// <returns></returns>
        public static SearchResult FindPath(MapSpace map,
            long fromLinkID, EDirection fromLinkDirection,
            long toLinkID, EDirection toLinkDirection,
            ILogger? logger, object[]? loggerInfo)
        {
            Debug.Assert(map != null);
            Debug.Assert(map.Graph.ContainsKey(fromLinkID));
            Debug.Assert(map.Graph.ContainsKey(toLinkID));
            

        }
    }
}

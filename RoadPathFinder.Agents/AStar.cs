using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models;
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
        /// <param name="toLinkID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SearchResult FindPath(GraphSet map, long fromLinkID, long toLinkID)
        {
            ArgumentNullException.ThrowIfNull(map, nameof(map));


        }
    }
}

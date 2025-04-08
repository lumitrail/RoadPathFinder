using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static SearchResult FindPath(MapSpace map,
            long fromLinkID, EDirection fromLinkDirection,
            long toLinkID, EDirection toLinkDirection)
        {
            ArgumentNullException.ThrowIfNull(map, nameof(map));

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models;
using RoadPathFinder.Models.Composites;
using RoadPathFinder.Models.Map;

namespace RoadPathFinder.Agents
{
    public static class AStar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fromLink"></param>
        /// <param name="toLink"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SearchResult FindPath(MapSpace map,
            LinkDirectionDesignation fromLink,
            LinkDirectionDesignation toLink)
        {
            ArgumentNullException.ThrowIfNull(map, nameof(map));

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoadPathFinder.Models;
using RoadPathFinder.Models.Elements;
using RoadPathFinder.Models.Map;

namespace RoadPathFinder
{
    public class PathFinder
    {
        public SearchResult Find(MapSpace map,
            long fromLinkID, EDirection fromLinkDirection,
            long toLinkID, EDirection toLinkDirection)
        {
            var resultID = new Guid();

            if (ContainsFromLinkAndToLink(map, fromLinkID, toLinkID)
                != SearchResult.EResultFlags.Success)
            {
                return new SearchResult()
                {
                    ResultID = resultID,
                    FlatCoordinates = null,
                    Links = null,
                    MapID = map.ID,
                    ResultFlags = SearchResult.EResultFlags.Success,
                };
            }

            throw new NotImplementedException();
        }

        // TO DO
        // Search result caching + identical mutex
        // load balancing?

        private static SearchResult.EResultFlags ContainsFromLinkAndToLink(
            MapSpace map,
            long fromLinkID,
            long toLinkID)
        {
            Debug.Assert(map != null);
            var linkExistence = SearchResult.EResultFlags.Success;

            if (!map.Graph.ContainsKey(fromLinkID))
            {
                linkExistence = linkExistence | SearchResult.EResultFlags.FromLinkNotFound;
            }

            if (!map.Graph.ContainsKey(toLinkID))
            {
                linkExistence = linkExistence | SearchResult.EResultFlags.ToLinkNotFound;
            }

            return linkExistence;
        }
    }
}

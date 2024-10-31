using SmallGeometry.Euclidean;
using SmallGeometry.Exceptions;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Map
{
    /// <summary>
    /// Map with spatial index
    /// </summary>
    public class GraphSet
    {
        /// <summary>
        /// 
        /// </summary>
        public SmallGeometry.CoordinateSystem CoordinateSystem { get; }
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<long, GraphLink> Graph => _graph;
        /// <summary>
        /// 
        /// </summary>
        public GraphSetDetails MapInfo { get; private set; } = new GraphSetDetails { };
        /// <summary>
        /// is ready to use
        /// </summary>
        public bool IsInitDone => _grid != null && _grid.IsInitDone;

        private Dictionary<long, GraphLink> _graph { get; set; }
        private SpatialIndex _grid { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="links"></param>
        /// <exception cref="ArgumentNullException">links is null</exception>
        /// <exception cref="CoordinateSystemDiscordanceException">links has links of multiple coordinate systems</exception>
        public GraphSet(IEnumerable<GraphLink> links)
        {
            ArgumentNullException.ThrowIfNull(links, nameof(links));
            var coordinateSystems = links.Select(l => l.Geometry.CoordinateSystem).Distinct();
            if (coordinateSystems.Count() != 1)
            {
                throw new CoordinateSystemDiscordanceException(coordinateSystems);
            }
            CoordinateSystem = coordinateSystems.First();
            _graph = links.ToDictionary(l => l.ID);
            _grid = new(Graph);
        }

        /// <summary>
        /// init includes: spatial index init
        /// </summary>
        /// <param name="refresh"></param>
        /// <param name="maxThreads"></param>
        /// <returns></returns>
        public async Task<Report> Init(bool refresh = false, int maxThreads = 4)
        {
            Task<Report> spatialIndexInit = _grid.Init(refresh, maxThreads);
            Report spatialIndexInitReport = await spatialIndexInit;
            return spatialIndexInitReport;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="maxDistanceMeter"></param>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemDiscordanceException"></exception>
        public SortedDictionary<double, List<GraphLink>> SearchNearLinks(FlatPoint center, double maxDistanceMeter)
        {
            if (center.CoordinateSystem != CoordinateSystem)
            {
                throw new CoordinateSystemDiscordanceException(CoordinateSystem, center.CoordinateSystem);
            }
            var result = _grid.SearchLinksWithinDistance(center, maxDistanceMeter);
            return result;
        }


    }
}

using SmallGeometry.Exceptions;
using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;

using RoadPathFinder.Models.Map;

namespace RoadPathFinder.Models
{
    public class SearchResult
    {
        [Flags]
        public enum EResultFlags
        {
            Success = 0,
            FromLinkNotFound,
            ToLinkNotFound,
            RouteNotFound,
        }

        public required Guid ResultID { get; init; } = new Guid();
        
        public required EResultFlags ResultFlags { get; init; }
        public required Guid MapID { get; init; }
        public required FlatLine? FlatCoordinates { get; init; }
        public required IReadOnlyList<long>? Links { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CoordinateSystemNoneException"></exception>
        /// <exception cref="TransformException"></exception>
        public GeoLine GetLngLatCoordinates()
        {
            return new GeoLine(FlatCoordinates.Select(c => c.TransformToGeoPoint()));
        }
    }
}

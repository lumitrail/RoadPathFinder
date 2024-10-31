using SmallGeometry.Exceptions;
using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;

using RoadPathFinder.Models.Map;

namespace RoadPathFinder.Models
{
    public class SearchResult
    {
        public required GraphSetDetails MapInfo { get; init; }
        public required IReadOnlyList<long> Links { get; init; }
        public required FlatLine FlatCoordinates { get; init; }

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

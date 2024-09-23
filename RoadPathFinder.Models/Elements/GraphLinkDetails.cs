namespace RoadPathFinder.Models.Elements
{
    public class GraphLinkDetails
    {
        /// <summary>link ID</summary>
        public required long ID { get; init; }
        /// <summary>no of lanes</summary>
        public required ushort Lanes { get; init; }
        /// <summary></summary>
        public required bool IsPassengerReachable { get; init; }
    }
}

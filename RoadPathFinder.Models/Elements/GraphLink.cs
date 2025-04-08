using System.Collections.Concurrent;

using SmallGeometry.Euclidean;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// immutable data of a link
    /// </summary>
    /// <remarks>
    public class GraphLink
    {
        /// <summary>
        /// ID
        /// </summary>
        public required long ID { get; init; }

        /// <summary>
        /// link geometry, in order of "Forward"
        /// </summary>
        public required FlatLine Geometry { get; init; }

        /// <summary>
        /// Oneway: only allowed to travel from StartNode to EndNode
        /// </summary>
        public required bool IsOneway { get; init; }

        /// <summary>
        /// ID of node where link starts at
        /// </summary>
        public required long StartNodeID { get; init; }
        /// <inheritdoc cref="_startNodeConnectedLinkIDs"/>
        public IReadOnlyDictionary<long, EDirection> StartNodeConnectedLinkIDs => _startNodeConnectedLinkIDs;
        /// <summary>
        /// start node를 통해 진출할 수 있는 link들의 ID
        /// </summary>
        /// <remarks>value: 연결된 링크로 진출했을 때, 진행 방향</remarks>
        private ConcurrentDictionary<long, EDirection> _startNodeConnectedLinkIDs { get; } = new();

        /// <summary>
        /// ID of node where link ends at
        /// </summary>
        public required long EndNodeID { get; init; }
        /// <inheritdoc cref="_endNodeConnectedLinkIDs"/>
        public IReadOnlyDictionary<long, EDirection> EndNodeConnectedLinkIDs => _endNodeConnectedLinkIDs;
        /// <summary>
        /// end node를 통해 진출할 수 있는 link들의 id
        /// </summary>
        /// <remarks>value: 연결된 링크로 진출했을 때, 진행 방향</remarks>
        private ConcurrentDictionary<long, EDirection> _endNodeConnectedLinkIDs { get; } = new();



        /// <summary>
        /// position of start node
        /// </summary>
        public FlatPoint First() => Geometry.First();

        /// <summary>
        /// position of end node
        /// </summary>
        public FlatPoint Last() => Geometry.Last();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if added</returns>
        public bool AddToConnectedLink(GraphLink link)
        {
            if (link == null
                || link.ID == this.ID)
            {
                return false;
            }

            if (!IsOneway
                && this.StartNodeID == link.StartNodeID)
            {
                _startNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Forward);
                return true;
            }
            else if (!IsOneway
                && this.StartNodeID == link.EndNodeID)
            {
                _startNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Backward);
                return true;
            }
            else if (this.EndNodeID == link.StartNodeID)
            {
                _endNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Forward);
                return true;
            }
            else if (this.EndNodeID == link.EndNodeID)
            {
                _endNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Backward);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

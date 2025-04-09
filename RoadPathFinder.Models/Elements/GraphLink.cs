using System.Collections.Concurrent;

using SmallGeometry.Euclidean;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// immutable data of a link
    /// </summary>
    public class GraphLink
    {
        /// <summary>
        /// ID MUST BE POSITIVE!
        /// </summary>
        public required long ID
        {
            get => _id;
            init
            {
                _id = value > 0 ? value
                    : throw new ArgumentOutOfRangeException(nameof(value), value, "Graph link ID must be positive.");
            }
        }
        private long _id;


        /// <summary>
        /// link geometry. order of points is of "Forward"
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
        /// <returns>true if added</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddToConnectedLink(GraphLink link)
        {
            ArgumentNullException.ThrowIfNull(link, nameof(link));

            if (link.ID == this.ID)
            {
                return false;
            }
            else if (!IsOneway
                && this.StartNodeID == link.StartNodeID)
            {
                return _startNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Forward);
            }
            else if (!IsOneway
                && this.StartNodeID == link.EndNodeID)
            {
                return _startNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Backward);
            }
            else if (this.EndNodeID == link.StartNodeID)
            {
                return _endNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Forward);
            }
            else if (this.EndNodeID == link.EndNodeID)
            {
                return _endNodeConnectedLinkIDs.TryAdd(link.ID, EDirection.Backward);
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns>true when found and deleted</returns>
        public bool RemoveConnectedLink(long linkID)
        {
            bool startNodeConnected = _startNodeConnectedLinkIDs.TryRemove(linkID, out _);
            bool endNodeConnected = _startNodeConnectedLinkIDs.TryRemove(linkID, out _);

            return startNodeConnected || endNodeConnected;
        }
    }
}
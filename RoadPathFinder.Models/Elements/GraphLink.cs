using SmallGeometry.Euclidean;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// immutable data of a link
    /// </summary>
    public class GraphLink
    {
        /// <summary></summary>
        public long ID { get; }

        /// <summary>Oneway: only allowed to travel from StartNode to EndNode</summary>
        public bool IsOneway { get; }
        /// <summary>link geometry, StartNode-EndNode</summary>
        public FlatLine Geometry { get; }

        /// <summary>ID of node where link starts at</summary>
        public long StartNodeID { get; }
        /// <summary>position of start node</summary>
        public FlatPoint StartNode { get; }
        /// <summary>start node를 통해 진출할 수 있는 link들의 ID</summary>
        /// <remarks>진출 시 해당 링크들의 end node를 통하는 경우 음수 값</remarks>
        public virtual IReadOnlySet<long> StartNodeConnectedLinkIDs { get; }

        /// <summary>ID of node where link ends at</summary>
        public long EndNodeID { get; }
        /// <summary>position of end node</summary>
        public FlatPoint EndNode { get; }
        /// <summary>end node를 통해 진출할 수 있는 link들의 id</summary>
        /// <remarks>진출 시 해당 링크들의 end node를 통하는 경우 음수 값</remarks>
        public virtual IReadOnlySet<long> EndNodeConnectedLinkIDs { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isOneway"></param>
        /// <param name="geometry"></param>
        /// <param name="startNodeID"></param>
        /// <param name="endNodeID"></param>
        /// <param name="startNodeConnectedLinkIDs"></param>
        /// <param name="endNodeConnectedLinkIDs"></param>
        /// <exception cref="ArgumentNullException">geometry</exception>
        public GraphLink(long id, bool isOneway, FlatLine geometry, long startNodeID, long endNodeID,
            IEnumerable<long> startNodeConnectedLinkIDs,
            IEnumerable<long> endNodeConnectedLinkIDs)
        {
            ArgumentNullException.ThrowIfNull(geometry, nameof(geometry));

            ID = id;
            IsOneway = isOneway;
            Geometry = geometry;

            StartNodeID = startNodeID;
            StartNode = geometry.First();
            StartNodeConnectedLinkIDs = startNodeConnectedLinkIDs.ToHashSet();

            EndNodeID = endNodeID;
            EndNode = geometry.Last();
            EndNodeConnectedLinkIDs = endNodeConnectedLinkIDs.ToHashSet();
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="b"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphLink(GraphLink b)
            : this(b?.ID ?? throw new ArgumentNullException(nameof(b)),
                  b.IsOneway, b.Geometry, b.StartNodeID, b.EndNodeID,
                  b.StartNodeConnectedLinkIDs,
                  b.EndNodeConnectedLinkIDs)
        {
        }
    }
}

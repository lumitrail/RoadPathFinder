using SmallGeometry.Euclidean;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// Link의 양 끝에 결합되어야 하는 node
    /// </summary>
    public class GraphNode
    {
        /// <summary></summary>
        public long ID { get; }
        /// <summary></summary>
        public FlatPoint Location { get; }

        /// <summary></summary>
        public IReadOnlySet<long> ConnectedLinkIDs => _connectedLinkIDs;


        private HashSet<long> _connectedLinkIDs { get; }
            = new HashSet<long>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        public GraphNode(long id, FlatPoint location)
        {
            ID = id;
            Location = location;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkID"></param>
        /// <returns></returns>
        public bool AddConnectedLink(long linkID)
        {
            return _connectedLinkIDs.Add(linkID);
        }
    }
}

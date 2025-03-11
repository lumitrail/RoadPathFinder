using System.Collections.Immutable;

using SmallGeometry.Euclidean;

using RoadPathFinder.Models.Utils;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// immutable data of a link
    /// </summary>
    /// <remarks>
    /// use <see cref="GraphLinkBuilder"/> to init</remarks>
    public class GraphLink
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; internal init; }

        /// <summary>
        /// link geometry, in order of "Forward"
        /// </summary>
        public FlatLine Geometry { get; internal init; }

        /// <summary>
        /// Oneway: only allowed to travel from StartNode to EndNode
        /// </summary>
        public bool IsOneway { get; internal init; }

        /// <summary>
        /// ID of node where link starts at
        /// </summary>
        public long StartNodeID { get; internal init; }
        /// <summary>
        /// start node를 통해 진출할 수 있는 link들의 ID
        /// </summary>
        public ImmutableDictionary<long, EDirection> StartNodeConnectedLinkIDs { get; internal init; }

        /// <summary>
        /// ID of node where link ends at
        /// </summary>
        public long EndNodeID { get; internal init; }
        /// <summary>
        /// end node를 통해 진출할 수 있는 link들의 id
        /// </summary>
        public ImmutableDictionary<long, EDirection> EndNodeConnectedLinkIDs { get; internal init; }


        [Obsolete]
        public GraphLink()
        {
        }


        /// <summary>
        /// position of start node
        /// </summary>
        public FlatPoint First() => Geometry.First();

        /// <summary>
        /// position of end node
        /// </summary>
        public FlatPoint Last() => Geometry.Last();
    }
}

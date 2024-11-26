using SmallGeometry.Euclidean;

using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Agents.AStarIntermediates
{
    internal class RouteTreeNode
    {
        public GraphLink Link { get; }
        public long DirectionalLinkID { get; }

        public RouteTreeNode? ParentNode { get; } = null;
        /// <summary>
        /// Sum of all length of all ancestor links and this link
        /// </summary>
        public double AccumulatedDistance { get; }

        /// <summary>
        /// Straight distance to destination from this link
        /// </summary>
        public double HeuristicDistance { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <param name="traversingLinkReversal"></param>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentNullException">link</exception>
        public RouteTreeNode(
            GraphLink link,
            bool traversingLinkReversal,
            FlatPoint destination)
        {
            ArgumentNullException.ThrowIfNull(link, nameof(link));

            Link = link;

            AccumulatedDistance = link.Geometry.GetLength();

            if (traversingLinkReversal)
            {
                // reversal
                DirectionalLinkID = -link.ID;
                HeuristicDistance = destination.GetDistance(link.StartNode);
            }
            else
            {
                // straight
                DirectionalLinkID = link.ID;
                HeuristicDistance = destination.GetDistance(link.EndNode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <param name="parentNode"></param>
        /// <param name="destination"></param>
        /// <exception cref="ArgumentException">parent node and link is not connected</exception>
        /// <exception cref="ArgumentNullException">link, parentNode</exception>
        public RouteTreeNode(
            GraphLink link,
            RouteTreeNode parentNode,
            FlatPoint destination)
            : this(link, !IsStraight(link, parentNode), destination)
        {
            ParentNode = parentNode;
            AccumulatedDistance += parentNode?.AccumulatedDistance ?? 0;
        }


        /// <summary>
        /// is this link traveling straight or reversed?
        /// </summary>
        /// <param name="link"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">parent node and link is not connected</exception>
        /// <exception cref="ArgumentNullException">link, parentNode</exception>
        private static bool IsStraight(GraphLink link, RouteTreeNode parentNode)
        {
            ArgumentNullException.ThrowIfNull(link, nameof(link));
            ArgumentNullException.ThrowIfNull(parentNode, nameof(parentNode));

            long parentEndNodeID = parentNode.GetDirectionalEndNodeID();

            if (parentEndNodeID == link.StartNodeID)
            {
                return true;
            }
            else if (parentEndNodeID == link.EndNodeID)
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"Link {link.ID}({link.StartNodeID}-{link.EndNodeID}) is not connected to parent link {parentNode.Link.ID}(->{parentEndNodeID})");
            }
        }


        private long GetDirectionalEndNodeID()
        {
            if (DirectionalLinkID > 0)
            {
                return Link.EndNodeID;
            }
            else
            {
                return Link.StartNodeID;
            }
        }
    }
}

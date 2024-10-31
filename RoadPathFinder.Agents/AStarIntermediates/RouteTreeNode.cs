using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Agents.AStarIntermediates
{
    internal class RouteTreeNode
    {
        public GraphLink Link { get; }
        public bool TraversingLinkReversal { get; }

        public RouteTreeNode? ParentNode { get; }
        public double AccumulatedDistance { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        /// <param name="traversingLinkReversal"></param>
        /// <param name="parentNode"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RouteTreeNode(GraphLink link, bool traversingLinkReversal,
            RouteTreeNode? parentNode)
        {
            ArgumentNullException.ThrowIfNull(link, nameof(link));

            Link = link;
            TraversingLinkReversal = traversingLinkReversal;

            ParentNode = parentNode;
            AccumulatedDistance = parentNode?.AccumulatedDistance ?? 0;
            AccumulatedDistance += Link.Geometry.GetLength();
        }
    }
}

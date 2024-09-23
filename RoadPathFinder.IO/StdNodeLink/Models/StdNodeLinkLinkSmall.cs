using SmallGeometry.Euclidean;

namespace RoadPathFinder.IO.StdNodeLink.Models
{
    public class StdNodeLinkLinkSmall
    {
        /// <summary>링크 ID</summary>
        public long LinkId { get; }
        /// <summary>시작 노드 ID</summary>
        public long FNode { get; }
        /// <summary>종료 노드 ID</summary>
        public long TNode { get; }
        /// <summary>링크 geom</summary>
        public FlatLine Geom { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="fNode"></param>
        /// <param name="tNode"></param>
        /// <param name="geom"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StdNodeLinkLinkSmall(long linkId, long fNode, long tNode, FlatLine geom)
        {
            ArgumentNullException.ThrowIfNull(geom, nameof(geom));

            LinkId = linkId;
            FNode = fNode;
            TNode = tNode;
            Geom = geom;
        }
    }
}

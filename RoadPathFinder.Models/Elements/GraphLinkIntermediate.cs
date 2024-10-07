using System.Collections.Concurrent;

using SmallGeometry.Euclidean;

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphLinkIntermediate : GraphLink
    {
        public override IReadOnlySet<long> StartNodeConnectedLinkIDs => _startNodeConnectedLinkIDs;
        public override IReadOnlySet<long> EndNodeConnectedLinkIDs => _endNodeConnectedLinkIDs;

        private ConcurrentBag<long> _startNodeConnectedLinkIDsInput { get; set; } = new();
        private ConcurrentBag<long> _endNodeConnectedLinkIDsInput { get; set; } = new();
        /// <summary>accessible links from this link throgh the start node</summary>
        private HashSet<long> _startNodeConnectedLinkIDs { get; } = new();
        /// <summary>accessible links from this link throgh the end node</summary>
        private HashSet<long> _endNodeConnectedLinkIDs { get; } = new();


        /// <inheritdoc cref="GraphLink.GraphLink(long, bool, FlatLine, long, long, IEnumerable{long}, IEnumerable{long})"/>
        /// <param name="id"></param>
        /// <param name="isOneway"></param>
        /// <param name="geometry"></param>
        /// <param name="startNodeID"></param>
        /// <param name="endNodeID"></param>
        public GraphLinkIntermediate(long id, bool isOneway, FlatLine geometry, long startNodeID, long endNodeID)
            : base(id, isOneway, geometry, startNodeID, endNodeID,
                  [], [])
        {
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="b"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphLinkIntermediate(GraphLinkIntermediate b)
            : this(b?.ID??throw new ArgumentNullException(nameof(b)),
                  b.IsOneway, b.Geometry, b.StartNodeID, b.EndNodeID)
        {
            foreach (var id in b.StartNodeConnectedLinkIDs)
            {
                _startNodeConnectedLinkIDs.Add(id);
            }

            foreach (var id in b.EndNodeConnectedLinkIDs)
            {
                _endNodeConnectedLinkIDs.Add(id);
            }
        }

        /// <summary>
        /// 이 링크에 연결된 링크일 경우 연결 정보 기록, 완료 시 <see cref="ApplyConnectedLink"/> 해야 적용됨
        /// </summary>
        /// <param name="link"></param>
        public bool RegisterConnectedLink(GraphLinkIntermediate link)
        {
            if (link == null
                || link.ID == ID)
            {
                return false;
            }

            bool isAdded = false;

            // 지금 거 역진행
            if (!IsOneway)
            {
                if (StartNodeID == link.StartNodeID) // 다음 거 순진행
                {
                    _startNodeConnectedLinkIDsInput.Add(link.ID);
                    isAdded = true;
                }
                if (!link.IsOneway
                    && StartNodeID == link.EndNodeID) // 다음 거 역진행
                {
                    _startNodeConnectedLinkIDsInput.Add(-link.ID);
                    isAdded = true;
                }
            }

            // 지금 거 순진행
            if (EndNodeID == link.StartNodeID) // 다음 거 순진행
            {
                _endNodeConnectedLinkIDsInput.Add(link.ID);
                isAdded = true;
            }
            if (!link.IsOneway
                && EndNodeID == link.EndNodeID) // 다음 거 역진행
            {
                _endNodeConnectedLinkIDsInput.Add(-link.ID);
                isAdded = true;
            }

            return isAdded;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ApplyConnectedLink()
        {
            var s = _startNodeConnectedLinkIDsInput;
            _startNodeConnectedLinkIDsInput = new();
            foreach (var linkID in s)
            {
                _startNodeConnectedLinkIDs.Add(linkID);
            }

            var e = _endNodeConnectedLinkIDsInput;
            _endNodeConnectedLinkIDsInput = new();
            foreach (var linkID in e)
            {
                _endNodeConnectedLinkIDs.Add(linkID);
            }

        }

        /// <summary>
        /// 이 링크가 막다른 길일 경우 되돌아 나올 수 있게 함
        /// </summary>
        internal void MakeReversible()
        {
            if (_startNodeConnectedLinkIDs.Count == 0)
            {
                _startNodeConnectedLinkIDs.Add(ID);
            }
            else if (_endNodeConnectedLinkIDs.Count == 0
                && !IsOneway)
            {
                _endNodeConnectedLinkIDs.Add(-ID);
            }
        }
    }
}

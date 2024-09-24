using System.Collections.Concurrent;

using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;

namespace RoadPathFinder.Models.Elements
{
    public class GraphLink
    {
        /// <summary>Link ID</summary>
        public long ID { get; }

        /// <summary>Oneway: only allowed to travel from StartNode to EndNode</summary>
        public bool IsOneway { get; }
        /// <summary>link geometry, StartNode-EndNode</summary>
        public FlatLine Geometry { get; }

        /// <summary>ID of node where link starts at</summary>
        public long StartNodeID { get; }
        /// <summary>position of start node</summary>
        public FlatPoint StartNode => Geometry.First();
        /// <summary>start node를 통해 진출할 수 있는 link들의 ID</summary>
        /// <remarks>진출 시 해당 링크들의 end node를 통하는 경우 음수 값</remarks>
        public IReadOnlySet<long> StartNodeConnectedLinkIDs => _startNodeConnectedLinkIDs;

        /// <summary>ID of node where link ends at</summary>
        public long EndNodeID { get; }
        /// <summary>position of end node</summary>
        public FlatPoint EndNode => Geometry.Last();
        /// <summary>end node를 통해 진출할 수 있는 link들의 id</summary>
        /// <remarks>진출 시 해당 링크들의 end node를 통하는 경우 음수 값</remarks>
        public IReadOnlySet<long> EndNodeConnectedLinkIDs => _endNodeConnectedLinkIDs;

        private ConcurrentQueue<long> _startNodeConnectedLinkIDsInput { get; set; } = new();
        private ConcurrentQueue<long> _endNodeConnectedLinkIDsInput { get; set; } = new();
        /// <summary>accessible links from this link throgh the start node</summary>
        private HashSet<long> _startNodeConnectedLinkIDs { get; } = new();
        /// <summary>accessible links from this link throgh the end node</summary>
        private HashSet<long> _endNodeConnectedLinkIDs { get; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphLink(long id, bool isOneway, FlatLine geometry, long startNodeID, long endNodeID)
        {
            ArgumentNullException.ThrowIfNull(geometry, nameof(geometry));

            ID = id;
            IsOneway = isOneway;
            Geometry = geometry;
            StartNodeID = startNodeID;
            EndNodeID = endNodeID;
        }

        /// <summary>
        /// Deep copy
        /// </summary>
        /// <returns></returns>
        public GraphLink GetCopy()
        {
            var result = new GraphLink(ID, IsOneway, Geometry, StartNodeID, EndNodeID);

            foreach (var linkID in StartNodeConnectedLinkIDs)
            {
                result._startNodeConnectedLinkIDs.Add(linkID);
            }

            foreach (var linkID in EndNodeConnectedLinkIDs)
            {
                result._endNodeConnectedLinkIDs.Add(linkID);
            }

            return result;
        }

        /// <summary>
        /// 이 link의 위경도 좌표계 형상을 얻음
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SmallGeometry.Exceptions.TransformException"></exception>
        public GeoLine GetGeoLine()
        {
            var geoPoints = Geometry
                 .Select(p => p.TransformToGeoPoint());
            var result = new GeoLine(geoPoints);

            return result;
        }

        /// <summary>
        /// 이 링크에 연결된 링크일 경우 연결 정보 기록, 완료 시 <see cref="ApplyConnectedLink"/> 해야 적용됨
        /// </summary>
        /// <param name="link"></param>
        public bool RegisterConnectedLink(GraphLink link)
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
                    _startNodeConnectedLinkIDsInput.Enqueue(link.ID);
                    isAdded = true;
                }
                if (!link.IsOneway
                    && StartNodeID == link.EndNodeID) // 다음 거 역진행
                {
                    _startNodeConnectedLinkIDsInput.Enqueue(-link.ID);
                    isAdded = true;
                }
            }

            // 지금 거 순진행
            if (EndNodeID == link.StartNodeID) // 다음 거 순진행
            {
                _endNodeConnectedLinkIDsInput.Enqueue(link.ID);
                isAdded = true;
            }
            if (!link.IsOneway
                && EndNodeID == link.EndNodeID) // 다음 거 역진행
            {
                _endNodeConnectedLinkIDsInput.Enqueue(-link.ID);
                isAdded = true;
            }

            return isAdded;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ApplyConnectedLink()
        {
            foreach (var linkID in _startNodeConnectedLinkIDsInput)
            {
                _startNodeConnectedLinkIDs.Add(linkID);
            }

            _startNodeConnectedLinkIDsInput = new();

            foreach (var linkID in _endNodeConnectedLinkIDsInput)
            {
                _endNodeConnectedLinkIDs.Add(linkID);
            }

            _endNodeConnectedLinkIDsInput = new();
        }

        /// <summary>
        /// 이 링크가 막다른 길일 경우 되돌아 나올 수 있게 함
        /// </summary>
        public void MakeReversible()
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

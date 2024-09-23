using SmallGeometry.Euclidean;
using SmallGeometry.Geographic;

namespace RoadPathFinder.Models.Elements
{
    internal class GraphLink
    {
        /// <summary>Link ID</summary>
        public long ID { get; }

        /// <summary>Oneway: only allowd to travel StartNode-EndNode</summary>
        public bool IsOneWay { get; }
        /// <summary>link geometry, StartNode-EndNode</summary>
        public FlatLine Geometry { get; }
        /// <summary>link length, in meter</summary>
        public double LengthMeter => Geometry.GetLength();

        /// <summary>ID of node where link starts at</summary>
        public long StartNodeID { get; }
        /// <summary>position of start node</summary>
        public FlatPoint StartNode => Geometry.First();

        /// <summary>accessible links from this link throgh the start node</summary>
        private HashSet<long> _startNodeConnectedLinkIdsDirected { get; } = new();
        /// <summary>start node를 통해 진출할 수 있는 link들의 ID</summary>
        /// <remarks>진출 시 해당 링크들의 end node를 통하는 경우 음수 값</remarks>
        public IReadOnlySet<long> StartNodeConnectedLinkIdsDirected => _startNodeConnectedLinkIdsDirected;


        /// <summary>
        /// link가 끝나는 점의 node ID
        /// </summary>
        public long EndNodeID { get; }
        public FlatPoint EndNode => Geometry.Last();
        private HashSet<long> _endNodeConnectedLinkIdsDirected { get; set; }
        /// <summary>
        /// end node를 통해 진출할 수 있는 link들의 id
        /// </summary>
        /// <remarks>
        /// 진출 시 해당 링크들의 end node를 통하는 경우 음수 값
        /// </remarks>
        public IReadOnlySet<long> EndNodeConnectedLinkIdsDirected => _endNodeConnectedLinkIdsDirected;



        public GraphLink DeepCopy()
        {
            var startNodeLinks = new HashSet<long>(_startNodeConnectedLinkIdsDirected);
            var endNodeLinks = new HashSet<long>(_endNodeConnectedLinkIdsDirected);

            var result = new GraphLink(Id, IsOneWay, Geometry, StartNodeId, EndNodeId)
            {
                _startNodeConnectedLinkIdsDirected = startNodeLinks,
                _endNodeConnectedLinkIdsDirected = endNodeLinks
            };

            return result;
        }

        /// <summary>
        /// 이 link의 위경도 좌표계 형상을 얻음
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TransformException"></exception>
        public GeoLine GetGeoLine()
        {
            GeoPoint[] geoPoints = Geometry
                .Select(p => p.TransformToGeoPoint())
                .ToArray();
            var result = new GeoLine(geoPoints);

            return result;
        }

        /// <summary>
        /// 이 링크에 연결된 링크일 경우 연결 정보 기록
        /// </summary>
        /// <param name="link"></param>
        public void AddConnectedLink(GraphLink link)
        {
            if (link == null
                || link.ID == ID)
            {
                return;
            }

            if (StartNodeId == link.StartNodeId)
            {
                _startNodeConnectedLinkIdsDirected.Add(link.ID);
            }
            if (StartNodeId == link.EndNodeId)
            {
                _startNodeConnectedLinkIdsDirected.Add(-link.ID);
            }

            if (EndNodeId == link.StartNodeId)
            {
                _endNodeConnectedLinkIdsDirected.Add(link.ID);
            }
            if (EndNodeId == link.EndNodeId)
            {
                _endNodeConnectedLinkIdsDirected.Add(-link.ID);
            }
        }

        /// <summary>
        /// 이 링크가 막다른 길일 경우 되돌아 나올 수 있게 함
        /// </summary>
        public void MakeReversible()
        {
            if (_startNodeConnectedLinkIdsDirected.Count == 0)
            {
                _startNodeConnectedLinkIdsDirected.Add(ID);
            }
            else if (_endNodeConnectedLinkIdsDirected.Count == 0
                && !IsOneWay)
            {
                _endNodeConnectedLinkIdsDirected.Add(-ID);
            }
        }

        /// <summary>
        /// link의 진행 방향과 heading과의 방향 일치율 (cosine)
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        public double GetLinkConcordance(double heading)
        {

        }

        public double GetLinkConcordance(Vector v)
        {

        }
    }
}

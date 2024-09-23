using SmallGeometry.Euclidean;

using static RoadPathFinder.IO.StdNodeLink.Models.StdNodeLinkEnums;

namespace RoadPathFinder.IO.StdNodeLink.Models
{
    public class StdNodeLinkLinkFull : StdNodeLinkLinkSmall
    {
        /// <summary>
        /// 차로 수
        /// </summary>
        public int Lanes { get; set; }
        /// <summary>
        /// 도로등급
        /// </summary>
        public RoadRank RoadRank { get; set; }
        /// <summary>
        /// 도로유형
        /// </summary>
        public int RoadType { get; set; }
        /// <summary>
        /// 노선번호
        /// </summary>
        public int RoadNo { get; set; }
        public string RoadName { get; set; }
        /// <summary>
        /// 도로사용여부
        /// </summary>
        public bool RoadUse { get; set; }
        /// <summary>
        /// 도로번호가 다른 여러 도로들이 한 링크 안에 있음
        /// </summary>
        public bool MultiLink { get; set; }
        /// <summary>
        /// 연결로코드 존재
        /// </summary>
        public bool Connect { get; set; }
        public int MaxSpd { get; set; }
        /// <summary>
        /// 통행제한차량
        /// </summary>
        public RestVeh RestVeh { get; set; }
        /// <summary>
        /// 통과제한하중
        /// </summary>
        public int RestW { get; set; }
        /// <summary>
        /// 통과제한높이
        /// </summary>
        public int RestH { get; set; }
        /// <summary>
        /// C-ITS 서비스 구간
        /// </summary>
        public char CITS { get; set; }
        public double Length { get; set; }
        public string Updatedate { get; set; }
        /// <summary>
        /// 비고
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 갱신이력유형
        /// </summary>
        public string? HistType { get; set; }
        /// <summary>
        /// 갱신이력설명
        /// </summary>
        public string? Histremark { get; set; }

        public StdNodeLinkLinkFull(long linkId, long fNode, long tNode, FlatLine geom)
            : base(linkId, fNode, tNode, geom)
        {
        }
    }
}

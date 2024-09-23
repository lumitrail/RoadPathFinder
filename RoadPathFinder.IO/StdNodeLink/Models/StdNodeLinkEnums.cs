namespace RoadPathFinder.IO.StdNodeLink.Models
{
    public static class StdNodeLinkEnums
    {
        /// <summary>
        /// Link - 도로 등급
        /// </summary>
        public enum RoadRank
        {
            고속국도 = 101,
            도시고속국도 = 102,
            일반국도 = 103,
            특별광역시도 = 104,
            국가지원지방도 = 105,
            지방도 = 106,
            시군도 = 107,
            기타 = 108
        }

        /// <summary>
        /// Link - 도로 타입
        /// </summary>
        public enum RoadType
        {
            일반도로 = 000,
            고가차도 = 001,
            지하차도 = 002,
            교량 = 003,
            터널 = 004
        }

        /// <summary>
        /// Link - 통행제한차량
        /// </summary>
        public enum RestVeh
        {
            모두통행가능 = 0,
            승용차 = 1,
            승합차 = 2,
            버스 = 3,
            트럭 = 4,
            이륜차 = 5,
            기타 = 6
        }

        /// <summary>
        /// TurnInfo - 회전제한유형
        /// </summary>
        public enum TurnType
        {
            비보호회전 = 1,
            버스만회전 = 2,
            회전금지 = 3,
            U_Turn = 11,
            P_Turn = 12,
            좌회전금지 = 101,
            직진금지 = 102,
            우회전금지 = 103
        }

        /// <summary>
        /// TurnInfo - 회전제한운영시간
        /// </summary>
        public enum TurnOper
        {
            전일제 = 0,
            시간제 = 1
        }
    }
}

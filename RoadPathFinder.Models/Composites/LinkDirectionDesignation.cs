using RoadPathFinder.Models.Elements;

namespace RoadPathFinder.Models.Composites
{
    /// <summary>
    /// 링크와, 그 링크의 어떤 방향으로 진행할지를 의미함
    /// </summary>
    public class LinkDirectionDesignation
    {
        /// <summary>
        /// 링크
        /// </summary>
        public required long ID { get; init; }

        /// <summary>
        /// 진행 방향
        /// </summary>
        public EDirection Direction { get; init; } = EDirection.Both;
    }
}

namespace RoadPathFinder.Models.Elements
{
    /// <summary>
    /// 링크 진행 방향 표현
    /// </summary>
    public enum EDirection
    {
        /// <summary>
        /// 전진 방향만
        /// </summary>
        Forward = 1,
        /// <summary>
        /// 전후진 모두
        /// </summary>
        Both = 0,
        /// <summary>
        /// 후진 방향만
        /// </summary>
        Backward = -1,
    }
}

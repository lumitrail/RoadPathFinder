namespace RoadPathFinder.Models.Utils
{
    public class LoggerStateBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid MapID { get; init; }
        /// <summary>
        /// 
        /// </summary>
        public required string Action { get; init; }
    }
}

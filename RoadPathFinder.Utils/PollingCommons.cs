namespace RoadPathFinder.Utils
{
    public abstract class PollingCommons
    {
        /// <summary>wait granularity, 최소 1ms, 최대 30초</summary>
        public int PollingIntervalMs
        {
            get => _pollingIntervalMs;
            set => _pollingIntervalMs = Math.Min(Math.Max(1, value), 30000);
        }
        private int _pollingIntervalMs = 10;

        /// <summary>wait timeout, 최소 10ms, 최대 1시간</summary>
        public int DefaultWaitTimeoutMs
        {
            get => _defaultWaitTimeoutMs;
            set => _defaultWaitTimeoutMs = Math.Min(Math.Max(10, value), 3600000);
        }
        private int _defaultWaitTimeoutMs = 10000;

        protected bool IsTimeout(DateTime startTime)
        {
            return GetElapsedTimeMs(startTime) >= DefaultWaitTimeoutMs;
        }

        protected double GetElapsedTimeMs(DateTime startTime)
        {
            return (DateTime.Now - startTime).TotalMilliseconds;
        }
    }
}

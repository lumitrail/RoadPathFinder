namespace RoadPathFinder.Utils
{
    public class CustomMutexSet<TResourceID>
        where TResourceID : notnull
    {
        /// <summary>
        /// wait granularity, 최소 1ms, 최대 30초
        /// </summary>
        public int PollingIntervalMs
        {
            get => _pollingIntervalMs;
            set => _pollingIntervalMs = Math.Min(Math.Max(1, value), 30000);
        }
        private int _pollingIntervalMs = 10;

        /// <summary>
        /// wait timeout, 최소 10ms, 최대 1시간
        /// </summary>
        public int DefaultWaitTimeoutMs
        {
            get => _defaultWaitTimeoutMs;
            set => _defaultWaitTimeoutMs = Math.Min(Math.Max(10, value), 3600000);
        }
        private int _defaultWaitTimeoutMs = 10000;

        private const char V = '\n';
        private System.Collections.Concurrent
            .ConcurrentDictionary<TResourceID, char> _tasksOngoing { get; }
            = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns>time waited in ms</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<double> Wait(
            TResourceID id,
            CancellationTokenSource? cancellationTokenSource = null)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            DateTime startTime = DateTime.Now;

            if (cancellationTokenSource == null)
            {
                while (_tasksOngoing.ContainsKey(id))
                {
                    if (IsTimeout(startTime))
                    {
                        // timeout
                        return (DateTime.Now - startTime).TotalMilliseconds;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }
            else
            {
                while (_tasksOngoing.ContainsKey(id))
                {
                    if (cancellationTokenSource.IsCancellationRequested
                        || IsTimeout(startTime))
                    {
                        // canceled OR timeout
                        return (DateTime.Now - startTime).TotalMilliseconds;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }

            return (DateTime.Now - startTime).TotalMilliseconds; ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool IsLocked(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            return _tasksOngoing.ContainsKey(id);
        }

        /// <summary>
        /// 한번 얻어 보기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TryAcquire(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _tasksOngoing.TryAdd(id, V);
        }

        /// <summary>
        /// 기다려서 얻기
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> TryAcquireAfterWait(
            TResourceID id,
            CancellationTokenSource? cancellationTokenSource = null)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            
            DateTime startTime = DateTime.Now;

            if (cancellationTokenSource == null)
            {
                while (_tasksOngoing.TryAdd(id, V))
                {
                    if (IsTimeout(startTime))
                    {
                        // timeout
                        return false;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }
            else
            {
                while (_tasksOngoing.TryAdd(id, V))
                {
                    if (cancellationTokenSource.IsCancellationRequested
                        || IsTimeout(startTime))
                    {
                        // canceled OR timeout
                        return false;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public void TryRelease(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            _tasksOngoing.Remove(id, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TResourceID[] GetLockedResources()
        {
            return _tasksOngoing.Keys.ToArray();
        }

        private bool IsTimeout(DateTime startTime)
        {
            return (DateTime.Now - startTime).TotalMilliseconds >= DefaultWaitTimeoutMs;
        }
    }
}

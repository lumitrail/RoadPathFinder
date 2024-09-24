namespace RoadPathFinder.Utils
{
    public class CustomMutex
    {
        /// <inheritdoc cref="CustomMutexSet{TResourceID}.PollingIntervalMs"/>
        public int PollingIntervalMs
        {
            get => _singleMutexSet.PollingIntervalMs;
            set => _singleMutexSet.PollingIntervalMs = value;
        }

        /// <inheritdoc cref="CustomMutexSet{TResourceID}.DefaultWaitTimeoutMs"/>
        public int DefaultWaitTimeoutMs
        {
            get => _singleMutexSet.DefaultWaitTimeoutMs;
            set => _singleMutexSet.DefaultWaitTimeoutMs = value;
        }


        private const char KEY = '0';
        private CustomMutexSet<char> _singleMutexSet { get; } = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        /// <returns>time waited in ms</returns>
        public async Task<double> Wait(CancellationTokenSource? cancellationTokenSource = null)
        {
            return await _singleMutexSet.Wait(KEY, cancellationTokenSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLocked()
        {
            return _singleMutexSet.IsLocked(KEY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryAcquire()
        {
            return _singleMutexSet.TryAcquire(KEY);
        }

        /// <summary>
        /// 기다려서 얻기
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        public async Task<bool> TryAcquireAfterWait(
            CancellationTokenSource? cancellationTokenSource = null)
        {
            return await _singleMutexSet.TryAcquireAfterWait(KEY, cancellationTokenSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
            _singleMutexSet.Release(KEY);
        }
    }
}

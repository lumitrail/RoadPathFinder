namespace RoadPathFinder.Utils
{
    /// <summary>
    /// 1. 읽기는 읽기를 막지 않음<br></br>
    /// 2. 읽기는 쓰기를 막음<br></br>
    /// 3. 쓰기는 읽기를 막음(세부 내용 필요, 이미 있는 읽기는 종료를 기다리며 추가 읽기는 막음-2때문)<br></br>
    /// 4. 쓰기는 쓰기를 막음<br></br>
    /// </summary>
    /// <typeparam name="TResourceID">resource id</typeparam>
    public class RwMutexSet<TResourceID>
        where TResourceID : notnull
    {
        /// <inheritdoc cref="CustomMutexSet{TResourceID}.PollingIntervalMs"/>
        public int PollingIntervalMs
        {
            get => _writes.PollingIntervalMs;
            set
            {
                _writes.PollingIntervalMs = value;
                _reads.PollingIntervalMs = value;
                //_noMoreReads.PollingIntervalMs = value;
            }
        }

        public int WaitTimeoutMs
        {
            get => _waitTimeoutMs;
            set => _waitTimeoutMs = Math.Min(Math.Max(1, value), 300000);
        }
        private int _waitTimeoutMs = 10000;

        private CustomMutexSet<TResourceID> _reads { get; } = new();
        //private CustomMutexSet<TResourceID> _noMoreReads { get; } = new();
        private CustomMutexSet<TResourceID> _writes { get; } = new();


        public async Task<double> WaitReading()
        {

        }

        public async Task<bool> TryAcquireReadingAfterWait(
            TResourceID id,
            CancellationTokenSource? cancellationTokenSource = null)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            

        }


        public bool TryAcquireReading(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

        }
    }
}

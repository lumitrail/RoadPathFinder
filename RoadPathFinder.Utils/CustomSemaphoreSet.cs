namespace RoadPathFinder.Utils
{
    public class CustomSemaphoreSet<TResourceID> : PollingCommons
        where TResourceID : notnull
    {
        /// <summary>최대 진입 허용 수, 1 이상</summary>
        public int MaxAllowed
        {
            get => _maxAllowed;
            set => _maxAllowed = Math.Max(1, value);
        }
        private int _maxAllowed;


        private System.Collections.Concurrent
            .ConcurrentDictionary<TResourceID, SemaphoreElem> _insiders { get; } = new();


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
                while (_insiders.TryGetValue(id, out SemaphoreElem? v))
                {
                    if (v.Count < MaxAllowed
                        || IsTimeout(startTime))
                    {
                        break;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }
            else
            {
                while (_insiders.TryGetValue(id, out SemaphoreElem? v))
                {
                    if (v.Count < MaxAllowed
                        || cancellationTokenSource.IsCancellationRequested
                        || IsTimeout(startTime))
                    {
                        break;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }

            return GetElapsedTimeMs(startTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool IsUsed(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            return !_insiders.TryGetValue(id, out SemaphoreElem? e)
                || e.Count == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool IsFull(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            if (_insiders.TryGetValue(id, out SemaphoreElem? v))
            {
                return v.Count >= MaxAllowed;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 한번 얻어 보기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryAcquire(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));

            if (_insiders.TryGetValue(id, out SemaphoreElem? v))
            {
                return v.TryUp(MaxAllowed);
            }
            else
            {
                return _insiders.TryAdd(id, new SemaphoreElem(1));
            }
        }

        /// <summary>
        /// 
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

            var e = new SemaphoreElem(1);

            if (cancellationTokenSource == null)
            {
                while (true)
                {
                    if (_insiders.TryAdd(id, e))
                    {
                        return true;
                    }
                    else if (_insiders.TryGetValue(id, out SemaphoreElem? v)
                        && v.TryUp(MaxAllowed))
                    {
                        return true;
                    }
                    else if (IsTimeout(startTime))
                    {
                        return false;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }
            else
            {
                while (true)
                {
                    if (_insiders.TryAdd(id, e))
                    {
                        return true;
                    }
                    else if (_insiders.TryGetValue(id, out SemaphoreElem? v)
                        && v.TryUp(MaxAllowed))
                    {
                        return true;
                    }
                    else if (IsTimeout(startTime))
                    {
                        return false;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryRelease(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            if (_insiders.TryGetValue(id, out SemaphoreElem? v))
            {
                bool downOk = v.TryDown();

                if (v.Count == 0
                    && _insiders.TryRemove(id, out _))
                {
                    return true;
                }
                else if (downOk)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceReleaseAll()
        {
            _insiders.Clear();
        }


        private class SemaphoreElem
        {
            public object LockObj { get; } = new();

            public int Count { get; private set; }


            public SemaphoreElem()
            {
                Count = 0;
            }

            public SemaphoreElem(int initCount)
            {
                Count = initCount;
            }

            public bool TryUp(int max)
            {
                lock (LockObj)
                {
                    if (Count >= max)
                    {
                        return false;
                    }
                    else
                    {
                        ++Count;
                        return true;
                    }
                }
            }

            public bool TryDown()
            {
                lock (LockObj)
                {
                    if (Count <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        --Count;
                        return true;
                    }
                }
            }
        }
    }
}

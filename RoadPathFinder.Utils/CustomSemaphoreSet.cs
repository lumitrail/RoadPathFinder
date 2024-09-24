using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                while (_insiders.TryGetValue(id, out SemaphoreElem v))
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
                while (_insiders.TryGetValue(id, out SemaphoreElem v))
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
        public bool IsFull(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            if (_insiders.TryGetValue(id, out SemaphoreElem v))
            {
                return v.Count >= MaxAllowed;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryAcquire(TResourceID id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            if (_insiders.TryAdd(id, new SemaphoreElem(1)))
            {
                return true;
            }
            else if (_insiders.TryGetValue(id, out SemaphoreElem v))
            {
                return SemaphoreElem
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

            if (cancellationTokenSource == null)
            {
                while (_insiders.TryGetValue(id, out SemaphoreElem e))
                {
                    if (e.Count < MaxAllowed)
                    {
                        e.Up();
                        return true;
                    }
                    if (IsTimeout(startTime))
                    {
                        return false;
                    }
                    await Task.Delay(PollingIntervalMs);
                }
                return _insiders.TryAdd(id, new SemaphoreElem(1));
            }
            else
            {
                while (_insiders.TryGetValue(id, out SemaphoreElem e))
            }
        }

        public bool TryIn(TResourceID id)
        {

        }


        private struct SemaphoreElem
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

            }

            public int Up()
            {
                lock (LockObj)
                {
                    ++Count;
                    return Count;
                }
            }

            public int Down()
            {
                lock (LockObj)
                {
                    --Count;
                    return Count;
                }
            }
        }
    }
}

using System.Collections.Concurrent;

namespace RoadPathFinder.Models
{
    public class Report
    {
        public string Title { get; }

        public ConcurrentBag<ReportMessage> Reports { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportTitle"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Report(string reportTitle)
        {
            ArgumentNullException.ThrowIfNull(reportTitle, nameof(reportTitle));
            Title = reportTitle;

            Reports = new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Report Merge(Report b)
        {
            ArgumentNullException.ThrowIfNull(b, nameof(b));
            var result = new Report(b.Title);

            result.Reports = Merge(Reports, b.Reports);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Add(ReportMessage message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            Reports.Add(message);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">message is empty</exception>
        /// <exception cref="ArgumentNullException">message is null</exception>
        public bool Add(ReportMessageType type, string message)
        {
            return Add(new ReportMessage(type, message));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static ConcurrentBag<ReportMessage> Merge(params IEnumerable<ReportMessage>[] b)
        {
            if (b == null
                || b.Length == 0)
            {
                return new();
            }
            return Merge(b.AsEnumerable());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private static ConcurrentBag<ReportMessage> Merge(IEnumerable<IEnumerable<ReportMessage>> ss)
        {
            if (ss == null
                || !ss.Any())
            {
                return [];
            }
            else
            {
                var allMessages = ss.Where(s => s != null)
                    .Where(s => s.Any())
                    .SelectMany(s => s);

                var result = new ConcurrentBag<ReportMessage>(allMessages);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ReportMessage
        {
            public DateTime TimestampUTC { get; } = DateTime.UtcNow;
            public ReportMessageType ReportMessageType { get; }
            public string Message { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reportMessageType"></param>
            /// <param name="message"></param>
            /// <exception cref="ArgumentException">message is empty</exception>
            /// <exception cref="ArgumentNullException">message is null</exception>
            public ReportMessage(
                ReportMessageType reportMessageType,
                string message)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(message, nameof(message));
                ReportMessageType = reportMessageType;
                Message = message;
            }

            /// <inheritdoc cref="ReportMessage.ReportMessage(ReportMessageType, string)"/>
            /// <param name="reportMessageType"></param>
            /// <param name="message"></param>
            /// <param name="timestampUTC"></param>
            public ReportMessage(
                ReportMessageType reportMessageType,
                string message,
                DateTime timestampUTC)
                : this(reportMessageType, message)
            {
                TimestampUTC = timestampUTC;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ReportMessageType
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
        };
    }
}

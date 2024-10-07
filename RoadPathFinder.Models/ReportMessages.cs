namespace RoadPathFinder.Models
{
    public class ReportMessages
    {
        public List<string> Normal { get; private set; } = new();
        public List<string> Warning { get; private set; } = new();
        public List<string> Error { get; private set; } = new();
        public List<string> Fatal { get; private set; } = new();


        public ReportMessages Merge(ReportMessages b)
        {
            var result = new ReportMessages();

            result.Normal = Merge(Normal, b.Normal);
            result.Warning = Merge(Warning, b.Warning);
            result.Error = Merge(Error, b.Error);
            result.Fatal = Merge(Fatal, b.Fatal);

            return result;
        }


        private static List<string> Merge(params IEnumerable<string>[] b)
        {
            return Merge(b.AsEnumerable());
        }

        private static List<string> Merge(IEnumerable<IEnumerable<string>> ss)
        {
            if (ss == null
                || !ss.Any())
            {
                return [];
            }
            else
            {
                return ss
                .Where(s => s != null)
                .Where(s => s.Any())
                .SelectMany(s => s)
                .ToList();
            }
        }
    }
}

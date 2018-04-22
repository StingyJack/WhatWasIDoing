namespace ShelfsetChangeReporter
{
    public class Difference
    {
        public string Path { get; set; }
        public int LineNumber { get; set; }
        public string OlderLine { get; set; }
        public string NewerLine { get; set; }
        public DiffType DiffType { get; set; }
        public string Set { get; set; }
        public string Message { get; set; }
    }
}
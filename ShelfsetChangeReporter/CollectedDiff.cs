namespace ShelfsetChangeReporter
{
    using System.Collections.Generic;

    public class CollectedDiff
    {
        public ShelfSet ShelfSetA { get; set; }
        public ShelfSet ShelfSetB { get; set; }
        public List<Difference> Diffs { get; set; }
        public string ProjectCollectionName { get; set; }
    }
}
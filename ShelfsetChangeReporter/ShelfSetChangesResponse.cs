namespace ShelfsetChangeReporter
{
    using System.Collections.Generic;

    public class ShelfSetChangesResponse
    {
        public int Count { get; set; }
        public List<ShelfSetChange> Value { get; set; }
    }
}
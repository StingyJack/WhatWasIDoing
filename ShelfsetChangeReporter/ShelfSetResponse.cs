namespace ShelfsetChangeReporter
{
    using System.Collections.Generic;

    public class ShelfSetResponse
    {
        public string Name { get; set; }
        public List<ShelfSet> Value { get; set; }
    }
}
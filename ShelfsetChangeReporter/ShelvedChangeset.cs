namespace ShelfsetChangeReporter
{
    using System.Collections.Generic;

    public class ShelvedChangeset
    {
        public string ProjectCollectionName { get; set; }
        public ShelfSet ShelfSet { get; set; }
        public List<ShelfSetChange> Changes { get; set; }
    }
}
namespace ShelfsetChangeReporter
{
    using System;

    public class ShelfSet
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Owner Owner { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Url { get; set; }
    }
}
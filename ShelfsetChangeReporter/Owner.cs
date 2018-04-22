namespace ShelfsetChangeReporter
{
    using System;

    public class Owner
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string UniqueName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}
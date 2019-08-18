using System;

namespace EclassApi.Models
{
    public class Announcement
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public string Description { get; set; }
        public string DatePublished { get; set; }
        public Announcement() { }
        public Announcement(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
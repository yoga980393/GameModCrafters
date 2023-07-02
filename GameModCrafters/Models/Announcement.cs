using System;

namespace GameModCrafters.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime AddTime { get; set; }

        // Add these lines
        public string UserEmail { get; set; }
        public User Publisher { get; set; }
    }
}

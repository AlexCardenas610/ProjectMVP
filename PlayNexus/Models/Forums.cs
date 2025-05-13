using System.ComponentModel.DataAnnotations.Schema;

namespace PlayNexus.Models {
    public class Forums {
        public int Id { get; set; }
        public string? Topic { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ForumReply> Replies { get; set; } = new List<ForumReply>();
        // Forum posts should use the ForumPost class for extensibility.
    }

    public class ForumReply {
        public int Id { get; set; } // Primary key for EF Core
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ForumPost {
        public int Id { get; set; }
        public string? Topic { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ForumReply> Replies { get; set; } = new List<ForumReply>();
    }
}
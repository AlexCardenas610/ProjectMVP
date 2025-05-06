using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayNexus.Models {
    public class Forums {
        public int Id { get; set; }
        public string Topic { get; set; }

        // Store Posts as a JSON string in the database
        public string PostsJson { get; set; }

        [NotMapped]
        public List<string> Posts {
            get => string.IsNullOrEmpty(PostsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(PostsJson);
            set => PostsJson = JsonSerializer.Serialize(value);
        }

        public void CreatePost(string postContent) {
            Posts.Add(postContent);
        }
    }
}
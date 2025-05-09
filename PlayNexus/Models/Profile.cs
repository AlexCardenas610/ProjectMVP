using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayNexus.Models {
    public class Profile {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        public string GamingInterests { get; set; }

        // Store Friends as a JSON string in the database
        public string FriendsJson { get; set; }

        [NotMapped]
        public List<int> Friends {
            get => string.IsNullOrEmpty(FriendsJson) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(FriendsJson);
            set => FriendsJson = JsonSerializer.Serialize(value);
        }

        public void UpdateProfile(string newUsername, string newEmail) {
            // Logic to update profile details
        }
    }
}
namespace PlayNexus.Models {
    public class UserSignInLog {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; } // Foreign key to the User table
        public DateTime SignInTimestamp { get; set; } // Timestamp of the sign-in
        public string IpAddress { get; set; } // IP address of the user

        // Navigation property
        public User User { get; set; }
    }
}
namespace PlayNexus.Models {
    public class Highlights {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FileName { get; set; }
        public string? UserName { get; set; } // Add this property

        public void UploadVideo(string filePath) {
            FileName = Path.GetFileName(filePath);
            // Logic to upload video file
        }
    }
}
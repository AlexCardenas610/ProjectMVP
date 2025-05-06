namespace PlayNexus.Models {
    public class News {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public DateTime PublishedAt { get; set; }

        public void PublishNews(string headline, string content) {
            // Logic to publish news articles
        }
    }
}
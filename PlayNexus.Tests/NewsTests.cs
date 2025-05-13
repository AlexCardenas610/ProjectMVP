using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class NewsTests
    {
        private readonly PlayNexusDbContext _dbContext;

        public NewsTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("NewsTestDb"));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
        }

        [Fact]
        public async Task AddNews_Succeeds()
        {
            var news = new News
            {
                Headline = "Test News",
                Content = "This is a test news content."
            };

            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();

            var savedNews = await _dbContext.News.FirstOrDefaultAsync(n => n.Headline == "Test News");
            Assert.NotNull(savedNews);
            Assert.Equal("This is a test news content.", savedNews.Content);
        }

        [Fact]
        public async Task UpdateNews_Succeeds()
        {
            var news = new News
            {
                Headline = "Update Test",
                Content = "Initial content"
            };

            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();

            news.Content = "Updated content";
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();

            var updatedNews = await _dbContext.News.FirstOrDefaultAsync(n => n.Headline == "Update Test");
            Assert.NotNull(updatedNews);
            Assert.Equal("Updated content", updatedNews.Content);
        }

        [Fact]
        public async Task DeleteNews_Succeeds()
        {
            var news = new News
            {
                Headline = "Delete Test",
                Content = "To be deleted"
            };

            _dbContext.News.Add(news);
            await _dbContext.SaveChangesAsync();

            _dbContext.News.Remove(news);
            await _dbContext.SaveChangesAsync();

            var deletedNews = await _dbContext.News.FirstOrDefaultAsync(n => n.Headline == "Delete Test");
            Assert.Null(deletedNews);
        }
    }
}

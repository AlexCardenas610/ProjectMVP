using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class ForumsTests
    {
        private readonly PlayNexusDbContext _dbContext;

        public ForumsTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("ForumsTestDb"));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
        }

        [Fact]
        public async Task AddForum_Succeeds()
        {
            var forum = new Forums
            {
                Topic = "Test Forum",
                PostsJson = "[]"
            };

            _dbContext.Forums.Add(forum);
            await _dbContext.SaveChangesAsync();

            var savedForum = await _dbContext.Forums.FirstOrDefaultAsync(f => f.Topic == "Test Forum");
            Assert.NotNull(savedForum);
            Assert.Equal("[]", savedForum.PostsJson);
        }

        [Fact]
        public async Task UpdateForum_Succeeds()
        {
            var forum = new Forums
            {
                Topic = "Update Test",
                PostsJson = "[]"
            };

            _dbContext.Forums.Add(forum);
            await _dbContext.SaveChangesAsync();

            forum.PostsJson = "[{\"post\":\"Updated post\"}]";
            _dbContext.Forums.Update(forum);
            await _dbContext.SaveChangesAsync();

            var updatedForum = await _dbContext.Forums.FirstOrDefaultAsync(f => f.Topic == "Update Test");
            Assert.NotNull(updatedForum);
            Assert.Equal("[{\"post\":\"Updated post\"}]", updatedForum.PostsJson);
        }

        [Fact]
        public async Task DeleteForum_Succeeds()
        {
            var forum = new Forums
            {
                Topic = "Delete Test",
                PostsJson = "[]"
            };

            _dbContext.Forums.Add(forum);
            await _dbContext.SaveChangesAsync();

            _dbContext.Forums.Remove(forum);
            await _dbContext.SaveChangesAsync();

            var deletedForum = await _dbContext.Forums.FirstOrDefaultAsync(f => f.Topic == "Delete Test");
            Assert.Null(deletedForum);
        }
    }
}

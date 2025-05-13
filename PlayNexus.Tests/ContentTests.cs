using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class ContentTests
    {
        private readonly PlayNexusDbContext _dbContext;

        public ContentTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("ContentTestDb"));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
        }

        [Fact]
        public async Task AddContent_Succeeds()
        {
            var content = new Content
            {
                Title = "Test Content",
                Description = "This is a test content description.",
                FileName = "testfile.txt"
            };

            _dbContext.Contents.Add(content);
            await _dbContext.SaveChangesAsync();

            var savedContent = await _dbContext.Contents.FirstOrDefaultAsync(c => c.Title == "Test Content");
            Assert.NotNull(savedContent);
            Assert.Equal("testfile.txt", savedContent.FileName);
        }

        [Fact]
        public async Task UpdateContent_Succeeds()
        {
            var content = new Content
            {
                Title = "Update Test",
                Description = "Initial description",
                FileName = "initial.txt"
            };

            _dbContext.Contents.Add(content);
            await _dbContext.SaveChangesAsync();

            content.Description = "Updated description";
            _dbContext.Contents.Update(content);
            await _dbContext.SaveChangesAsync();

            var updatedContent = await _dbContext.Contents.FirstOrDefaultAsync(c => c.Title == "Update Test");
            Assert.NotNull(updatedContent);
            Assert.Equal("Updated description", updatedContent.Description);
        }

        [Fact]
        public async Task DeleteContent_Succeeds()
        {
            var content = new Content
            {
                Title = "Delete Test",
                Description = "To be deleted",
                FileName = "delete.txt"
            };

            _dbContext.Contents.Add(content);
            await _dbContext.SaveChangesAsync();

            _dbContext.Contents.Remove(content);
            await _dbContext.SaveChangesAsync();

            var deletedContent = await _dbContext.Contents.FirstOrDefaultAsync(c => c.Title == "Delete Test");
            Assert.Null(deletedContent);
        }
    }
}

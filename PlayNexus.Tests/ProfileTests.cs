using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class ProfileTests
    {
        private readonly PlayNexusDbContext _dbContext;

        public ProfileTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("ProfileTestDb"));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
        }

        [Fact]
        public async Task AddProfile_Succeeds()
        {
            var profile = new Profile
            {
                UserId = "user1",
                Username = "testuser",
                Email = "testuser@example.com",
                Biography = "Test biography",
                GamingInterests = "Gaming",
                FriendsJson = "[]"
            };

            _dbContext.Profiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            var savedProfile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Username == "testuser");
            Assert.NotNull(savedProfile);
            Assert.Equal("testuser@example.com", savedProfile.Email);
        }

        [Fact]
        public async Task UpdateProfile_Succeeds()
        {
            var profile = new Profile
            {
                UserId = "user2",
                Username = "updateuser",
                Email = "updateuser@example.com",
                Biography = "Initial biography",
                GamingInterests = "Gaming",
                FriendsJson = "[]"
            };

            _dbContext.Profiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            profile.Biography = "Updated biography";
            _dbContext.Profiles.Update(profile);
            await _dbContext.SaveChangesAsync();

            var updatedProfile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Username == "updateuser");
            Assert.NotNull(updatedProfile);
            Assert.Equal("Updated biography", updatedProfile.Biography);
        }

        [Fact]
        public async Task DeleteProfile_Succeeds()
        {
            var profile = new Profile
            {
                UserId = "user3",
                Username = "deleteuser",
                Email = "deleteuser@example.com",
                Biography = "To be deleted",
                GamingInterests = "Gaming",
                FriendsJson = "[]"
            };

            _dbContext.Profiles.Add(profile);
            await _dbContext.SaveChangesAsync();

            _dbContext.Profiles.Remove(profile);
            await _dbContext.SaveChangesAsync();

            var deletedProfile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Username == "deleteuser");
            Assert.Null(deletedProfile);
        }
    }
}

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class AdminUsersTests
    {
        private readonly PlayNexusDbContext _dbContext;

        public AdminUsersTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("AdminUsersTestDb"));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
        }

        [Fact]
        public async Task AddUser_Succeeds()
        {
            var user = new User
            {
                UserName = "adminuser",
                Email = "adminuser@example.com"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var savedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "adminuser");
            Assert.NotNull(savedUser);
            Assert.Equal("adminuser@example.com", savedUser.Email);
        }

        [Fact]
        public async Task UpdateUser_Succeeds()
        {
            var user = new User
            {
                UserName = "updateadmin",
                Email = "updateadmin@example.com"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            user.Email = "updatedadmin@example.com";
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "updateadmin");
            Assert.NotNull(updatedUser);
            Assert.Equal("updatedadmin@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task DeleteUser_Succeeds()
        {
            var user = new User
            {
                UserName = "deleteadmin",
                Email = "deleteadmin@example.com"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            var deletedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "deleteadmin");
            Assert.Null(deletedUser);
        }
    }
}

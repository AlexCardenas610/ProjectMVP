using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PlayNexus.Models;
using Xunit;

namespace PlayNexus.Tests
{
    public class AccountTests
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly PlayNexusDbContext _dbContext;

        public AccountTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PlayNexusDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddLogging();

services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<PlayNexusDbContext>()
    .AddDefaultTokenProviders();

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetRequiredService<PlayNexusDbContext>();
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
            var contextAccessor = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userClaimsPrincipalFactory = serviceProvider.GetRequiredService<IUserClaimsPrincipalFactory<User>>();
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var logger = serviceProvider.GetRequiredService<ILogger<SignInManager<User>>>();
            var schemes = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            var confirmation = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.IUserConfirmation<User>>();

            _signInManager = new SignInManager<User>(_userManager, contextAccessor, userClaimsPrincipalFactory, options, logger, schemes, confirmation);
        }

        [Fact]
        public async Task RegisterUser_PasswordIsHashed()
        {
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var password = "TestPassword123!";

            var result = await _userManager.CreateAsync(user, password);

            Assert.True(result.Succeeded);

            var storedUser = await _userManager.FindByNameAsync("testuser");
            Assert.NotNull(storedUser);

            // Password should not be stored in plain text
            Assert.NotEqual(password, storedUser.PasswordHash);

            // Password hash should be set
            Assert.False(string.IsNullOrEmpty(storedUser.PasswordHash));
        }

        [Fact]
        public async Task SignInUser_ValidPassword_Succeeds()
        {
            var user = new User { UserName = "signinuser", Email = "signinuser@example.com" };
            var password = "SignInPassword123!";

            var createResult = await _userManager.CreateAsync(user, password);
            Assert.True(createResult.Succeeded);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            Assert.True(signInResult.Succeeded);
        }

        [Fact]
        public async Task SignInUser_InvalidPassword_Fails()
        {
            var user = new User { UserName = "signinuser2", Email = "signinuser2@example.com" };
            var password = "CorrectPassword123!";
            var wrongPassword = "WrongPassword123!";

            var createResult = await _userManager.CreateAsync(user, password);
            Assert.True(createResult.Succeeded);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, wrongPassword, false);
            Assert.False(signInResult.Succeeded);
        }
        [Fact]
        public async Task RegisterUser_EmptyPassword_Fails()
        {
            var user = new User { UserName = "emptyuser", Email = "emptyuser@example.com" };
            var password = "";

            var result = await _userManager.CreateAsync(user, password);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUser_LongPassword_Succeeds()
        {
            var user = new User { UserName = "longuser", Email = "longuser@example.com" };
            var password = new string('a', 256); // 256 characters long

            var result = await _userManager.CreateAsync(user, password);

            Assert.True(result.Succeeded);

            var storedUser = await _userManager.FindByNameAsync("longuser");
            Assert.NotNull(storedUser);
            Assert.False(string.IsNullOrEmpty(storedUser.PasswordHash));
        }

        [Fact]
        public async Task RegisterUser_SpecialCharacterPassword_Succeeds()
        {
            var user = new User { UserName = "specialuser", Email = "specialuser@example.com" };
            var password = "!@#$%^&*()_+-=[]{}|;':,.<>/?";

            var result = await _userManager.CreateAsync(user, password);

            Assert.True(result.Succeeded);

            var storedUser = await _userManager.FindByNameAsync("specialuser");
            Assert.NotNull(storedUser);
            Assert.False(string.IsNullOrEmpty(storedUser.PasswordHash));
        }
    }
}

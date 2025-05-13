using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PlayNexus.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PlayNexus.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state.");
                return Page();
            }

            // Normalize email to lowercase during login
            Email = Email.ToLowerInvariant();

            // Ensure email-password association is validated
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                _logger.LogInformation("Email not found.");
                ViewData["ErrorMessage"] = "Email not found.";
                return Page();
            }

            // Use ASP.NET Core's PasswordHasher to verify the password
            var passwordHasher = new PasswordHasher<User>();
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Password is not set for this user.");
                ViewData["ErrorMessage"] = "Password is not set for this user.";
                return Page();
            }
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Password is incorrect.");
                ViewData["ErrorMessage"] = "Password is incorrect.";
                return Page();
            }

            // Use UserName for sign-in (not Email)
            var signInResult = await _signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, Password, isPersistent: false, lockoutOnFailure: false);
            _logger.LogInformation($"SignIn result: {signInResult.Succeeded}, IsLockedOut: {signInResult.IsLockedOut}, IsNotAllowed: {signInResult.IsNotAllowed}, RequiresTwoFactor: {signInResult.RequiresTwoFactor}");

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User logged in successfully.");
                ViewData["SuccessMessage"] = "You have successfully logged in!";
                return RedirectToPage("/Profile/Index");
            }
            else
            {
                _logger.LogWarning("Login failed.");
                ViewData["ErrorMessage"] = "Password is incorrect.";
                return Page();
            }
        }
    }
}

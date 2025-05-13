using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PlayNexus.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayNexus.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        [Required]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""':{}|<>]).{6,}$", ErrorMessage = "Password must be at least 6 characters and contain a special character.")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public RegisterModel(UserManager<User> userManager, ILogger<RegisterModel> logger, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        ViewData["ErrorMessage"] += error.ErrorMessage + " ";
                    }
                }
                return Page();
            }

            // Check if email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(Email);
            if (existingUserByEmail != null)
            {
                ViewData["ErrorMessage"] += "Email already exists. ";
            }

            // Check if username already exists
            var existingUserByUsername = await _userManager.FindByNameAsync(Username);
            if (existingUserByUsername != null)
            {
                ViewData["ErrorMessage"] += "Username already exists. ";
            }

            if (!string.IsNullOrEmpty(ViewData["ErrorMessage"] as string))
            {
                return Page();
            }

            // Hash the password before storing
            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                UserName = Username,
                Email = Email
            };
            user.PasswordHash = passwordHasher.HashPassword(user, Password); // Securely hash the password

            var result = await _userManager.CreateAsync(user);

            if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
            {
                ViewData["ErrorMessage"] = "Username already exists.";
                return Page();
            }
            if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
            {
                ViewData["ErrorMessage"] = "Email already exists.";
                return Page();
            }

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account successfully.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                ViewData["SuccessMessage"] = "Account created successfully! Redirecting to home...";
                return Page();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}

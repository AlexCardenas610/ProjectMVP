using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;

namespace PlayNexus.Pages.Account {
    public class RegisterModel : PageModel {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid || Password != ConfirmPassword) {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var user = new User { UserName = Username, Email = Email };
            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded) {
                ViewData["SuccessMessage"] = "You have successfully signed up!";
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            } else if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail")) {
                ViewData["ErrorMessage"] = "An account with this email already exists.";
                return Page();
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;

namespace PlayNexus.Pages.Account {
    public class LoginModel : PageModel {
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public LoginModel(SignInManager<User> signInManager) {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded) {
                    ViewData["SuccessMessage"] = "You have successfully logged in!";
                    return RedirectToPage("/Profile/Index");
                } else {
                    ViewData["ErrorMessage"] = "Email or password is incorrect.";
                    return Page();
                }
        }
    }
}

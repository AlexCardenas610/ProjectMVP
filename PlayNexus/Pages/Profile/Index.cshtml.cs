using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlayNexus.Pages.Profile {
    [Authorize]
    public class IndexModel : PageModel {
        private readonly PlayNexusDbContext _context;

        public User CurrentUser { get; set; }
        public PlayNexus.Models.Profile UserProfile { get; set; }
        public List<User> Friends { get; set; }
        public List<string> ViewingActivity { get; set; } = new List<string>();

        [BindProperty]
        public string Biography { get; set; }

        [BindProperty]
        public string GamingInterests { get; set; }

        [BindProperty]
        public string FriendSearchName { get; set; }

        public IndexModel(PlayNexusDbContext context) {
            _context = context;
        }

        public void OnGet() {
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (CurrentUser != null) {
                UserProfile = _context.Profiles.FirstOrDefault(p => p.Username == CurrentUser.UserName) ?? new PlayNexus.Models.Profile();
            } else {
                UserProfile = new PlayNexus.Models.Profile();
            }
            Friends = new List<User>(); // Implement friend retrieval logic as needed
            ViewingActivity = new List<string>(); // Implement viewing activity retrieval as needed

            Biography = ""; // No Biography property in Profile model
            GamingInterests = ""; // No GamingInterests property in Profile model
        }

        public IActionResult OnPostUpdateProfile() {
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (CurrentUser == null) {
                return RedirectToPage();
            }
            UserProfile = _context.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id);

            if (UserProfile == null) {
                UserProfile = new PlayNexus.Models.Profile {
                    UserId = CurrentUser.Id,
                    Biography = Biography,
                    GamingInterests = GamingInterests
                };
                _context.Profiles.Add(UserProfile);
            } else {
                UserProfile.Biography = Biography;
                UserProfile.GamingInterests = GamingInterests;
                _context.Profiles.Update(UserProfile);
            }

            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostAddFriend() {
            if (string.IsNullOrEmpty(FriendSearchName)) {
                ModelState.AddModelError(string.Empty, "Please enter a friend name to search.");
                return Page();
            }

            var friend = _context.Users.FirstOrDefault(u => u.UserName == FriendSearchName);
            if (friend == null) {
                ModelState.AddModelError(string.Empty, "Friend not found.");
                return Page();
            }

            // Implement friend adding logic here (e.g., add to a friends list)

            return RedirectToPage();
        }
    }
}

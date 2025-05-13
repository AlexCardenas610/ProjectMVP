using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlayNexus.Pages.Account {
    [Authorize]
    public class IndexModel : PageModel {
        private readonly PlayNexusDbContext _context;
        private readonly IWebHostEnvironment _env;

        public User CurrentUser { get; set; }
        public PlayNexus.Models.Profile UserProfile { get; set; }
        public List<User> Friends { get; set; }
        public List<string> ViewingActivity { get; set; } = new List<string>();
        public string ProfilePictureUrl { get; set; }

        [BindProperty]
        public string Biography { get; set; }

        [BindProperty]
        public string GamingInterests { get; set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        public IndexModel(PlayNexusDbContext context, IWebHostEnvironment env) {
            _context = context;
            _env = env;
        }

        public void OnGet() {
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (CurrentUser != null) {
                UserProfile = _context.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id) ?? new PlayNexus.Models.Profile();
                Biography = UserProfile.Biography;
                GamingInterests = UserProfile.GamingInterests;
                ProfilePictureUrl = UserProfile.ProfilePictureUrl;
            } else {
                UserProfile = new PlayNexus.Models.Profile();
            }
            Friends = new List<User>(); // Implement friend retrieval logic as needed
            ViewingActivity = new List<string>(); // Implement viewing activity retrieval as needed
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
            TempData["ProfileSaved"] = true;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUploadPictureAsync() {
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (CurrentUser == null || ProfilePicture == null) {
                return RedirectToPage();
            }
            UserProfile = _context.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id);
            if (UserProfile == null) {
                UserProfile = new PlayNexus.Models.Profile { UserId = CurrentUser.Id };
                _context.Profiles.Add(UserProfile);
                await _context.SaveChangesAsync(); // Save to get a valid Id
            }
            var uploadsFolder = Path.Combine(_env.WebRootPath, "profilepics");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{CurrentUser.Id}_{Path.GetFileName(ProfilePicture.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await ProfilePicture.CopyToAsync(stream);
            }
            UserProfile.ProfilePictureUrl = $"/profilepics/{fileName}";
            if (UserProfile.Id != 0) {
                _context.Profiles.Update(UserProfile);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}

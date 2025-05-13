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

        public User? CurrentUser { get; set; }
        public PlayNexus.Models.Profile? UserProfile { get; set; }
        public List<User> Friends { get; set; } = new List<User>();
        public List<string> ViewingActivity { get; set; } = new List<string>();
        public string? ProfilePictureUrl { get; set; }

        [BindProperty]
        public string? Biography { get; set; }

        [BindProperty]
        public string? GamingInterests { get; set; }

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }

        public IndexModel(PlayNexusDbContext context, IWebHostEnvironment env) {
            _context = context;
            _env = env;
        }

        public void OnGet() {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) {
                UserProfile = new PlayNexus.Models.Profile();
                Friends = new List<User>();
                ViewingActivity = new List<string>();
                return;
            }
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (CurrentUser != null) {
                UserProfile = _context.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id) ?? new PlayNexus.Models.Profile();
                Biography = UserProfile.Biography;
                GamingInterests = UserProfile.GamingInterests;
                ProfilePictureUrl = UserProfile.ProfilePictureUrl;
                Friends = new List<User>(); // Implement friend retrieval logic as needed
                ViewingActivity = new List<string>();
                // Add forum post activity
                var userPosts = _context.Forums.Where(f => f.UserName == userName).ToList();
                foreach (var post in userPosts)
                {
                    ViewingActivity.Add($"Created forum topic: '{post.Topic}'");
                }
                // Add video upload activity
                var userVideos = _context.Contents.Where(c => c.UserName == userName).ToList();
                foreach (var video in userVideos)
                {
                    ViewingActivity.Add($"Uploaded video: '{video.Title}'");
                }
            } else {
                UserProfile = new PlayNexus.Models.Profile();
                Friends = new List<User>();
                ViewingActivity = new List<string>();
            }
        }

        public IActionResult OnPostUpdateProfile() {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToPage();
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
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
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName) || ProfilePicture == null) {
                return RedirectToPage();
            }
            CurrentUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (CurrentUser == null) {
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

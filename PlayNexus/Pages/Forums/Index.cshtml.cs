using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.ComponentModel.DataAnnotations;

namespace PlayNexus.Pages.Forums {
    [Authorize]
    public class IndexModel : PageModel {
        private readonly PlayNexusDbContext _context;

        public IndexModel(PlayNexusDbContext context) {
            _context = context;
        }

        [BindProperty]
        [Required]
        public string? Topic { get; set; }

        [BindProperty]
        [Required]
        public string? PostContent { get; set; }

        public List<PlayNexus.Models.Forums>? Posts { get; set; }

        public List<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

        public void OnGet() {
            Posts = _context.Forums.OrderByDescending(p => p.Id).ToList();
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                Posts = _context.Forums.OrderByDescending(p => p.Id).ToList();
                return Page();
            }

            var forum = _context.Forums.FirstOrDefault(f => f.Topic == Topic);
            if (forum == null) {
                forum = new PlayNexus.Models.Forums {
                    Topic = Topic ?? string.Empty,
                    UserName = User.Identity?.Name,
                    Content = PostContent ?? string.Empty,
                    CreatedAt = DateTime.Now,
                    Replies = new List<ForumReply>()
                };
                _context.Forums.Add(forum);
            } else {
                // Optionally update content or ignore duplicate topic
            }
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostReply(int forumId, string ReplyContent) {
            var forum = _context.Forums.FirstOrDefault(f => f.Id == forumId);
            if (forum != null && User.Identity?.Name != forum.UserName) {
                forum.Replies.Add(new ForumReply {
                    UserName = User.Identity?.Name,
                    Content = ReplyContent,
                    CreatedAt = DateTime.Now
                });
                _context.Forums.Update(forum);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int forumId) {
            var forum = _context.Forums.FirstOrDefault(f => f.Id == forumId);
            if (forum != null && forum.UserName == User.Identity?.Name) {
                _context.Forums.Remove(forum);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}

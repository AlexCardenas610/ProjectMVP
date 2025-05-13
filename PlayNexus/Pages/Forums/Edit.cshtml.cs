using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Linq;

namespace PlayNexus.Pages.Forums {
    [Authorize]
    public class EditModel : PageModel {
        private readonly PlayNexusDbContext _context;
        public EditModel(PlayNexusDbContext context) {
            _context = context;
        }

        [BindProperty]
        public string? Topic { get; set; }
        [BindProperty]
        public string? PostContent { get; set; }
        [BindProperty(SupportsGet = true)]
        public int ForumId { get; set; }

        public IActionResult OnGet() {
            var userName = User?.Identity?.Name;
            var forum = _context.Forums.FirstOrDefault(f => f.Id == ForumId && f.UserName == userName);
            if (forum == null) return RedirectToPage("Index");
            Topic = forum.Topic;
            PostContent = forum.Content;
            return Page();
        }

        public IActionResult OnPost() {
            var userName = User?.Identity?.Name;
            var forum = _context.Forums.FirstOrDefault(f => f.Id == ForumId && f.UserName == userName);
            if (forum == null) return RedirectToPage("Index");
            forum.Topic = Topic;
            forum.Content = PostContent ?? string.Empty;
            _context.Forums.Update(forum);
            _context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}

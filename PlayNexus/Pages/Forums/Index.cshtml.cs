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
        public string Topic { get; set; }

        [BindProperty]
        [Required]
        public string PostContent { get; set; }

        public List<PlayNexus.Models.Forums> Posts { get; set; }

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
                    Topic = Topic,
                    Posts = new List<string> { PostContent }
                };
                _context.Forums.Add(forum);
            } else {
                forum.Posts.Add(PostContent);
                _context.Forums.Update(forum);
            }
            _context.SaveChanges();

            return RedirectToPage();
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlayNexus.Pages.News {
    public class IndexModel : PageModel {
        private readonly PlayNexusDbContext _context;

        public List<PlayNexus.Models.News> NewsArticles { get; set; }

        public IndexModel(PlayNexusDbContext context) {
            _context = context;
        }

        public void OnGet() {
            NewsArticles = _context.News.OrderByDescending(n => n.PublishedAt).ToList();
        }
    }
}

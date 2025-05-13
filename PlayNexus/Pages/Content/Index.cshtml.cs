using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlayNexus.Pages.Content {
    public class IndexModel : PageModel {
        private readonly PlayNexusDbContext _context;
        public List<Highlights> Highlights { get; set; } = new List<Highlights>();

        public IndexModel(PlayNexusDbContext context) {
            _context = context;
        }

        public void OnGet() {
            Highlights = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
        }
    }
}

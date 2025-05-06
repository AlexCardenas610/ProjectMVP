using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using HighlightsModel = PlayNexus.Models.Highlights;

namespace PlayNexus.Pages.Content {
    public class CreateModel : PageModel {
        private readonly PlayNexusDbContext _context;

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public IFormFile FilePath { get; set; }

        public CreateModel(PlayNexusDbContext context) {
            _context = context;
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var content = new HighlightsModel {
                Title = Title,
                Description = Description,
                CreatedAt = DateTime.Now
            };

            // Logic to handle file upload (simplified for now)
            if (FilePath != null) {
                var filePath = Path.Combine("wwwroot/uploads", FilePath.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    FilePath.CopyTo(stream);
                }
            }

            _context.Contents.Add(content);
            _context.SaveChanges();

            return RedirectToPage("/Index");
        }
    }
}
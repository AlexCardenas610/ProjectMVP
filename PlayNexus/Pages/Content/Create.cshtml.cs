using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using PlayNexus.Models;
using HighlightsModel = PlayNexus.Models.Highlights;

namespace PlayNexus.Pages.Content {
    [Authorize]
    [RequestSizeLimit(104857600)] // 100 MB
    public class CreateModel : PageModel {
        private readonly PlayNexusDbContext _context;

        [BindProperty]
        public string? Title { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public IFormFile? FilePath { get; set; }

        public CreateModel(PlayNexusDbContext context) {
            _context = context;
        }

        public IActionResult OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var content = new HighlightsModel {
                Title = Title ?? string.Empty,
                Description = Description ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            // Logic to handle file upload with 50MB size limit
            if (FilePath != null) {
                if (FilePath.Length > 50 * 1024 * 1024) {
                    ModelState.AddModelError(string.Empty, "File size must be 50MB or less.");
                    return Page();
                }
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
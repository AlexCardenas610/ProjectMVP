using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using PlayNexus.Models;
using HighlightsModel = PlayNexus.Models.Highlights;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PlayNexus.Pages.Content {
    [Authorize]
    [RequestSizeLimit(104857600)] // 100 MB
    public class CreateModel : PageModel {
        private readonly PlayNexusDbContext _context;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string? Title { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        [BindProperty]
        public IFormFile? FilePath { get; set; }

        public List<HighlightsModel> HighlightsList { get; set; } = new List<HighlightsModel>();

        public CreateModel(PlayNexusDbContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        public void OnGet() {
            HighlightsList = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                HighlightsList = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
                return Page();
            }

            var content = new HighlightsModel {
                Title = Title ?? string.Empty,
                Description = Description ?? string.Empty,
                CreatedAt = DateTime.Now,
                UserName = User.Identity?.Name // Set the uploader's username
            };

            if (FilePath != null) {
                if (FilePath.Length > 100 * 1024 * 1024) {
                    ModelState.AddModelError(string.Empty, "File size must be 100MB or less.");
                    HighlightsList = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
                    return Page();
                }
                // Store video locally in wwwroot/uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(FilePath.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await FilePath.CopyToAsync(stream);
                }
                content.FileName = "/uploads/" + uniqueFileName;
            }

            _context.Contents.Add(content);
            _context.SaveChanges();

            HighlightsList = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
            return Page();
        }
    }
}
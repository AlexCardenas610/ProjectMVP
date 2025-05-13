using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayNexus.Models;
using HighlightsModel = PlayNexus.Models.Highlights;

namespace PlayNexus.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly PlayNexusDbContext _context;

    public List<HighlightsModel> Clips { get; set; } = new List<HighlightsModel>();
    public List<HighlightsModel> Highlights { get; set; } = new List<HighlightsModel>();

    public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment, PlayNexusDbContext context)
    {
        _logger = logger;
        _environment = environment;
        _context = context;
    }

    public IActionResult OnGet()
    {
        try
        {
            Highlights = _context.Contents.OrderByDescending(c => c.CreatedAt).ToList();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving clips.");
            return StatusCode(500);
        }
    }
}

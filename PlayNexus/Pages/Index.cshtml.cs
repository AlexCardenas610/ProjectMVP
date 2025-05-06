using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HighlightsModel = PlayNexus.Models.Highlights;

namespace PlayNexus.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public List<HighlightsModel> Clips { get; set; } = new List<HighlightsModel>();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        // Example: Populate clips with sample data
        Clips.Add(new HighlightsModel { Id = 1, Title = "Sample Video", Description = "A sample video description", CreatedAt = DateTime.Now });
    }
}

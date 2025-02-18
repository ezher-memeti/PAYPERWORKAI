using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class DownloadModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Image1Url { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Image2Url { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Prompt { get; set; }

    public string VideoUrl { get; set; } = "/Assets/sample-video.mp4"; // Example video URL

    public void OnGet()
    {
        // Simulate that the video is not ready initially
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class DownloadModel : PageModel
{
    [BindProperty(Name = "image1Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image1Url { get; set; }

    [BindProperty(Name = "image2Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image2Url { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Prompt { get; set; }

    public string VideoUrl { get; set; } = "/Assets/sample-video.mp4";
public void OnGet()
{
    // Debug için ekstra kontroller
    if (string.IsNullOrEmpty(Image1Url) || string.IsNullOrEmpty(Image2Url))
    {
        // Hata durumunda loglama
        Console.WriteLine($"Missing images: {Image1Url ?? "null"}, {Image2Url ?? "null"}");
        // Kullanıcıya hata mesajı
        TempData["Error"] = "Image URLs are missing. Please upload images again.";
    }

    if (string.IsNullOrEmpty(Category))
    {
        // Kategori bilgisi eksikse hata mesajı
        TempData["Error"] = "Category is missing. Please select a category.";
    }
}
}
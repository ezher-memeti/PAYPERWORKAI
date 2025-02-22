using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace MyRazorApp.Website.UI.Pages{
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
    public string VideoFileName { get; set; }
    public string VideoDownloadUrl { get; set; }
    public string VideoStreamUrl { get; set; }

    public async Task OnGetAsync()
    {
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5123"); 

                var response = await client.GetAsync("/api/video/latest-video");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);
                    VideoFileName = result.fileName;
                    VideoDownloadUrl = $"/api/video/download/{VideoFileName}";
                    VideoStreamUrl = $"/api/video/stream/{VideoFileName}";
                }
            }
        }
    }

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
}
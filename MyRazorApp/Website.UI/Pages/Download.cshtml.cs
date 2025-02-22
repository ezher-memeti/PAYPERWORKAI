using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace MyRazorApp.Website.UI.Pages{
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
    public string VideoFileName { get; set; }
    public string VideoDownloadUrl { get; set; }
    public string VideoStreamUrl { get; set; }

    public async Task OnGetAsync()
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
}
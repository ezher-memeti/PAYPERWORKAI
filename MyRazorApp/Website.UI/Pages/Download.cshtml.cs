using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using MyRazorApp.Website.API.Models;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MyRazorApp.Website.UI.Pages{
public class DownloadModel : PageModel
{
    [BindProperty(Name = "image1Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image1FileName { get; set; }

    [BindProperty(Name = "image2Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image2FileName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Prompt { get; set; }
    public string VideoFileName { get; set; }
    public string VideoDownloadUrl { get; set; }
    public string VideoStreamUrl { get; set; }
    public string Image1Url { get; set;}
    public string Image2Url { get; set;}

    [BindProperty(SupportsGet = true)]
    public string Duration { get; set; }

    [BindProperty(SupportsGet = true)]
    public string NegativePrompt {get; set; }

    [BindProperty(SupportsGet = true)]
    public string UIPrompt { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Image2Controller { get; set; }

    private readonly IHttpClientFactory _httpClientFactory;

    public DownloadModel(IHttpClientFactory httpClientFactory){
        _httpClientFactory =httpClientFactory;
    }

    public async Task<IActionResult> OnGetAsync()
    {

        Console.WriteLine("OnGetAsync execution started at: " + DateTime.Now);

        Console.WriteLine("IMAGE1 name: " + Image1FileName);
        Console.WriteLine("IMAGE2 name: " + Image2FileName);
        Console.WriteLine("PROMPT: "+ Prompt);
        Console.WriteLine("CATEGORY: " + Category);
        Console.WriteLine("DURATION: "+Duration);

        var client = _httpClientFactory.CreateClient("server");
        var photoResponse = await client.GetAsync("/api/server/get-latest-images");
        var photoResponseJson = await photoResponse.Content.ReadAsStringAsync();

        if (photoResponse.IsSuccessStatusCode)
        {
            var result = JObject.Parse(photoResponseJson);

            Image1Url = result["image1Url"]?.ToString();
            //if image 2 is not uploaded it = null
            Image2Url = null;
            if(Image2Controller){
                Image2Url = result["image2Url"]?.ToString();
            }

            Console.WriteLine("RESPONSE: " + photoResponseJson);
            Console.WriteLine("IMAGE1ULR: " + Image1Url);
            Console.WriteLine("IMAGE2ULR: " + Image2Url);
        }
        else
        {
            Console.WriteLine("FAILED TO FETCH IMAGES");
            return Page();
        }

    



        Console.WriteLine("OnGetAsync is being executed");
        return Page();
    }

    /*public void OnGet()
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

    }*/
}
}
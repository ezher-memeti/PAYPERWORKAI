using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using MyRazorApp.Website.API.Models;

namespace MyRazorApp.Website.UI.Pages{
public class CategorySelectionModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string SelectedCategory { get; set; } = "Cinematic"; // Default category
    
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public IFormFile Image1 { get; set; }

    [BindProperty]
    public IFormFile Image2 { get; set; }

    [BindProperty]
    public string Prompt{ get; set; }



    public CategorySelectionModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<string> Categories { get; set; } = new()
    {
        "Cinematic", "Fashion", "Food", "Architecture", "Science Fiction", "Personal Video", "Cars"
    };


    public void OnGet()
    {
        var categoryQuery = Request.Query["category"].ToString();
        if (!string.IsNullOrEmpty(categoryQuery) && Categories.Contains(categoryQuery))
        {
            SelectedCategory = categoryQuery;
        }
    }
    
    
    public async Task<IActionResult> OnPostAsync()
    {
    // Validate both images
    if (Image1 == null || Image1.Length == 0)
    {
        ModelState.AddModelError("", "Please select an image.");
        return Page();
    }

    if (Image2 == null || Image2.Length == 0)
    {
        ModelState.AddModelError("", "Please select a tail image.");
        return Page();
    }
    
    var client = _httpClientFactory.CreateClient("server");
    string Image1Name = "";
    string Image2Name = "";
    
    using (var formData = new MultipartFormDataContent())
    {
        // Add first image
        var fileStreamContent1 = new StreamContent(Image1.OpenReadStream());
        fileStreamContent1.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Adjust if needed
        formData.Add(fileStreamContent1, "file1", Image1.FileName);

        // Add second image
        var fileStreamContent2 = new StreamContent(Image2.OpenReadStream());
        fileStreamContent2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Adjust if needed
        formData.Add(fileStreamContent2, "file2", Image2.FileName);

        // Add additional string parameters
        formData.Add(new StringContent(Prompt), "prompt");

        // Send the POST request
        var response = await client.PostAsync("/api/server/upload", formData);

        // Check response
        if (!response.IsSuccessStatusCode)
        {
            ViewData["Message"] = "File upload failed.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(json);
        Image1Name = result.File1Name;
        Image2Name = result.File2Name;
    }
    


    
    return RedirectToPage("/Download", new
        {
            category = SelectedCategory,
            image1Url = Image1Name, // Parametre isimlerini düzelt
            image2Url = Image2Name,
            prompt = Prompt
        }
    ); 


}

}}
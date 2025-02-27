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
    



    
    return RedirectToPage("/Download", new
        {
            category = SelectedCategory,
            image1Url = Image1, // Parametre isimlerini düzelt
            image2Url = Image2,
            prompt = Prompt
        }
    ); 


}

}}
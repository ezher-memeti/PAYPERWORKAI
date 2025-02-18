using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class CategorySelectionModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string SelectedCategory { get; set; } = "Cinematic"; // Default category

    public List<string> Categories { get; set; } = new()
    {
        "Cinematic", "Fashion", "Food", "Architecture", "Science Fiction", "Personal Video", "Cars"
    };

    [BindProperty]
    public IFormFile Image1 { get; set; }

    [BindProperty]
    public IFormFile Image2 { get; set; }

    [BindProperty]
    public string Description { get; set; }

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
        if (Image1 != null && Image2 != null && !string.IsNullOrEmpty(Description))
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var image1Path = Path.Combine(uploadsFolder, Image1.FileName);
            var image2Path = Path.Combine(uploadsFolder, Image2.FileName);

            using (var stream = new FileStream(image1Path, FileMode.Create))
            {
                await Image1.CopyToAsync(stream);
            }

            using (var stream = new FileStream(image2Path, FileMode.Create))
            {
                await Image2.CopyToAsync(stream);
            }

            TempData["SuccessMessage"] = "Images and description successfully submitted!";

            // ✅ Return statement must be inside a method
            return RedirectToPage("/Download", new
            {
                category = SelectedCategory,
                image1 = "/uploads/" + Image1.FileName,
                image2 = "/uploads/" + Image2.FileName,
                prompt = Description
            });
        }

        ModelState.AddModelError(string.Empty, "Please upload two images and enter a description.");
        return Page();
    }
}

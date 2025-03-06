using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MyRazorApp.Website.UI.Pages;

public class HomeModel : PageModel
{
    public string Username { get; set; }

     [BindProperty]
     public IFormFile Photo1 { get; set; }
        
    [BindProperty]
    public IFormFile? Photo2 { get; set; }

    public IActionResult OnGet()
    {
        // Retrieve the username from the session
        Username = HttpContext.Session.GetString("Username");
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        // Clear the session to log the user out
        HttpContext.Session.Remove("IsAuthenticated");
        HttpContext.Session.Remove("Username");

        // Redirect to the Index page
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostUploadImagesAsync()
        {
            // Folder path to save images
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Handle Image 1
            if (Photo1 != null)
            {
                var fileName1 = Path.GetRandomFileName() + Path.GetExtension(Photo1.FileName);
                var filePath1 = Path.Combine(folderPath, fileName1);

                using (var stream = new FileStream(filePath1, FileMode.Create))
                {
                    await Photo1.CopyToAsync(stream);
                }
            }

            // Handle Image 2
            if (Photo2 != null)
            {
                var fileName2 = Path.GetRandomFileName() + Path.GetExtension(Photo2.FileName);
                var filePath2 = Path.Combine(folderPath, fileName2);

                using (var stream = new FileStream(filePath2, FileMode.Create))
                {
                    await Photo2.CopyToAsync(stream);
                }
            }

            TempData["SuccessMessage"] = "Images uploaded successfully!";
            return RedirectToPage();
        }
}  


//deneme
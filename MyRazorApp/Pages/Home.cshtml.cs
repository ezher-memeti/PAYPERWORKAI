using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MyRazorApp.Pages;

public class HomeModel : PageModel
{
    public string Username { get; set; }

    public void OnGet()
    {
        // Retrieve the username from the session
        Username = HttpContext.Session.GetString("Username");
    }

    public IActionResult OnPostLogout()
    {
        // Clear the session to log the user out
        HttpContext.Session.Remove("IsAuthenticated");
        HttpContext.Session.Remove("Username");

        // Redirect to the Index page
        return RedirectToPage("/Index");
    }
}  
//deneme
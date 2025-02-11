using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string Message { get; set; }

    public void OnGet()
    {
        // Check if the user is already logged in
        if (HttpContext.Session.GetString("IsAuthenticated") == "true")
        {
            Message = "You are already logged in!";
        }
    }

    public IActionResult OnPostLogin()
    {
        // Hardcoded credentials for demonstration purposes
        string validUsername = "admin";
        string validPassword = "password";

        if (Username == validUsername && Password == validPassword)
        {
            // Set session to indicate the user is authenticated
            HttpContext.Session.SetString("IsAuthenticated", "true");
            HttpContext.Session.SetString("Username", Username);

            // Redirect to the Home page
            return RedirectToPage("/Home");
        }
        else
        {
            Message = "Invalid username or password.";
            return Page();
        }
    }

    public IActionResult OnPostLogout()
    {
        // Clear the session to log the user out
        HttpContext.Session.Remove("IsAuthenticated");
        HttpContext.Session.Remove("Username");

        Message = "You have been logged out.";
        return RedirectToPage("/Index");
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MyRazorApp.Website.UI.Pages{

public class RegisterModel : PageModel
{
    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public string Message { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (Password != ConfirmPassword)
        {
            Message = "Passwords do not match!";
            return Page();
        }

        // In a real application, you would save the user to a database here
        // For demonstration, we'll just redirect to login
        return RedirectToPage("/SignIn", new { message = "Registration successful! Please login." });
    }
}}
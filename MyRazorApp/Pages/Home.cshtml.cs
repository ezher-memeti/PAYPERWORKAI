using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyRazorApp.Pages;

public class HomeModel : PageModel
{
    public string Username { get; set; }

    public void OnGet()
    {
        Username = HttpContext.Session.GetString("Username");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            // Authentication check
            if (HttpContext.Session.GetString("IsAuthenticated") != "true")
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Authentication required",
                    redirect = Url.Page("/Index")
                });
            }

            // Validate form data
            var photo1 = Request.Form.Files["photo1"];
            var photo2 = Request.Form.Files["photo2"];
            var userText = Request.Form["userText"];

            if (photo1 == null || photo2 == null || photo1.Length == 0 || photo2.Length == 0)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Please upload both images" 
                });
            }

            // Convert images to base64
            var photo1Base64 = await ConvertToBase64(photo1);
            var photo2Base64 = await ConvertToBase64(photo2);

            // Prepare API payload
            var payload = new
            {
                photo1 = photo1Base64,
                photo2 = photo2Base64,
                text = userText,
                category = Request.Form["selectedCategory"] // Kategori bilgisini ekledik
            };

            // Send to Kling.ai API
            var apiKey = "4dc056d0087d4c3fa02e02c774a66f04";
            var apiUrl = "https://api.kling.ai/v1/generate-video";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);

            var response = await httpClient.PostAsync(
                apiUrl,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = $"API Error: {response.StatusCode}" 
                });
            }

            // Parse API response (Örnek format, API dokümantasyonuna göre güncelleyin)
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<KlingApiResponse>(responseContent);

            return new JsonResult(new { 
                success = true,
                videoUrl = apiResponse?.video_url,
                message = "Video başarıyla oluşturuldu!"
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { 
                success = false, 
                message = $"Error: {ex.Message}" 
            });
        }
    }

    private async Task<string> ConvertToBase64(IFormFile file)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove("IsAuthenticated");
        HttpContext.Session.Remove("Username");
        return RedirectToPage("/Index");
    }

    // Kling API yanıt modeli (API dokümantasyonuna göre güncelleyin)
    private class KlingApiResponse
    {
        public string video_url { get; set; }
        public string status { get; set; }
    }
}
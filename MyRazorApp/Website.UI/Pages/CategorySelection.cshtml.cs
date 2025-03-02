using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using MyRazorApp.Website.API.Models;

namespace MyRazorApp.Website.UI.Pages{
public class CategorySelectionModel : PageModel
{   
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public IFormFile Image1 { get; set; }

    [BindProperty]
    public IFormFile Image2 { get; set; }

    [BindProperty]
    public string Prompt{ get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedCategory { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedPerspective { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedShotType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedCameraMovement { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedFormat { get; set; }

    [BindProperty]
    public string SelectedDuration { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedStyle { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedLightColor { get; set; }

    public List<string> Categories { get; set; } = new()
    {
        "Cinematic", "Fashion", "Food", "Architecture", "Science Fiction", "Personal Video", "Cars"
    };

    public List<string> Perspectives { get; set; } = new()
    {
        "Bird's Eye View", "Worm's Eye View", "Side View", "I-Level", "Shoulder Level", "Frontal View", "Angled View","Isometric View"
    };

    public List<string> ShotTypes { get; set; } = new()
    {
        "Total Shot", "Close-Up", "Medium Close-Up", "Drone Shot", "Extreme Close-Up", "Over-the-Shoulder Shot",
        "Establishing Shot", "Full Shot"
    };

    public List<string> CameraMovements { get; set; } = new()
    {
        "Static", "Push-In", "Push-Out", "Handheld", "Orbit", "Pan", "Tilt", "Zoom-In", "Zoom-Out", "Dolly"
    };

    public List<string> Formats { get; set; } = new()
    {
        "16:9", "9:16", "1:1"
    };

    public List<string> Durations { get; set; } = new()
    {
        "5 sec.", "10 sec."
    };

    public List<string> Styles { get; set; } = new()
    {
        "Cartoon", "Disney", "Ultra-realistic", "Anime", "Cyberpunk", "Minimalistic", "Impressionistic"
    };

    public List<string> LightColors { get; set; } = new()
    {
        "Black & White", "RAW", "Cool Blue", "Warm-toned", "Neon", "Pastel"
    };



    public CategorySelectionModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }



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
        Console.WriteLine("result response: " + result);
        Image1Name = result.file1Name;
        Image2Name = result.file2Name;
        Console.WriteLine("IMAGE1 name: " + Image1Name);
        Console.WriteLine("IMAGE2 name: " + Image2Name);
    }
    
    // Build the dynamic prompt based on selected dropdowns
    string prompt = $"{Prompt}. ";
    prompt += GetCategoryPrompt(SelectedCategory);
    prompt += GetPerspectivePrompt(SelectedPerspective);
    prompt += GetShotTypePrompt(SelectedShotType);
    prompt += GetCameraMovementPrompt(SelectedCameraMovement);
    prompt += GetFormatPrompt(SelectedFormat);
    prompt += GetDurationPrompt(SelectedDuration);
    prompt += GetStylePrompt(SelectedStyle);
    prompt += GetLightColorPrompt(SelectedLightColor);
    

    string negativePrompt = $"{GetNegativeCategoryPrompt(SelectedCategory)}";
    Console.WriteLine(prompt+" prompt"+ negativePrompt+ "Negative");

    
    return RedirectToPage("/Download", new
        {
            category = SelectedCategory,
            image1Url = Image1Name, // Parametre isimlerini düzelt
            image2Url = Image2Name,
            Prompt = prompt,
            negativePrompt = negativePrompt
        }
    ); 


}

private string GetCategoryPrompt(string category)
{
    return category switch
    {
        "Cinematic" => "High quality, ultra-detailed cinematic scene with balanced, dynamic lighting, rich color grading, cinematic depth, 8K resolution, and sharp textures. The image should exude a filmic atmosphere with smooth transitions and natural tonal balance.",
        "Fashion" => "High quality, ultra-detailed fashion scene with crisp, elegant textures, precise color accuracy, and 8K resolution. Emphasize refined details, consistent logo integrity, and a stylish composition with luxurious ambient lighting.",
        "Food" => "High quality, ultra-detailed food scene with vibrant, appetizing colors, crisp textures, and 8K resolution. Emphasize natural lighting, freshness, and meticulous detail in every ingredient for a mouth-watering presentation.",
        "Architecture" =>  "High quality, ultra-detailed architectural scene with clear, precise structures, balanced natural lighting, cinematic depth, 8K resolution, and refined textures. Emphasize clean lines, geometric precision, and realistic details that showcase the design.",
        "Science Fiction" => "High quality, ultra-detailed futuristic scene with glowing neon accents, deep atmospheric lighting, cinematic depth, 8K resolution, and clear textures. The image should exude a sense of futuristic realism with immersive ambient effects.",
        "Personal Video" => "High quality, ultra-detailed personal video with natural, flattering lighting, 8K resolution, and crisp, clear textures. Emphasize authentic details, balanced color grading, and smooth transitions to capture a genuine and polished look.",
        "Cars" => "High quality, ultra-detailed automotive scene with shiny, reflective surfaces, crisp textures, cinematic depth, and 8K resolution. Emphasize realistic materials, dynamic lighting, and a polished, high-gloss finish for a visually striking presentation.",
        _ => ""
    };
}

private string GetNegativeCategoryPrompt(string category){
    return category switch
    {
        "Cinematic" => "Exclude low resolution, blur, oversaturated colors, harsh lighting, digital noise, and any artifacts that disrupt the cinematic quality.",
        "Fashion" => "Exclude low detail, blurry textures, logo distortions, inconsistent colors, and any artifacts that compromise the fashion aesthetic.",
        "Food" => "Exclude blur, low resolution, dull colors, poor texture definition, and any artifacts that diminish the food's appeal.",
        "Architecture" =>  "Exclude distortions, blur, low resolution, lack of detail, and any digital artifacts that compromise structural integrity.",
        "Science Fiction" => "Exclude glitches, blur, low resolution, low texture, artifacts, pixelation, oversaturation, and any unwanted digital noise.",
        "Personal Video" => "Exclude low resolution, blur, overexposure, digital noise, and any artifacts that detract from the personal authenticity.",
        "Cars" => "Exclude glitches, low texture, blur, low resolution, digital artifacts, and any imperfections that reduce the vehicle's visual appeal.",
        _ => ""
    };
}

private string GetPerspectivePrompt(string perspective)
{
    return perspective switch
    {
        "Bird's Eye View" => "Render the scene from a bird's eye view, capturing the entire landscape from above with expansive detail.",
        "Worm's Eye View" => "Render the scene from a worm's eye view, emphasizing dramatic low-angle perspective.",
        "Side View" => "Render the scene from a side view, showcasing a lateral perspective of the subject.",
        "I-Level" => "Render the scene at eye level, providing a natural and immersive viewpoint.",
        "Shoulder Level" => "Render the scene from shoulder level to create a slightly elevated, personal perspective.",
        "Frontal View" => "Render the scene head-on with a frontal view that captures the subject directly.",
        "Angled View" => "Render the scene from an angled view to add dynamic depth and interest.",
        "Isometric View" => "Render the scene in isometric view, giving a geometric and stylized perspective.",
        _ => ""
    };
}

private string GetShotTypePrompt(string shotType)
{
    return shotType switch
    {
        "Total Shot" => "Generate a total shot that captures the entire scene and environment in full scale.",
        "Close-Up" => "Generate a close-up shot focusing on the subject's detailed expressions and features.",
        "Medium Close-Up" => "Generate a medium close-up shot that balances the subject with some background context.",
        "Drone Shot" => "Generate a drone shot providing an aerial view of the scene with sweeping coverage.",
        "Extreme Close-Up" => "Generate an extreme close-up to emphasize intricate details of the subject.",
        "Over-the-Shoulder Shot" => "Generate an over-the-shoulder shot that frames the subject from behind another character.",
        "Establishing Shot" => "Generate an establishing shot that sets the scene by capturing the broader context of the environment.",
        "Full Shot" => "Generate a full shot that shows the subject completely along with its surroundings.",
        _ => ""
    };
}

private string GetCameraMovementPrompt(string cameraMovement)
{
    return cameraMovement switch
    {
        "Static" => "Keep the camera static, with no movement.",
        "Push-In" => "Animate a gentle push-in movement that gradually brings the subject closer to the viewer.",
        "Push-Out" => "Animate a push-out movement to gradually reveal more of the environment.",
        "Handheld" => "Simulate a handheld camera effect with natural jitters for a realistic, dynamic feel.",
        "Orbit" => "Animate an orbit movement around the subject for a 360-degree dynamic view.",
        "Pan" => "Animate a smooth horizontal pan across the scene.",
        "Tilt" => "Animate a vertical tilt movement to follow the subject's motion.",
        "Zoom-In" => "Animate a gradual zoom-in to focus on detailed aspects of the subject.",
        "Zoom-Out" => "Animate a gradual zoom-out to reveal the broader environment.",
        "Dolly" => "Simulate a dolly movement that smoothly tracks the subject along a set path.",
        _ => ""
    };
}

private string GetFormatPrompt(string format)
{
    return format switch
    {
        "16:9" => "Set the video aspect ratio to 16:9 for a widescreen cinematic look.",
        "9:16" => "Set the video aspect ratio to 9:16, ideal for vertical mobile displays.",
        "1:1" => "Set the video aspect ratio to 1:1, suitable for square-format presentations.",
        "4:3" => "Set the video aspect ratio to 4:3 for a classic television look.",
        "2:1" => "Set the video aspect ratio to 2:1 to achieve a panoramic, cinematic feel.",
        _ => ""
    };
}

private string GetDurationPrompt(string duration)
{
    return duration switch
    {
        "5s" => "Set the video duration to 5 seconds.",
        "10s" => "Set the video duration to 10 seconds.",
        _ => ""
    };
}

private string GetStylePrompt(string style)
{
    return style switch
    {
        "Cartoon" => "Apply a cartoon style with simplified forms, bold outlines, and vibrant, playful colors.",
        "Disney" => "Apply a Disney-inspired style with soft, magical, and animated aesthetics.",
        "Ultra-realistic" => "Render the scene in ultra-realistic detail with high-fidelity textures and lifelike lighting.",
        "Anime" => "Apply an anime style with expressive characters and stylized, dynamic backgrounds.",
        "Cyberpunk" => "Apply a cyberpunk style with neon accents, futuristic elements, and urban grit.",
        "Minimalistic" => "Apply a minimalistic style with clean lines and a limited color palette.",
        "Impressionistic" => "Apply an impressionistic style with visible brushstrokes and a soft, dreamlike focus.",
        _ => ""
    };
}

private string GetLightColorPrompt(string lightColor)
{
    return lightColor switch
    {
        "Black & White" => "Render the video in black and white for a classic, timeless visual style.",
        "RAW" => "Apply a RAW aesthetic with high contrast and natural, unprocessed colors.",
        "Cool Blue" => "Apply a cool blue color tone to evoke a futuristic and calm atmosphere.",
        "Warm-toned" => "Apply warm tones to create a cozy, inviting ambiance.",
        "Neon" => "Apply neon lighting effects with vibrant, glowing hues to enhance a high-tech feel.",
        "Pastel" => "Apply a pastel color scheme for a soft, gentle visual mood.",
        _ => ""
    };
}





}}




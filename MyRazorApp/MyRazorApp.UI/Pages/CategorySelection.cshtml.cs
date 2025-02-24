using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class CategorySelectionModel : PageModel
{
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

    [BindProperty(SupportsGet = true)]
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
        "16:9", "9:16", "1:1", "4:3", "2:1"
    };

    public List<string> Durations { get; set; } = new()
    {
        "5 seconds", "10 seconds"
    };

    public List<string> Styles { get; set; } = new()
    {
        "Cartoon", "Disney", "Ultra-realistic", "Anime", "Cyberpunk", "Minimalistic", "Impressionistic"
    };

    public List<string> LightColors { get; set; } = new()
    {
        "Black & White", "RAW", "Cool Blue", "Warm-toned", "Neon", "Pastel"
    };

    [BindProperty]
    public IFormFile Image1 { get; set; }

    [BindProperty]
    public IFormFile Image2 { get; set; }

    [BindProperty]
    public string Description { get; set; }

    public void OnGet()
{
    // Log the full query string for debugging
    Console.WriteLine("Query Parameters: " + Request.QueryString);

    // Extract query parameters safely
    SelectedCategory = Request.Query["category"].ToString();
    SelectedPerspective = Request.Query["perspective"].ToString();
    SelectedShotType = Request.Query["shotType"].ToString();
    SelectedCameraMovement = Request.Query["cameraMovement"].ToString();
    SelectedFormat = Request.Query["format"].ToString();
    SelectedDuration = Request.Query["duration"].ToString();
    SelectedStyle = Request.Query["style"].ToString();
    SelectedLightColor = Request.Query["lightColor"].ToString();

    // Debugging: Log extracted values
    Console.WriteLine($"SelectedCategory: {SelectedCategory}");
    Console.WriteLine($"SelectedPerspective: {SelectedPerspective}");
    Console.WriteLine($"SelectedShotType: {SelectedShotType}");
    Console.WriteLine($"SelectedCameraMovement: {SelectedCameraMovement}");
    Console.WriteLine($"SelectedFormat: {SelectedFormat}");
    Console.WriteLine($"SelectedDuration: {SelectedDuration}");
    Console.WriteLine($"SelectedStyle: {SelectedStyle}");
    Console.WriteLine($"SelectedLightColor: {SelectedLightColor}");
}


  public async Task<IActionResult> OnPostAsync()
{
    if (Image1 != null && Image2 != null && !string.IsNullOrEmpty(Description))
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        Directory.CreateDirectory(uploadsFolder);

        var image1Name = $"{Guid.NewGuid()}{Path.GetExtension(Image1.FileName)}";
        var image2Name = $"{Guid.NewGuid()}{Path.GetExtension(Image2.FileName)}";

        var image1Path = Path.Combine(uploadsFolder, image1Name);
        var image2Path = Path.Combine(uploadsFolder, image2Name);

        using (var stream = new FileStream(image1Path, FileMode.Create))
        {
            await Image1.CopyToAsync(stream);
        }

        using (var stream = new FileStream(image2Path, FileMode.Create))
        {
            await Image2.CopyToAsync(stream);
        }

        // Build the dynamic prompt based on selected dropdowns
        string prompt = $"{Description}. ";
        prompt += GetPerspectivePrompt(SelectedPerspective);
        prompt += GetShotTypePrompt(SelectedShotType);
        prompt += GetCameraMovementPrompt(SelectedCameraMovement);
        prompt += GetFormatPrompt(SelectedFormat);
        prompt += GetDurationPrompt(SelectedDuration);
        prompt += GetStylePrompt(SelectedStyle);
        prompt += GetLightColorPrompt(SelectedLightColor);

           // Log the generated prompt
        Console.WriteLine("Generated Prompt: " + prompt);
        Console.WriteLine("SelectedPerspective: " + SelectedPerspective);
        Console.WriteLine("SelectedPerspective: " + SelectedCategory);


        // Redirect to the download page with all the parameters including the prompt
        return RedirectToPage("/Download", new
        {
            category = SelectedCategory,
            image1Url = $"/uploads/{image1Name}",
            image2Url = $"/uploads/{image2Name}",
            prompt = prompt
        });
    }

    ModelState.AddModelError(string.Empty, "Please upload two images and enter a description.");
    return Page();
}


///UPDATE
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
        "5 seconds" => "Set the video duration to 5 seconds.",
        "10 seconds" => "Set the video duration to 10 seconds.",
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
}

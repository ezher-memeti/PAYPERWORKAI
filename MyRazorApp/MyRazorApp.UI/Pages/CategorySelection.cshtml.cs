using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

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
    }

    // Step 2: Trigger video generation
    var videoGenerationRequest = new VideoGenerationRequest
    {
        Prompt = Prompt,
        Image = Image1.FileName, // Send only the filenames of the uploaded images
        ImageTail = Image2.FileName,
        NegativePrompt = "",  // Set if you have a negative prompt
        CfgScale = 0.5f,      // Adjust as necessary
        Mode = "std",         // Adjust as necessary
        StaticMask= "",
        DynamicMasks= [],
        Duration = "5",       // Adjust as necessary
        CallbackUrl = "",     // Set if you have a callback URL
        ExternalTaskId = ""   // Set if you need to include an external task ID
    };

    var jsonContent = JsonConvert.SerializeObject(videoGenerationRequest); // Serializing the object
    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    // Step 3: Send the video generation request
    var videoResponse = await client.PostAsync("/api/video/generate", content);

    // Check if video generation was successful
    if (videoResponse.IsSuccessStatusCode)
    {
        var result = await videoResponse.Content.ReadAsStringAsync();
        ViewData["Message"] = "Video generated successfully. " + result;
    }
    else
    {
        var errorResult = await videoResponse.Content.ReadAsStringAsync();
        ViewData["Message"] = "Video generation failed. " + errorResult;
    }

    return Page();


    
}
public class VideoGenerationRequest
    {
        
        public string Prompt { get; set; } = "";
        public string Image { get; set; } =""; // Only the filename, e.g., "myImage.jpg"
        public string ImageTail { get; set; } ="";
        public string NegativePrompt { get; set; } ="";
        public float CfgScale { get; set; } = 0.5f; 
        public string Mode { get; set; } = "std";
        public string StaticMask { get; set; } ="";
        public dynamic[] DynamicMasks { get; set; } = [];
        public string Duration { get; set; } = "5";
        public string CallbackUrl { get; set; } ="";
        public string ExternalTaskId { get; set; } ="";
    }
}

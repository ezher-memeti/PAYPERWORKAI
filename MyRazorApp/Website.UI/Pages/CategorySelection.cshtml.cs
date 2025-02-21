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
    var videoResponseString = await videoResponse.Content.ReadAsStringAsync();

    if (videoResponseString.StartsWith("\"") && videoResponseString.EndsWith("\""))
    {
        videoResponseString = JsonConvert.DeserializeObject<string>(videoResponseString); // First step to remove quotes
    }

        // Now properly deserialize it into VideoQueryResponse
    var videoQueryResponse = JsonConvert.DeserializeObject<VideoGenerationResponse>(videoResponseString);


    string taskId = "";

    if (videoQueryResponse != null && videoQueryResponse.Data != null)
    {
        taskId = videoQueryResponse.Data.TaskId;
        ViewData["Message"] = $"Video submitted successfully. Task ID: {taskId}";
    }
    else
    {
        ViewData["Message"] = "Video submission failed. Invalid response.";
    }
     

   
   
    
   // Step 4: Query Video Task Status every 30 seconds until it succeeds
    bool taskSucceeded = false;
    string videoUrl = "";

    while (!taskSucceeded)
    {
        try
        {
            var queryResponse = await client.GetAsync($"/api/video/query/{taskId}");
            if (!queryResponse.IsSuccessStatusCode)
            {
                ViewData["Message"] = "Failed to query video status.";
                return Page();
            }

            var queryResultJson = await queryResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Raw JSON Response: " + queryResultJson); // Log the response

            if (queryResultJson.StartsWith("\"") && queryResultJson.EndsWith("\""))
            {
                queryResultJson = JsonConvert.DeserializeObject<string>(queryResultJson); // First step to remove quotes
            }

            var queryResult = JsonConvert.DeserializeObject<VideoQueryResponse>(queryResultJson);




            if (queryResult?.Data?.TaskResult?.Videos != null && queryResult.Data.TaskResult.Videos.Count > 0)
            {
                var task_status = queryResult.Data.TaskStatus?.ToLower();

                if (task_status == "succeed") 
                {   
                    var videoList = queryResult.Data.TaskResult?.Videos;
                    if (videoList != null && videoList.Count > 0)
                    {
                        videoUrl = videoList[0].Url;
                        taskSucceeded = true;
                        break; // ✅ Break loop once success
                    }
                    else
                    {
                        ViewData["Message"] = "Video generation succeeded, but no video URL found.";
                        return Page();
                    }
                }
                else if (task_status == "failed")
                {
                    ViewData["Message"] = "Video generation failed.";
                    return Page();
                }
            }
        }
        catch (JsonSerializationException ex)
        {
           Console.WriteLine("❌JSON Deserialization Error: " + ex.Message); 
        }

        // Wait 30 seconds before retrying
            await Task.Delay(30000);

    }   


    if (string.IsNullOrEmpty(videoUrl))
    {
        ViewData["Message"] = "No video URL found.";
        return Page();
    }

    // Step 5: Download the Video
    var downloadRequest = new { videoUrl };
    var downloadContent = new StringContent(JsonConvert.SerializeObject(downloadRequest), Encoding.UTF8, "application/json");

    var downloadResponse = await client.PostAsync("/api/video/download", downloadContent);

    if (!downloadResponse.IsSuccessStatusCode)
    {
        var downloadError = await downloadResponse.Content.ReadAsStringAsync();
        ViewData["Message"] = "Video download failed. " + downloadError;
        return Page();
    }

    var downloadResultJson = await downloadResponse.Content.ReadAsStringAsync();
    var downloadResult = JsonConvert.DeserializeObject<dynamic>(downloadResultJson);

    string downloadUrl = downloadResult?.url ?? "";

    if (!string.IsNullOrEmpty(downloadUrl))
    {
        ViewData["Message"] = "Video downloaded successfully! Download it here: " + downloadUrl;
    }
    else
    {
        ViewData["Message"] = "Download URL not found.";
    }

    return Page();
}

}}
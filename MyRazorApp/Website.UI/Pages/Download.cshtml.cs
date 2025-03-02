using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using MyRazorApp.Website.API.Models;
using System.Text;

namespace MyRazorApp.Website.UI.Pages{
public class DownloadModel : PageModel
{
    [BindProperty(Name = "image1Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image1FileName { get; set; }

    [BindProperty(Name = "image2Url", SupportsGet = true)] // Parametre ismi eklendi
    public string Image2FileName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Prompt { get; set; }
    public string VideoFileName { get; set; }
    public string VideoDownloadUrl { get; set; }
    public string VideoStreamUrl { get; set; }
    public string Image1Url { get; set;}
    public string Image2Url { get; set;}
    [BindProperty(SupportsGet = true)]
    public string NegativePrompt {get; set; }
    private readonly IHttpClientFactory _httpClientFactory;

    public DownloadModel(IHttpClientFactory httpClientFactory){
        _httpClientFactory =httpClientFactory;
    }

    public async Task<IActionResult> OnGetAsync()
    {

     
    var client = _httpClientFactory.CreateClient("server");
    var photoResponse = await client.GetAsync("/api/server/get-latest-images");

    if (photoResponse.IsSuccessStatusCode)
    {
        var json = await photoResponse.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(json);

        Image1Url = result.Image1Url;
        Image2Url = result.Image2Url;
    }
    else
    {
        return Page();
    }

    
    // Step 2: Trigger video generation
    var videoGenerationRequest = new VideoGenerationRequest
    {
        Prompt = Prompt,
        Image = Image1FileName, // Send only the filenames of the uploaded images
        ImageTail = Image2FileName,
        NegativePrompt = NegativePrompt,
        CfgScale = 0.5f,      // Adjust as necessary
        Mode = "std",         // Adjust as necessary
        StaticMask= "",
        DynamicMasks= [],
        Duration = "5",       // Adjust as necessary
        CallbackUrl = "",     // Set if you have a callback URL
        ExternalTaskId = ""   // Set if you need to include an external task ID
    };
    Console.WriteLine("pr "+Prompt+"ng "+NegativePrompt);
    var jsonContent = JsonConvert.SerializeObject(videoGenerationRequest); // Serializing the object
    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    /* Step 3: Send the video generation request
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
    }*/
    
     

   
   
    
   // Step 4: Query Video Task Status every 30 seconds until it succeeds
    bool taskSucceeded = false;
    string videoUrl = "";

    while (!taskSucceeded)
    {
        try
        {
            var queryResponse = await client.GetAsync($"/api/video/query/CjJi7me0aekAAAAAAEzMSA");
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








































































        Console.WriteLine("OnGetAsync is being executed");
        {

            
            var response = await client.GetAsync("/api/server/latest-video");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);
                VideoFileName = result.fileName;
                Console.WriteLine("Video file name: " + VideoFileName);

                var VideoDownloadUrlResponse = await client.GetAsync($"api/server/download/{VideoFileName}");
                var VideoDownloadUrlJson = await VideoDownloadUrlResponse.Content.ReadAsStringAsync();
                var VideoDownloadUrlObj = JsonConvert.DeserializeObject<dynamic>(VideoDownloadUrlJson);
                VideoDownloadUrl = VideoDownloadUrlObj.url;

                Console.WriteLine("Video download URL: " + VideoDownloadUrl);



                var VideoStreamUrlResponse = await client.GetAsync($"api/server/stream/{VideoFileName}");
                var VideoStreamUrlJson = await VideoStreamUrlResponse.Content.ReadAsStringAsync();
                var VideoStreamUrlObj = JsonConvert.DeserializeObject<dynamic>(VideoStreamUrlJson);
                VideoStreamUrl = VideoStreamUrlObj.url; // Extract URL from JSON

                Console.WriteLine("Video stream URL: " + VideoStreamUrl);
            }
            
        }
        return Page();
    }

    /*public void OnGet()
    {
    // Debug için ekstra kontroller
        if (string.IsNullOrEmpty(Image1Url) || string.IsNullOrEmpty(Image2Url))
        {
            // Hata durumunda loglama
            Console.WriteLine($"Missing images: {Image1Url ?? "null"}, {Image2Url ?? "null"}");
            // Kullanıcıya hata mesajı
            TempData["Error"] = "Image URLs are missing. Please upload images again.";
        }

        if (string.IsNullOrEmpty(Category))
        {
            // Kategori bilgisi eksikse hata mesajı
            TempData["Error"] = "Category is missing. Please select a category.";
        }

    }*/
}
}
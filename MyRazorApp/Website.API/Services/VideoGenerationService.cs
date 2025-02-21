using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyRazorApp.Services.KlingAPI
{
    public class VideoGenerationService
    {
        private readonly string _apiUrl = "https://api.klingai.com/v1/videos/image2video";

        public async Task<string> GenerateVideo(string token, string prompt, string image = null, string imageTail = null, 
            string negativePrompt = null, float cfgScale = 0.5f, string mode = "std", string staticMask = null, 
            dynamic[] dynamicMasks = null, string duration = "5", string callbackUrl = null, string externalTaskId = null)
        {
            using (var client = new HttpClient())
            {
                // Set the Authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Create the request body
                var requestBody = new
                {
                    model_name = "kling-v1",      // Optional
                    image = image,                // Required if provided, else null
                    image_tail = imageTail,        // Optional
                    prompt = prompt,               // Optional
                    negative_prompt = negativePrompt,  // Optional
                    cfg_scale = cfgScale,          // Optional
                    mode = mode,                   // Optional
                    static_mask = staticMask,      // Optional
                    dynamic_masks = dynamicMasks,  // Optional array of dynamic masks
                    duration = duration,           // Optional
                    callback_url = callbackUrl,    // Optional
                    external_task_id = externalTaskId // Optional
                };

                // Serialize the request body to JSON
                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await client.PostAsync(_apiUrl, content);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;  // Return the response data
                }
                else
                {
                    string errorData = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to generate video. Status code: {response.StatusCode}, Error: {errorData}");
                }
            }
        }
    }
}

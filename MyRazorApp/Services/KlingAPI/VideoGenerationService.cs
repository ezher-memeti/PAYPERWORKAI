using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyRazorApp.Services.KlingAPI
{
    public class VideoGenerationService
    {
        private readonly string _apiUrl = "https://api.klingai.com/v1/videos/text2video";  

        public async Task<string> GenerateVideo(string token, string prompt)
        {
            using (var client = new HttpClient())
            {
                // Set the Authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Create the request body
                var requestBody = new
                {
                    model_name = "kling-v1",
                    prompt = prompt,
                    negative_prompt = "",
                    cfg_scale = 0.7,
                    mode = "std",
                    camera_control = new
                    {
                        type = "simple",
                        config = new
                        {
                            horizontal = 2,
                            vertical = 0,
                            pan = 0,
                            tilt = 0,
                            roll = 0,
                            zoom = 0
                        }
                    },
                    aspect_ratio = "16:9",
                    duration = "5",
                    callback_url = "https://yourcallbackurl.com",
                    external_task_id = "custom-task-id"
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

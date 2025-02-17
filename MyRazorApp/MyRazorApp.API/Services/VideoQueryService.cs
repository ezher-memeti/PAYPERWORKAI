using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyRazorApp.Services.KlingAPI
{
    public class VideoQueryService
    {
        private readonly string _apiUrl = "https://api.klingai.com/v1/videos/image2video";

        public async Task<string> QueryVideoTaskStatus(string token, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Task ID or External Task ID is required.");
            }

            // Create the full URL with the task ID or external task ID
            string requestUrl = $"{_apiUrl}/{id}";

            using (var client = new HttpClient())
            {
                // Set the Authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Send the GET request
                var response = await client.GetAsync(requestUrl);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    string errorData = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to query video task status. Status code: {response.StatusCode}, Error: {errorData}");
                }
            }
        }
    }
}

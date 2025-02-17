using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class VideoDownloadClient
{
    private readonly string _apiUrl = "https://localhost:5062/api/video/download";

    public async Task<string> DownloadVideo(string videoUrl)
    {
        using (var client = new HttpClient())
        {
            // Corrected line: Use JsonConvert instead of JsonContent
            var jsonRequestBody = JsonConvert.SerializeObject(videoUrl);
            
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Failed to download video. Status code: {response.StatusCode}");
            }
        }
    }
}
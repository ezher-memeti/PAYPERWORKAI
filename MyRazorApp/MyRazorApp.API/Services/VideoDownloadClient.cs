using System.Text;

public class VideoDownloadClient
{
    private readonly string _apiUrl = "https://localhost:5062/api/video/download";

    public async Task<string> DownloadVideo(string videoUrl)
    {
        using (var client = new HttpClient())
        {
            var jsonRequestBody = JsonContent.SerializeObject(videoUrl);
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

using Newtonsoft.Json;
using System.Collections.Generic;
using MyRazorApp.Website.API.Models;

namespace MyRazorApp.Website.API.Models{

public class VideoGenerationResponse
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("request_id")]
    public string RequestId { get; set; }

    [JsonProperty("data")]
    public VideoQueryData Data { get; set; }
}
}
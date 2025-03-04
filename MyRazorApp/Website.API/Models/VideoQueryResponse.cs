using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyRazorApp.Website.API.Models
{
    public class VideoQueryResponse
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

public class VideoQueryData
{
    [JsonProperty("task_id")]
    public string TaskId { get; set; }

    [JsonProperty("task_status")]
    public string TaskStatus { get; set; }

    [JsonProperty("task_status_msg")]
    public string TaskStatusMsg { get; set; }

    [JsonProperty("task_result")]
    public TaskResult TaskResult { get; set; } = new TaskResult(); // ✅ Prevents null issues

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public long UpdatedAt { get; set; }
}

public class TaskResult
{
    [JsonProperty("videos")]
    public List<Video> Videos { get; set; } = new List<Video>(); // ✅ Prevents null issues
}

public class Video
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }
}
}

using Newtonsoft.Json;
using System.Collections.Generic;

public class Video
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("duration")]
    public string Duration { get; set; }
}

public class TaskResult
{
    [JsonProperty("videos")]
    public List<Video> Videos { get; set; }
}

public class Data
{
    [JsonProperty("task_id")]
    public string TaskId { get; set; }

    [JsonProperty("task_status")]
    public string TaskStatus { get; set; }

    [JsonProperty("task_status_msg")]
    public string TaskStatusMsg { get; set; }

    [JsonProperty("task_result")]
    public TaskResult TaskResult { get; set; }

    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public long UpdatedAt { get; set; }
}

public class VideoQueryResponse
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("request_id")]
    public string RequestId { get; set; }

    [JsonProperty("data")]
    public Data Data { get; set; }
}

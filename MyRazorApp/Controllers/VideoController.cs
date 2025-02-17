using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyRazorApp.Services.KlingAPI;

namespace MyRazorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly VideoGenerationService _videoGenerationService;
        private readonly JWTtoken _jwtService;
        private readonly ILogger<VideoController> _logger;
        private readonly string _uploadsFolder;
        private readonly VideoQueryService _videoQueryService;

        public VideoController(VideoGenerationService videoService, JWTtoken jwtService, ILogger<VideoController> logger,VideoQueryService videoQueryService)
        {
            _videoGenerationService = videoService;
            _videoQueryService = videoQueryService;
            _jwtService = jwtService;
            _logger = logger;
             _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateVideo([FromBody] VideoGenerationRequest request)
        {
            try
            {

                string imagePath = Path.Combine(_uploadsFolder, request.Image);
                string imageTailPath = Path.Combine(_uploadsFolder, request.ImageTail);
                
                // Check if the file exists
                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound(new { message = "Image not found in uploads folder." });
                }

                if (!System.IO.File.Exists(imageTailPath))
                {
                    return NotFound(new { message = "ImageTail not found in uploads folder." });
                }

                string imageBase64;
                using (var image = System.IO.File.OpenRead(imagePath))
                {
                    byte[] imageBytes = new byte[image.Length];
                    await image.ReadAsync(imageBytes, 0, (int)image.Length);
                    imageBase64 = Convert.ToBase64String(imageBytes);
                }

                string imageTailBase64;
                using (var image = System.IO.File.OpenRead(imageTailPath))
                {
                    byte[] imageBytes = new byte[image.Length];
                    await image.ReadAsync(imageBytes, 0, (int)image.Length);
                    imageTailBase64 = Convert.ToBase64String(imageBytes);
                }


                // Call the service to generate the video
                string result = await _videoGenerationService.GenerateVideo(
                    token: _jwtService.Sign(),
                    prompt: request.Prompt,
                    image: imageBase64,  // Pass the image path to the API
                    imageTail: imageTailBase64,
                    negativePrompt: request.NegativePrompt,
                    cfgScale: request.CfgScale,
                    mode: request.Mode,
                    staticMask: request.StaticMask,
                    dynamicMasks: request.DynamicMasks,
                    duration: request.Duration,
                    callbackUrl: request.CallbackUrl,
                    externalTaskId: request.ExternalTaskId
                );

                // Return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Return an error response if the video generation fails
                return BadRequest(new { message = ex.Message });
            }
        }

         // GET api/video/query/{id}
        [HttpGet("query/{id}")]
        public async Task<IActionResult> QueryVideoTaskStatus([FromRoute] string id)
        {
            try
            {
                var status = await _videoQueryService.QueryVideoTaskStatus(_jwtService.Sign(), id);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //POST api/video/download
        [HttpPost("download")]
        public async Task<IActionResult> DownloadVideo([FromBody] VideoUrl request)
        {
            string videoUrl = request.videoUrl;
            if (string.IsNullOrEmpty(videoUrl))
            {
                return BadRequest("Video URL is required.");
            }

            // Create HttpClient
            using (var client = new HttpClient())
            {
                try
                {
                    // Get the video content
                    var response = await client.GetAsync(videoUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var videoBytes = await response.Content.ReadAsByteArrayAsync();

                        // Set the folder path to save the video
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "created_videos");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Create a unique filename for the video
                        var fileName = $"{Guid.NewGuid()}.mp4";
                        var filePath = Path.Combine(folderPath, fileName);

                        // Save the video file
                        await System.IO.File.WriteAllBytesAsync(filePath, videoBytes);

                        // Return the path to the downloaded video
                        var downloadUrl = $"/downloads/{fileName}";
                        return Ok(new { message = "Video downloaded successfully", url = downloadUrl });
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Failed to download video.");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        // GET: api/video/token
        [HttpGet("token")]
        public IActionResult GetToken()
        {
            try
            {
                string token = _jwtService.Sign();
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating token: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    
    public class VideoGenerationRequest
    {
        
        public string Prompt { get; set; } = "";
        public string Image { get; set; } =""; // Only the filename, e.g., "myImage.jpg"
        public string ImageTail { get; set; } ="";
        public string NegativePrompt { get; set; } ="";
        public float CfgScale { get; set; } = 0.5f; 
        public string Mode { get; set; } = "std";
        public string StaticMask { get; set; } ="";
        public dynamic[] DynamicMasks { get; set; } = [];
        public string Duration { get; set; } = "5";
        public string CallbackUrl { get; set; } ="";
        public string ExternalTaskId { get; set; } ="";
    }

    public class VideoUrl{
        public string videoUrl { get; set; } = "";
    }
}

//1st try : SUCCEEDED;
//{"code":0,"message":"SUCCEED","request_id":"CjI17WevH1IAAAAAAHmgsw","data":{"task_id":"CjI17WevH1IAAAAAAHmgsw","task_status":"submitted","created_at":1739744384259,"updated_at":1739744384259}}

//created video;
//{"code":0,"message":"SUCCEED","request_id":"CjnVU2evHRAAAAAAAKDHAg","data":{"task_id":"CjI17WevH1IAAAAAAHmgsw","task_status":"succeed","task_status_msg":"","task_result":{"videos":[{"id":"28444a6c-b923-4d7c-be2c-e0898ea73dc0","url":"https://cdn.klingai.com/bs2/upload-kling-api/5739745938/image2video/CjI17WevH1IAAAAAAHmgsw-0_raw_video_1.mp4","duration":"5.1"}]},"created_at":1739744384259,"updated_at":1739744621359}}
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
        private readonly VideoGenerationService _videoService;
        private readonly JWTtoken _jwtService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(VideoGenerationService videoService, JWTtoken jwtService, ILogger<VideoController> logger)
        {
            _videoService = videoService;
            _jwtService = jwtService;
            _logger = logger;
        }

        // POST: api/video
        [HttpPost]
        public async Task<IActionResult> GenerateVideo([FromBody] VideoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt is required.");
            }

            try
            {
                var token = _jwtService.Sign();  // Get the JWT token
                var videoResponse = await _videoService.GenerateVideo(token, request.Prompt);  // Get the API response
                return Ok(new { Message = "Video generation request was successful.", VideoResponse = videoResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating video: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
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

    public class VideoRequest
    {
        public string Prompt { get; set; }
    }
}

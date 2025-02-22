using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;


namespace MyRazorApp.Website.API.ServerController
{
    [Route("api/server")]
    [ApiController]
    [EnableCors]
    public class ServerController : ControllerBase
    {

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImagesAsync([FromForm] IFormFile file1, [FromForm] IFormFile file2, [FromForm] string prompt)
        {
            if (file1 == null || file1.Length == 0)
            {
                return BadRequest("No first image uploaded.");
            }

            if (file2 == null || file2.Length == 0)
            {
                return BadRequest("No second image uploaded.");
            }

            // Define the upload directory
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Media/UploadedPhotos");
            Directory.CreateDirectory(uploadDir);

            // Save first image
            var fileName1 = Path.GetFileName(file1.FileName);
            var savePath1 = Path.Combine(uploadDir, fileName1);
            using (var fileStream1 = new FileStream(savePath1, FileMode.Create))
            {
                await file1.CopyToAsync(fileStream1);
            }

            // Save second image
            var fileName2 = Path.GetFileName(file2.FileName);
            var savePath2 = Path.Combine(uploadDir, fileName2);
            using (var fileStream2 = new FileStream(savePath2, FileMode.Create))
            {
                await file2.CopyToAsync(fileStream2);
            }

            // Return confirmation
            return Ok(new
            {
                Message = "Files uploaded successfully.",
                File1Name = fileName1,
                File2Name = fileName2,
                Prompt = prompt
            });
        }

        [HttpGet("stream/{fileName}")]
        public IActionResult StreamVideo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadVideo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "video/mp4", fileName);
        }

        [HttpGet("latest-video")]
        public IActionResult GetLatestVideo()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos");

            if (!Directory.Exists(filePath))
                return NotFound(new { message = "No videos found." });

            var latestVideo = new DirectoryInfo(filePath)
                      .GetFiles("*.mp4")
                      .OrderByDescending(f => f.CreationTime)
                      .FirstOrDefault(); // Get the latest file

if (latestVideo == null)
{
    return NotFound("No video files found.");
}
            return Ok(new { fileName = latestVideo.Name });
        }
    }
}

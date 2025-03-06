using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using MyRazorApp.Website.API.Models;
using Newtonsoft.Json;
using System.Text;


namespace MyRazorApp.Website.API.ServerController
{
    [Route("api/server")]
    [ApiController]
    [EnableCors]
    public class ServerController : ControllerBase
    {

        // [HttpPost("upload")]
        // public async Task<IActionResult> UploadImagesAsync([FromForm] IFormFile file1, [FromForm] IFormFile file2, [FromForm] string prompt)
        // {
        //     if (file1 == null || file1.Length == 0)
        //     {
        //         return BadRequest("No first image uploaded.");
        //     }

        //     if (file2 == null || file2.Length == 0)
        //     {
        //         Console.WriteLine("IMAGE 2 NOT UPLOADED");
        //         return BadRequest("No second image uploaded.");
        //     }

        //     // Define the upload directory
        //     var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Media/UploadedPhotos");
        //     Directory.CreateDirectory(uploadDir);

        //     // Save first image
        //     var fileName1 = Path.GetFileName(file1.FileName);
        //     var savePath1 = Path.Combine(uploadDir, fileName1);
        //     using (var fileStream1 = new FileStream(savePath1, FileMode.Create))
        //     {
        //         await file1.CopyToAsync(fileStream1);
        //     }

        //     // Save second image
        //     var fileName2 = Path.GetFileName(file2.FileName);
        //     var savePath2 = Path.Combine(uploadDir, fileName2);
        //     using (var fileStream2 = new FileStream(savePath2, FileMode.Create))
        //     {
        //         await file2.CopyToAsync(fileStream2);
        //     }

        //     // Return confirmation
        //     return Ok(new
        //     {
        //         Message = "Files uploaded successfully.",
        //         File1Name = fileName1,
        //         File2Name = fileName2,
        //         Prompt = prompt
        //     });
        // }

      [HttpPost("upload")]
        public async Task<IActionResult> UploadImagesAsync([FromForm] IFormFile file1, [FromForm] string prompt, [FromForm] IFormFile file2 = null)
        {
            if (file1 == null || file1.Length == 0)
            {
                return BadRequest("No first image uploaded.");
            }

            try
            {
                // Define the upload directory
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Media", "UploadedPhotos");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Function to generate a unique file name (avoid duplicates)
                string GetUniqueFileName(string originalName)
                {
                    var fileExt = Path.GetExtension(originalName);
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalName);
                    return $"{fileNameWithoutExt}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{fileExt}";
                }

                // Save first image
                var uniqueFileName1 = GetUniqueFileName(file1.FileName);
                var savePath1 = Path.Combine(uploadDir, uniqueFileName1);
                
                using (var fileStream1 = new FileStream(savePath1, FileMode.Create))
                {
                    await file1.CopyToAsync(fileStream1);
                }

                // Process second image (if provided)
                string uniqueFileName2 = null;
                if (file2 != null && file2.Length > 0)
                {
                    uniqueFileName2 = GetUniqueFileName(file2.FileName);
                    var savePath2 = Path.Combine(uploadDir, uniqueFileName2);
                    
                    using (var fileStream2 = new FileStream(savePath2, FileMode.Create))
                    {
                        await file2.CopyToAsync(fileStream2);
                    }
                }

                // Return confirmation with updated file names
                return Ok(new
                {
                    Message = "Files uploaded successfully.",
                    File1Name = uniqueFileName1,
                    File2Name = uniqueFileName2, // Null if second image wasn't uploaded
                    Prompt = prompt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("get-latest-images")]
        public IActionResult GetLatestImages()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Media/UploadedPhotos");
            var imageFiles = Directory.GetFiles(folderPath)
                                    .OrderByDescending(f => new FileInfo(f).CreationTime)
                                    .Take(2)
                                    .ToList();

            if (imageFiles.Count < 2)
            {
                return NotFound("Not enough images found.");
            }

            var fileName1 = Path.GetFileName(imageFiles[0]);
            var fileName2 = Path.GetFileName(imageFiles[1]);

            var encodedFileName1 = Uri.EscapeDataString(fileName1);
            var encodedFileName2 = Uri.EscapeDataString(fileName2);

            // Ensure correct URLs for both local and deployed environments
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                Image1Url = $"{baseUrl}/Media/UploadedPhotos/{encodedFileName1}",
                Image2Url = $"{baseUrl}/Media/UploadedPhotos/{encodedFileName2}"
            });
        }




        [HttpGet("stream/{fileName}")]
        public IActionResult StreamVideo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var streamUrl = $"{Request.Scheme}://{Request.Host}/api/server/stream-file/{fileName}";

            return Ok(new { url = streamUrl });
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadVideo(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var downloadUrl = $"{Request.Scheme}://{Request.Host}/api/server/download-file/{fileName}";
            return Ok(new { url = downloadUrl });
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



        [HttpGet("download-file/{fileName}")]
        public IActionResult DownloadVideoFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "video/mp4", fileName);
        }

        [HttpGet("stream-file/{fileName}")]
        public IActionResult StreamVideoFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media/CreatedVideos", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Video not found." });

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }
                

    }
}

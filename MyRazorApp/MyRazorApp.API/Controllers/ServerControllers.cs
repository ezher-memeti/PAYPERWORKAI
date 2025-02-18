using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;


namespace MyRazorApp.MyRazorApp.API.ServerController
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

        [HttpGet("test")]
public IActionResult TestEndpoint()
{
    return Ok("Server is working");
}
    }
}

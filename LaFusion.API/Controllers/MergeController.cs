using System.Net.Mime;
using LaFusion.PkgeCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaFusion.Api.Controllers
{

    [Route("LaFusion/[controller]")]
    [ApiController]
    public class MergeController : ControllerBase
    {
        private readonly ILogger<MergeController> _logger;

        // Dependency injection for logging
        public MergeController(ILogger<MergeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Merges multiple PDF files uploaded by the user into a single document.
        /// </summary>
        /// <param name="files">A collection of PDF files to be merged (multipart/form-data).</param>
        /// <returns>The merged PDF document as a file stream.</returns>
        
        [HttpPost("merge")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MergePdfs([FromForm] List<IFormFile> files)
        {
            // 1. Input Validation
            if (files == null || files.Count == 0 || files.Any(f => f.Length == 0))
            {
                return BadRequest(new { Message = "No valid files uploaded. Please provide at least one PDF file." });
            }

            // 2. Data Preparation for Core Library
            var pdfStreams = new List<MemoryStream>();
            try
            {
                foreach (var file in files)
                {
                    // Copy IFormFile content into a MemoryStream required by the Core library
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    pdfStreams.Add(stream);
                }

                // 3. Core Fusion Logic (Single call to the NuGet Library logic)
                using (var mergedStream = PdfMerger.Merge(pdfStreams))
                {
                    // 4. Prepare Output
                    var fileBytes = mergedStream.ToArray();
                    var fileName = $"LaFusion_Merged_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    // Return the file content
                    return File(
                        fileBytes,
                        MediaTypeNames.Application.Pdf,
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during PDF merging.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An internal error occurred while processing the PDF files." });
            }
            finally
            {
                // Ensure all streams are cleaned up
                foreach (var stream in pdfStreams)
                {
                    stream.Dispose();
                }
            }
        }

    }
}
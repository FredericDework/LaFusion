using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Net.Mime;

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
            if (files == null || files.Count == 0)
            {
                _logger.LogWarning("Merge request failed: No files were uploaded.");
                return BadRequest(new { Message = "No files uploaded. Please provide at least one PDF file for merging." });
            }

            // Ensure all files are actually PDF files (basic check, could be enhanced)
            if (files.Any(f => !f.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Merge request failed: Non-PDF file detected.");
                return BadRequest(new { Message = "All uploaded files must be in PDF format." });
            }

            // 2. Core Fusion Logic
            try
            {
                // Create the final output document
                PdfDocument outputDocument = new PdfDocument();

                foreach (var file in files)
                {
                    // Use a MemoryStream to read the uploaded file content
                    using (var stream = new MemoryStream())
                    {
                        // Copy the uploaded file stream to the memory stream
                        await file.CopyToAsync(stream);
                        stream.Position = 0; // Reset position to the beginning for reading

                        // Use PdfSharp's PdfReader to open the source document from the stream
                        using (PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                        {
                            // Iterate through all pages of the source document
                            // This ensures that pages with images/scans are correctly imported
                            for (int i = 0; i < inputDocument.PageCount; i++)
                            {
                                // Import the page from the input document to the output document
                                outputDocument.AddPage(inputDocument.Pages[i]);
                            }
                        }
                    }
                    _logger.LogInformation($"Successfully imported {file.FileName} with {outputDocument.PageCount} pages total.");
                }

                // 3. Prepare Output
                // Use a MemoryStream to save the resulting PDF document
                using (var outputStream = new MemoryStream())
                {
                    outputDocument.Save(outputStream, false);
                    outputStream.Position = 0;

                    // Convert the stream to a byte array to return as a file
                    var fileBytes = outputStream.ToArray();

                    // Set the file name for the download
                    var fileName = $"LaFusion_Merged_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    // Return the file content.
                    // ContentType is set to "application/pdf"
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
        }
    }
}
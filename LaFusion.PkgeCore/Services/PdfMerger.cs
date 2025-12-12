using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;

namespace LaFusion.PkgeCore.Services
{
    public static class PdfMerger
    {

        /// <summary>
        /// Merges a collection of input PDF streams into a single output stream.
        /// </summary>
        /// <param name="pdfStreams">A list of MemoryStream objects, each containing the content of a PDF file.</param>
        /// <returns>A MemoryStream containing the resulting merged PDF document.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the input list of streams is null or empty.</exception>
        /// <exception cref="PdfReaderException">Thrown if any of the input streams is not a valid PDF.</exception>
        public static MemoryStream Merge(List<MemoryStream> pdfStreams)
        {
            if (pdfStreams == null || pdfStreams.Count == 0)
            {
                throw new ArgumentNullException(nameof(pdfStreams), "The list of PDF streams cannot be null or empty.");
            }

            // Create the final output document
            using (PdfDocument outputDocument = new PdfDocument())
            {
                // Process each input stream
                foreach (var stream in pdfStreams)
                {
                    stream.Position = 0; // Ensure stream is read from the beginning

                    // Open the source document from the stream in Import mode
                    using (PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                    {
                        // Iterate through all pages of the source document and add them to the output
                        for (int i = 0; i < inputDocument.PageCount; i++)
                        {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                        }
                    }
                }

                // Save the resulting PDF document to an output stream
                var outputStream = new MemoryStream();
                outputDocument.Save(outputStream, false);
                outputStream.Position = 0; // Reset position for reading/return

                return outputStream;
            }
        }

    }
}

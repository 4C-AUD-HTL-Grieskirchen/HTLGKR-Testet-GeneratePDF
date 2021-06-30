


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Cloud.Firestore.V1;
using Google.LongRunning;
using IronPdf;
using iText.Html2pdf;
using iText.Html2pdf.Attach.Impl.Layout;
using iText.Kernel.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PageSize = iText.Kernel.Geom.PageSize;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;


namespace PDFGenerationService
{
    public class PdfManager
    {
        public void GeneratePDF(Dictionary<string, object> dict)
        {

            string html = File.ReadAllText("PDF/pdf.html");

            foreach (var (key, value) in dict)
            {
                html = html.Replace("{{" + key + "}}", value.ToString());
            }
            
            var renderer = new IronPdf.HtmlToPdf();
            
            var pdfDoc = renderer.RenderHtmlAsPdf(html, "./PDF");
            
            pdfDoc.SaveAs("out.pdf");

        }
    }
}

using System.Collections.Generic;
using System.IO;
using IronPdf;

namespace PDFGenerationService
{
    public class PdfManager
    {
        public void GeneratePDF(Dictionary<string, object> dict)
        {
            var html = File.ReadAllText("PDF/pdf.html");

            foreach (var (key, value) in dict)
            {
                if (value == null) continue;
                
                html = html.Replace("{{" + key + "}}", value.ToString());
            }

            var renderer = new HtmlToPdf();

            var pdfDoc = renderer.RenderHtmlAsPdf(html, Path.GetFullPath("PDF"));

            pdfDoc.SaveAs("out.pdf");
            
        }
    }
}
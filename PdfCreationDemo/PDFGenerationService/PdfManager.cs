using System.Collections.Generic;
using System.IO;
using IronPdf;

namespace PDFGenerationService
{
    public class PdfManager
    {
        public void GeneratePDF(Dictionary<string, object> dict, bool isRegistrationPdf)
        {
            var html = isRegistrationPdf ? File.ReadAllText("Registration-PDF/pdf.html") : File.ReadAllText("Result-PDF/pdf.html");

            foreach (var (key, value) in dict)
            {
                if (value == null) continue;
                
                html = html.Replace("{{" + key + "}}", value.ToString());
            }

            var renderer = new HtmlToPdf();

            var pdfDoc = renderer.RenderHtmlAsPdf(html, isRegistrationPdf ? Path.GetFullPath("Registration-PDF") : Path.GetFullPath("Result-PDF"));

            pdfDoc.SaveAs("out.pdf");
            
        }
    }
}
using System.Collections.Generic;
using System.IO;



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

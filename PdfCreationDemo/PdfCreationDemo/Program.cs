using iText.Kernel.Pdf;
using iText.Layout;
using iTextSharp.text;
using System;
using System.Security.AccessControl;
using iTextSharp.text.pdf;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using Document = iTextSharp.text.Document;
using System.IO;
using System.Security.AccessControl;
using iText.Layout.Properties;
using iText.IO.Image;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using System.Drawing.Imaging;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Firebase.Storage;
using FireSharp;

namespace PdfCreationDemo 
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Program_Load();
            p.GeneratePDF();
            p.pushFirebaseAsync();

        }

        public void Program_Load()
        {

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "AIzaSyB04YQ9Xv7ZHyMjUNvu3iRwRm-rxrXXMsg",
                BasePath = "https://htlgkr-testet-default-rtdb.firebaseio.com",

            };

            IFirebaseClient client;

            client = new FireSharp.FirebaseClient(config);
        }

        public void GeneratePDF()
        {
            
            var path = @"C:\Users\Julia\Desktop\AUD-PDFCreation\PdfCreation\PdfCreationDemo\pdfcreated.pdf";

            
            Document pdfDocument = new Document();

            PdfWriter.GetInstance(pdfDocument,
                    new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
            pdfDocument.Open();

            //Logo
            var imagepathLogo = @"C:\Users\Julia\Desktop\AUD-PDFCreation\PdfCreation\Bilder\logo.png";
            using(FileStream fs = new FileStream(imagepathLogo, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(8f);
                
                png.SetAbsolutePosition(pdfDocument.Left, pdfDocument.Top);
                pdfDocument.Add(png);

            }

            using (FileStream fs = new FileStream(imagepathLogo, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(8f);

                png.SetAbsolutePosition(pdfDocument.Right, pdfDocument.Top);
                pdfDocument.Add(png);

            }

            //TopText
            iTextSharp.text.Font myText = FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD);
            Paragraph TopText = new Paragraph("Aktion Österreich testet", myText);
            pdfDocument.Add(TopText);
            pdfDocument.Add(new Paragraph("Einwilligungserklärung Antigen-Test", myText));
            pdfDocument.Add(new Paragraph("SARS-CoV-2 / Covid-19", myText));

            pdfDocument.Add(new Paragraph("   ")); //Empty Space
            pdfDocument.Add(new Paragraph("   ")); //Empty Space
            pdfDocument.Add(new Paragraph("Zu testende Person"));
            pdfDocument.Add(new Paragraph("   ")); //Empty Space


            //Table
            PdfPTable table = new PdfPTable(4);
            PdfPCell cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            table.AddCell("Name:        ");//table.AddCell("Col 1 Row 1");
            table.AddCell("             ");//table.AddCell("Col 2 Row 1");

            table.AddCell("Geburtsdatum: ");//table.AddCell("Col 3 Row 1");
            table.AddCell("             ");//table.AddCell("Col 1 Row 2");

            table.AddCell("             ");//table.AddCell("Col 2 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("Geschlecht:  ");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("Vorname      ");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("SV-Nummer(10-stellig): ");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("PLZ, Ort:    ");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("Mobiltelefonnummer:    ");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("Straße, Hausnummer:");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            table.AddCell("E-Mail-Adresse:");//table.AddCell("Col 3 Row 2");
            table.AddCell("             ");//table.AddCell("Col 3 Row 2");

            pdfDocument.Add(table);


            //TestergebnisBottom
            var imagepathTEB = @"C:\Users\Julia\Desktop\AUD-PDFCreation\PdfCreation\Bilder\TestergebnisFoto.png";
            using (FileStream fs = new FileStream(imagepathTEB, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(55f); //30f unten

                png.SetAbsolutePosition(pdfDocument.Bottom, pdfDocument.Bottom);
                pdfDocument.Add(png);

            }

            pdfDocument.Close(); 



        }

        public async System.Threading.Tasks.Task pushFirebaseAsync()
        {
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"C:\Users\Julia\Desktop\AUD-PDFCreation\PdfCreation\PdfCreationDemo\pdfcreated.pdf", FileMode.Open);

            // Construct FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(@"pdf-creation-56da4.appspot.com")
                .Child("data")
                .Child("pdf")
                .Child("pdfcreated.pdf")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

        }
    }
}

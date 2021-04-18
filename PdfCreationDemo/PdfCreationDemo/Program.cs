using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Firebase.Auth;
using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using FirebaseConfig = FireSharp.Config.FirebaseConfig;

namespace PdfCreationDemo
{
    public class Program
    {
        private static string FIREBASE_APPKEY;

        private static void Main(string[] args)
        {
            

            var p = new Program();
            p.Program_Load();
            p.GeneratePDF();
            p.PushFirebaseAsync(Config.Read()).Wait(10000);
        }

        public void Program_Load()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "AIzaSyB04YQ9Xv7ZHyMjUNvu3iRwRm-rxrXXMsg",
                BasePath = "https://htlgkr-testet-default-rtdb.firebaseio.com"
            };

            IFirebaseClient client;

            client = new FirebaseClient(config);
        }

        public void GeneratePDF()
        {
            var path = @"pdfcreated.pdf";


            var pdfDocument = new Document();

            PdfWriter.GetInstance(pdfDocument,
                new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
            pdfDocument.Open();

            //Logo
            var imagepathLogo = @"Bilder\logo.png";
            using (var fs = new FileStream(imagepathLogo, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(8f);

                png.SetAbsolutePosition(pdfDocument.Left, pdfDocument.Top);
                pdfDocument.Add(png);
            }

            using (var fs = new FileStream(imagepathLogo, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(8f);

                png.SetAbsolutePosition(pdfDocument.Right, pdfDocument.Top);
                pdfDocument.Add(png);
            }

            //TopText
            var myText = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var TopText = new Paragraph("Aktion Österreich testet", myText);
            pdfDocument.Add(TopText);
            pdfDocument.Add(new Paragraph("Einwilligungserklärung Antigen-Test", myText));
            pdfDocument.Add(new Paragraph("SARS-CoV-2 / Covid-19", myText));

            pdfDocument.Add(new Paragraph("   ")); //Empty Space
            pdfDocument.Add(new Paragraph("   ")); //Empty Space
            pdfDocument.Add(new Paragraph("Zu testende Person"));
            pdfDocument.Add(new Paragraph("   ")); //Empty Space


            //Table
            var table = new PdfPTable(4);
            var cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            table.AddCell("Name:        "); //table.AddCell("Col 1 Row 1");
            table.AddCell("             "); //table.AddCell("Col 2 Row 1");

            table.AddCell("Geburtsdatum: "); //table.AddCell("Col 3 Row 1");
            table.AddCell("             "); //table.AddCell("Col 1 Row 2");

            table.AddCell("             "); //table.AddCell("Col 2 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("Geschlecht:  "); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("Vorname      "); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("SV-Nummer(10-stellig): "); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("PLZ, Ort:    "); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("Mobiltelefonnummer:    "); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("Straße, Hausnummer:"); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            table.AddCell("E-Mail-Adresse:"); //table.AddCell("Col 3 Row 2");
            table.AddCell("             "); //table.AddCell("Col 3 Row 2");

            pdfDocument.Add(table);


            //TestergebnisBottom
            var imagepathTEB = @"Bilder\TestergebnisFoto.png";
            using (var fs = new FileStream(imagepathTEB, FileMode.Open))
            {
                var png = Image.GetInstance(System.Drawing.Image.FromStream(fs),
                    ImageFormat.Png);
                png.ScalePercent(55f); //30f unten

                png.SetAbsolutePosition(pdfDocument.Bottom, pdfDocument.Bottom);
                pdfDocument.Add(png);
            }

            pdfDocument.Close();
        }

        public async Task PushFirebaseAsync(Config conf)
        {
            Console.WriteLine("Start upload ...");
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"pdfcreated.pdf", FileMode.Open);

            var authProvider =
                new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(conf.ApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(conf.Email, conf.Pw);
            var authOption = new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                ThrowOnCancel = true
            };

            // Construct FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(@"htlgkr-testet.appspot.com", authOption)
                .Child("pdf")
                .Child("pdfcreated.pdf")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;

            Console.WriteLine("URL: " + downloadUrl);
        }
    }
}
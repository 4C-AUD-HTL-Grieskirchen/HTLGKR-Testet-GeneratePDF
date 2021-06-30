using System;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace PDFGenerationService
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonString = File.ReadAllText("htlgkr-testet-firebase-adminsdk-s5lca-0cbf2afb33.json");

            var pdfManager = new PdfManager();
            var storageManager = new StorageManager(jsonString);

            

            var builder = new FirestoreClientBuilder { JsonCredentials = jsonString };
            var db = FirestoreDb.Create("htlgkr-testet", builder.Build());


            var coll = db.Collection("Registrations");

            var listener = coll.Listen(snapshot =>
            {
                foreach (var change in snapshot.Changes)
                {
                    try
                    {
                        var doc = change.Document;
                        var dict = doc.ToDictionary();

                        // Console.WriteLine($"Document ID: {doc.Id}");

                        string email = dict["email"] as string;
                        bool? created = dict["registrationPdfCreated"] as bool?;

                        if (!(created ?? true))
                        {
                            Console.WriteLine($"Generating email for " + email);
                            pdfManager.GeneratePDF(dict);
                            storageManager.UploadFile("out.pdf", doc.Id);

                            change.Document.Reference.UpdateAsync("registrationPdfCreated", true);

                        }

                    }
                    catch (KeyNotFoundException e)
                    {
                        // Console.WriteLine(e.Message);
                    }
                }
            });
            listener.ListenerTask.Wait();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace PDFGenerationService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jsonString = File.ReadAllText("htlgkr-testet-firebase-adminsdk-s5lca-0cbf2afb33.json");

            var pdfManager = new PdfManager();
            var storageManager = new StorageManager(jsonString);


            var builder = new FirestoreClientBuilder {JsonCredentials = jsonString};
            var db = FirestoreDb.Create("htlgkr-testet", builder.Build());


            var coll = db.Collection("Registrations");

            var listener = coll.Listen(snapshot =>
            {
                foreach (var change in snapshot.Changes)
                    try
                    {
                        var doc = change.Document;
                        var dict = doc.ToDictionary();
                        dict["doc_id"] = doc.Id;

                        var name = dict["lastname"] as string;
                        var createdRegistration = dict["registrationPdfCreated"] as bool?;
                        var createdResult = dict["resultPdfCreated"] as bool?;

                        if (!(createdRegistration ?? true))
                        {
                            Console.WriteLine("Generating registration-pdf for " + name);
                            pdfManager.GeneratePDF(dict, true);
                            storageManager.UploadFile("out.pdf", doc.Id, "registration-pdf");

                            change.Document.Reference.UpdateAsync("registrationPdfCreated", true);
                        }
                        
                        if (!(createdResult ?? true))
                        {
                            Console.WriteLine("Generating result-pdf for " + name);
                            pdfManager.GeneratePDF(dict, false);
                            storageManager.UploadFile("out.pdf", doc.Id, "result-pdf");

                            change.Document.Reference.UpdateAsync("resultPdfCreated", true);
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        // Console.WriteLine(e.Message);
                    }
            });
            listener.ListenerTask.Wait();
        }
    }
}
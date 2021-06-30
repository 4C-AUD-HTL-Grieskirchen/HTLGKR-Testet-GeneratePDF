using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace PDFGenerationService
{
    internal class StorageManager
    {
        private readonly StorageClient client;

        public StorageManager(string jsonString)
        {
            var creds = GoogleCredential.FromJson(jsonString);
            client = StorageClient.Create(creds);
        }

        public void UploadFile(string filename, string targetname)
        {
            client.UploadObject("htlgkr-testet.appspot.com", $"registration-pdf/{targetname}.pdf", null,
                File.Open(filename, FileMode.Open));
        }
    }
}
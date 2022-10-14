using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualsLib
{
    // Class to demonstrate use of Drive create folder API.
    public class CreateFolder
    {
        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <returns>created folder id, null otherwise</returns>
        public static string DriveCreateFolder()
        {
            try
            {
                /* Load pre-authorized user credentials from the environment.
                 TODO(developer) - See https://developers.google.com/identity for 
                 guides on implementing OAuth2 for your application. */
                GoogleCredential credential = GoogleCredential.GetApplicationDefault()
                    .CreateScoped(DriveService.Scope.Drive);

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Snippets"
                });

                // File metadata
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "Invoices",
                    MimeType = "application/vnd.google-apps.folder"
                };

                // Create a new folder on drive.
                var request = service.Files.Create(fileMetadata);
                request.Fields = "id";
                var file = request.Execute();
                // Prints the created folder id.
                Console.WriteLine("Folder ID: " + file.Id);
                return file.Id;
            }
            catch (Exception e)
            {
                // TODO(developer) - handle error appropriately
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else
                {
                    throw;
                }
            }
            return null;
        }
    }
}

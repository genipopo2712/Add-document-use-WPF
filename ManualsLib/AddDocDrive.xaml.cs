using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Net;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;

namespace ManualsLib
{
    /// <summary>
    /// Interaction logic for AddDocDrive.xaml
    /// </summary>
    public partial class AddDocDrive : Window
    {
        public string ThongBao = "";
        public AddDocDrive()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {
            string inewBrand = newBrand.Text;
            if (inewBrand !="")
            {
                Sub_Cloud_Create();
            }
            
        }
        private void Sub_Cloud_Create()
        {
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "ManualsLib";
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";/*System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);*/

                //credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            var request1 = service.Files.List();
            request1.Fields = "id";
            var parentPath = request1.Execute();
            
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = newBrand.Text,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentPath.Id },
            };
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            CheckResult.Text = file.Id;
        }
    }
}


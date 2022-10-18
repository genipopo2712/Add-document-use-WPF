using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Download;

namespace ManualsLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public string ThongBao = "";
        public string currPath = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string currpath = Directory.GetCurrentDirectory();
            currPath = currpath;
            Directory.Delete(currPath + "\\Temporary", true);
            if (!File.Exists(currPath + "\\Temporary"))
            {
                Directory.CreateDirectory(currPath + "\\Temporary");
                //get file Brand.txt from drive to local to load Brand name
                CheckResult.Text = Sub_Get_Id("Brand.txt", "txt");
                Sub_Download(Sub_Get_Id("Brand.txt", "txt"), "Brand.txt");
                Sub_Download(Sub_Get_Id("Template.html", "html"), "Template.html");
                Sub_Download(Sub_Get_Id("Index.html", "html"), "Index.html");
            }


        }
        private void Sub_Download(string fileId, string outputname)
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
            var request = service.Files.Get(fileId);
            var streamdownload = new MemoryStream();
            request.MediaDownloader.ProgressChanged += progress =>
            {
                if (progress.Status == DownloadStatus.Completed)
                {
                    using (FileStream fs = new FileStream(currPath + "\\Temporary\\" + outputname, FileMode.Create))
                    {
                        streamdownload.WriteTo(fs);
                        fs.Flush();
                    }
                }
            };
            request.Download(streamdownload);
        }

        private string Sub_Get_Id(string foldername, string filetype)
        {
            string mimetype = "";
            switch (filetype)
            {
                case "html":
                    mimetype = "text/html";
                    break;
                case "txt":
                    mimetype = "text/plain";
                    break;
                case "pdf":
                    mimetype = "application/pdf";
                    break;
                case "doc":
                    mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "docx":
                    mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "xlsx":
                    mimetype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "xlsm":
                    mimetype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheetl";
                    break;
                case "jpeg":
                    mimetype = "image/jpeg";
                    break;
                case "png":
                    mimetype = "image/png";
                    break;
            }

            string folderId = "";
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
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"mimeType ='{mimetype}' and name = '{foldername}' and parents='{Sub_Get_Id("ManualDocument")}' ";
            try
            {
                IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            .Files;

                if (files.Count > 0)
                {
                    folderId = listRequest.Execute().Files[0].Id;

                }
                else
                {
                    folderId = "File not found.";
                }
            }
            catch (Exception ex)
            {
                folderId = ex.Message;
            }
            return folderId;
        }
        private string Sub_Get_Id(string foldername)
        {

            string folderId = "";
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
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"mimeType ='application/vnd.google-apps.folder' and name = '{foldername}' ";
            try
            {
                IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            .Files;

                if (files.Count > 0)
                {
                    folderId = listRequest.Execute().Files[0].Id;

                }
                else
                {
                    folderId = "File not found.";
                }
            }
            catch (Exception ex)
            {
                folderId = ex.Message;
            }
            return folderId;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            AddDocument addDocument = new AddDocument();
            addDocument.Show();
            this.Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EditDocument editDocument = new EditDocument();
            editDocument.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddDocDrive addDocDrive = new AddDocDrive();
            addDocDrive.Show();
            this.Close();
        }

    }
}

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
            MessageBox.Show("Downloading content, please wait....");
            using (stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            currPath = Directory.GetCurrentDirectory();
            parentIds = "1V1JrUEmwBvrEWNyr8IpAXl09Z8VFZdjO";
            brandIds = Sub_Get_Id(parentIds, "Brand.txt", "txt");
            templateIds = Sub_Get_Id(parentIds, "Template.html", "html");
            indexIds = Sub_Get_Id(parentIds, "Index.html", "html");
            if (Directory.Exists(currPath + "\\Temporary"))
            {
                Directory.Delete(currPath + "\\Temporary", true);
                Directory.CreateDirectory(currPath + "\\Temporary");
            }
            else
            {
                Directory.CreateDirectory(currPath + "\\Temporary");
            }
            Sub_Download(brandIds, "Brand.txt");
            Sub_Download(templateIds, "Template.html");
            Sub_Download(indexIds, "Index.html");
            var listBrandhtml = Sub_Get_List_Drive(Sub_Get_Id("Databases"));
            foreach (var it in listBrandhtml)
            {
                Sub_Download(Sub_Get_Id(it + ".html", "html"), it + ".html");
            }
            if (listBrandhtml != null && !string.IsNullOrEmpty(listBrandhtml[0]))
            {
                CheckResult.Text = listBrandhtml[0];
            }

            MessageBox.Show("Download completed.");


        }
        public string ThongBao = "";
        public string currPath = "";
        public DriveService service;
        public string folderId = "";
        public string brandIds = "";
        public string templateIds = "";
        public string parentIds = "";
        public string indexIds = "";
        public FileStream stream;
        public UserCredential credential;
        public string[] Scopes = { DriveService.Scope.Drive };
        public string ApplicationName = "ManualsLib";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            using (stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            currPath = Directory.GetCurrentDirectory();
            parentIds = "1V1JrUEmwBvrEWNyr8IpAXl09Z8VFZdjO";
            brandIds = Sub_Get_Id(parentIds, "Brand.txt", "txt");
            templateIds = Sub_Get_Id(parentIds, "Template.html", "html");
            indexIds = Sub_Get_Id(parentIds, "Index.html", "html");
            if (Directory.Exists(currPath + "\\Temporary"))
            {
                Directory.Delete(currPath + "\\Temporary", true);
                Directory.CreateDirectory(currPath + "\\Temporary");
            }
            else
            {
                Directory.CreateDirectory(currPath + "\\Temporary");
            }
        }

        private void Sub_Download(string fileId, string outputname)
        {
            //string[] Scopes = { DriveService.Scope.Drive };
            //string ApplicationName = "ManualsLib";
            //UserCredential credential;
            //using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            //{
            //    string credPath = "token.json";/*System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);*/

            //    //credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //}
            //var service = new DriveService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName,
            //});
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
                string credPath = "token.json";

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
            listRequest.Q = $"mimeType ='{mimetype}' and name = '{foldername}' ";
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
        private string Sub_Get_Id(string parent, string foldername, string filetype)
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

            //string folderId = "";
            //string[] Scopes = { DriveService.Scope.Drive };
            //string ApplicationName = "ManualsLib";
            //UserCredential credential;
            //using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            //{
            //    string credPath = "token.json";

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //}
            //var service = new DriveService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName,
            //});
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"mimeType ='{mimetype}' and name = '{foldername}' and parents='{parent}' ";
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


            return folderId;
        }
        private string Sub_Get_Id(string foldername)
        {

            //string folderId = "";
            //string[] Scopes = { DriveService.Scope.Drive };
            //string ApplicationName = "ManualsLib";
            //UserCredential credential;
            //using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            //{
            //    string credPath = "token.json";

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //}
            //var service = new DriveService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName,
            //});
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
        private string[] Sub_Get_List_Drive(string parent)
        {
            string[] folderIds;
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"mimeType ='application/vnd.google-apps.folder' and parents='{parent}' ";
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
       .Files;
            if (files.Count > 0)
            {
                folderIds = new string[listRequest.Execute().Files.Count];
                for (int i = 0; i < folderIds.Length; i++)
                {
                    folderIds[i] = listRequest.Execute().Files[i].Name;
                }

            }
            else
            {
                folderIds = new string[] { "Database empty." };
            }
            return folderIds;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Sub_Download(brandIds, "Brand.txt");
            //Sub_Download(templateIds, "Template.html");
            //Sub_Download(indexIds, "Index.html");
            EditDocument editDocument = new EditDocument();
            editDocument.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //get file Brand.txt from drive to local to load Brand name
            //Sub_Download(brandIds, "Brand.txt");
            //Sub_Download(templateIds, "Template.html");
            //Sub_Download(indexIds, "Index.html");
            AddDocDrive addDocDrive = new AddDocDrive();
            addDocDrive.Show();
            this.Close();
        }


    }
}

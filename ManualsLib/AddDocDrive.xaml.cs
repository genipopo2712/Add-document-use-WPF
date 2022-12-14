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
using Google.Apis.Drive.v3.Data;
using Microsoft.Win32;
using File = System.IO.File;
using Google.Apis.Download;
using System.Text.RegularExpressions;

namespace ManualsLib
{
    /// <summary>
    /// Interaction logic for AddDocDrive.xaml
    /// </summary>
    public partial class AddDocDrive : Window
    {
        public string folderId = "";
        public string currPath = Directory.GetCurrentDirectory();
        public string pathSource = "";
        public string ThongBao = "";
        public List<string> ParentPath = new List<string>();
        public string ParentId = "";
        public string templateDoc = "";
        public DriveService service;
        public string[] Scopes = { DriveService.Scope.Drive };
        public string ApplicationName = "ManualsLib";
        public FileStream stream;
        public UserCredential credential;
        public AddDocDrive()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SubMainWindow subMainWindow = new SubMainWindow();
            subMainWindow.Show();
            this.Close();
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            //this.Close();
        }

        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {

            string outputBrand = "";
            string kq = "File not found.";
            kq = Sub_Get_Id(newBrand.Text);
            if (kq != "File not found.")
            {
                ThongBao = "This brand name already exsist.";
            }
            else
            {
                var tempPath = new List<string> { Sub_Get_Id("Databases") };
                ParentPath = tempPath;
                string inewBrand = newBrand.Text;
                if (inewBrand != "")
                {
                    Sub_Cloud_Create(newBrand.Text);
                }
                string folderName = newBrand.Text;
                string inputBrand = File.ReadAllText(currPath + "\\Temporary\\" + "Template.html");
                outputBrand = inputBrand.Replace("brandnameheader", folderName);
                File.WriteAllText(currPath + "\\Temporary\\" + folderName + ".html", outputBrand);
                Sub_Upload(currPath + "\\Temporary\\" + folderName + ".html", Sub_Get_Id(folderName), folderName + ".html", "html");
                string inputIndex = File.ReadAllText(currPath + "\\Temporary\\" + "Brand.txt");
                string outputIndex = inputIndex + folderName + ";";
                File.WriteAllText(currPath + "\\Temporary\\" + "Brand.txt", outputIndex);
                Sub_Delete_Drive(Sub_Get_Id("Brand.txt", "txt"));
                Sub_Upload(currPath + "\\Temporary\\" + "Brand.txt", "1V1JrUEmwBvrEWNyr8IpAXl09Z8VFZdjO", "Brand.txt", "txt");
                ThongBao = "Added folder database for " + newBrand.Text;
                do
                {
                    Sub_Window_Loaded();
                } while (!File.Exists(currPath + "\\Temporary\\" + "Brand.txt"));

            }
            CheckResult.Text = ThongBao;
        }
        private void desPdf_Click(object sender, RoutedEventArgs e)
        {
            //Choose file pdf from
            OpenFileDialog openDesPdf = new OpenFileDialog();
            openDesPdf.DefaultExt = ".pdf";
            openDesPdf.Filter = "Text Document (.pdf)|*.pdf";
            if (openDesPdf.ShowDialog() == true)
            {
                string filePath = openDesPdf.FileName;
                desPdfPath.Text = filePath;
                pathSource = filePath;
            }

        }

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
            SubloadTemplate();
            Sub_Window_Loaded();
        }
        private void Sub_Cloud_Create(string name)
        {
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
            //var request1 = service.Files.List();
            //request1.Fields = "id";
            //var parentPath = request1.Execute();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = ParentPath,
            };
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            CheckResult.Text = file.Id;
        }
        //private string Sub_Get_Id(string foldername, string parentid)
        //{

        //    string[] Scopes = { DriveService.Scope.Drive };
        //    string ApplicationName = "ManualsLib";
        //    UserCredential credential;
        //    using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        //    {
        //        string credPath = "token.json";/*System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);*/

        //        //credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            Scopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //    }
        //    var service = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });
        //    //var request1 = service.Files.List();
        //    //request1.Fields = "id";
        //    //var parentPath = request1.Execute();

        //    FilesResource.ListRequest listRequest = service.Files.List();
        //    listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{foldername}' and parents = '{parentid}'";

        //    IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
        //    .Files;
        //    string folderId = listRequest.Execute().Files[0].Id;
        //    return folderId;
        //}
        //private string Sub_Get_Id(string foldername)
        //{
        //    string[] Scopes = { DriveService.Scope.Drive };
        //    string ApplicationName = "ManualsLib";
        //    UserCredential credential;
        //    using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        //    {
        //        string credPath = "token.json";/*System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);*/

        //        //credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            Scopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //    }
        //    var service = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });
        //    //var request1 = service.Files.List();
        //    //request1.Fields = "id";
        //    //var parentPath = request1.Execute();

        //    FilesResource.ListRequest listRequest = service.Files.List();
        //    listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{foldername}'";

        //    IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
        //    .Files;
        //    string folderId = listRequest.Execute().Files[0].Id;
        //    return folderId;
        //} not use replace by Sub_Get_Id(string foldername, string filetype) and Sub_Get_Id(string foldername)
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
        //private void Sub_Upload(string name)
        //{
        //    string[] Scopes = { DriveService.Scope.Drive };
        //    string ApplicationName = "ManualsLib";
        //    UserCredential credential;
        //    using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        //    {
        //        string credPath = "token.json";

        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            Scopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //    }
        //    var service = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });
        //    var FileMetaData = new Google.Apis.Drive.v3.Data.File()
        //    {
        //        Parents = ParentPath,
        //        Name = name
        //    };
        //    FilesResource.CreateMediaUpload request;

        //    using (var stream = new FileStream(pathSource, System.IO.FileMode.Open))
        //    {
        //        request = service.Files.Create(FileMetaData, stream, "application/pdf");
        //        request.Fields = "id";
        //        request.Upload();
        //    }
        //    ThongBao = "Upload completed file: ";
        //}
        private void Sub_Upload(string pathsource, string destination, string name, string filetype)
        {
            var parents = new List<string> { destination };
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

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {

                Parents = parents,
                Name = name
            };
            FilesResource.CreateMediaUpload request;

            using (var stream = new FileStream(pathsource, System.IO.FileMode.Open))
            {
                request = service.Files.Create(FileMetaData, stream, mimetype);
                request.Upload();
            }
            ThongBao = "Upload completed file: ";
        }
        private void Sub_Read(string foldername)
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
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Q = $"'text/html' and name = '{foldername}' and parents = '1V1JrUEmwBvrEWNyr8IpAXl09Z8VFZdjO'";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            .Files;

        }
        private void Sub_Delete_Drive(string fileId)
        {
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

            var request = service.Files.Delete(fileId);
            request.Execute();
        }

        private void SubloadTemplate()
        {

            string doc = "<!--template add new document-->";
            string tempAddBrand = File.ReadAllText(currPath + "\\Temporary\\Template.html");
            string tempAddDoc = tempAddBrand.Substring(tempAddBrand.IndexOf(doc) + doc.Length + 6, tempAddBrand.LastIndexOf(doc) - tempAddBrand.IndexOf(doc) - doc.Length - 11);
            templateDoc = tempAddDoc;
        }
        private void Sub_Window_Loaded()
        {
            string brandList = "";
            string filename = currPath + "\\Temporary\\Brand.txt";
            string localBrand = File.ReadAllText(filename);
            var arrlocalBrand = localBrand.Split(';').ToList();
            arrlocalBrand.Remove("");
            brandName.ItemsSource = arrlocalBrand.ToList();
            if (string.IsNullOrEmpty(localBrand))
            {
                brandName.IsEnabled = false;
            }
            else
            {
                brandName.IsEnabled = true;
                foreach (var it in arrlocalBrand)
                {
                    brandList = brandList + "\"" + it + "\"" + ",";
                }
                brandList = brandList.Substring(0, brandList.Length - 1);
            }
            //Load brand name from Brand.txt
            //brandList = brandList + "\"Edit\"";
            //Open Index.html =>edit => save
            string Database = currPath + "\\Temporary\\Index.html";
            string index = File.ReadAllText(Database);
            int startpoint = index.IndexOf("[\"");
            int endpoint = index.IndexOf("\"]");
            string indextemp = index.Remove(startpoint + 1, endpoint - startpoint);
            string parentPath = currPath.Replace("\\", "\\\\");
            indextemp = indextemp.Insert(startpoint + 1, brandList);
            indextemp = indextemp.Remove(indextemp.IndexOf("index1 = ") + 10, indextemp.IndexOf(";//index1") - indextemp.IndexOf("index1 = ") - 11);
            indextemp = indextemp.Insert(indextemp.IndexOf("index1 = ") + 10, parentPath + "\\\\Temporary");
            File.WriteAllText(Database, indextemp);
        }

        private string Sub_String_To_String(string a)//input string output string without special character
        {
            string iModel = a;
            string output = "";
            Regex alphabet = new Regex("[a-zA-Z0-9]");
            string[] iStr = alphabet.Split(iModel);
            string sCh = @"\n|";
            string temp = "";
            try
            {
                foreach (var i in iStr)
                {
                    temp = temp + i.Trim();
                }
                temp = temp.Trim();
                if (temp != "")
                {
                    foreach (var ch in iStr)
                    {
                        if (ch != "")
                        {                           
                            var chh = Regex.Escape(ch);
                            sCh = sCh + chh + @"|";
                        }
                    }
                    sCh = sCh.Remove(sCh.Length - 1);
                    Regex speCh = new Regex(sCh);
                    string[] Str = speCh.Split(iModel);
                    ThongBao = "";
                    foreach (var it in Str)
                    {
                        output = output + it;
                    }
                    output = output.ToLower();
                }
                else
                {
                    Regex speCh2 = new Regex("\n");
                    string[] arrout = speCh2.Split(iModel);
                    foreach (var item in arrout)
                    {
                        output = output + item;
                    }
                }
            }
            catch (Exception ex)
            {

                ThongBao = ex.Message;
            }
            return output;
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
        private void AddDoc_Click(object sender, RoutedEventArgs e)
        {
            string outputBrand = "";
            string inputBrand = "";
            string filename = "";
            string inputModel = "";
            string templateBrand = "<!--Add new " + Model.Text.Trim() + "-->" + "\n" + "<tr>" + "\n" + templateDoc + "<!--Add new " + Model.Text.Trim() + "-->" + "\n";
            string namePdf = pathSource.Substring(pathSource.LastIndexOf("\\") + 1, pathSource.Length - pathSource.LastIndexOf("\\") - 1);
            string drivePdfpath = "https://drive.google.com/file/d/pdfIdspath/view?usp=sharing";
            if (brandName.SelectedItem.ToString() == "" || Title.Text == "" || Model.Text.Trim() == "" || desPdfPath.Text == "")
            {
                if (brandName.SelectedItem.ToString() == "")
                {
                    ThongBao = "Select Brandname of document.";
                }
                else
                {
                    if (Title.Text == "")
                        ThongBao = "Tittle cannot empty.";
                    else
                    {

                        if (Model.Text.Trim() == "")
                            ThongBao = "Model cannot empty";
                        else
                        {
                            if (desPdfPath.Text == "")
                                ThongBao = "Please add document";
                        }
                    }
                }

            }
            else
            {
                string tb = "";
                filename = currPath + "\\Temporary\\" + brandName.SelectedItem.ToString() + ".html";
                inputBrand = File.ReadAllText(filename);
                string[] arrModel = inputBrand.Substring(inputBrand.IndexOf("<!--IndexModel") + 14, inputBrand.LastIndexOf("IndexModel-->") - inputBrand.IndexOf("<!--IndexModel") - 14).Trim().Split(";");
                inputModel = Sub_String_To_String(Model.Text.Trim());
                foreach (var it in arrModel)
                {
                    if (inputModel == Sub_String_To_String(it))
                    {
                        tb = tb + it ;
                    }
                }
                if (tb != "")
                    ThongBao = $"Model existed. Check model: \n {tb}";
                else
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(desPdfPath.Text))
                        {
                            Sub_Upload(pathSource, Sub_Get_Id(brandName.SelectedItem.ToString()), namePdf, "pdf");
                        }
                        drivePdfpath = drivePdfpath.Replace("pdfIdspath", Sub_Get_Id(namePdf, "pdf"));
                        templateBrand = templateBrand.Replace("outputpdfPath", drivePdfpath);//outputpdfPath in template 
                        templateBrand = templateBrand.Replace("outputTitle", Title.Text.Trim());//outputTitle
                        templateBrand = templateBrand.Replace("outputModel", Model.Text.Trim());//outputModel
                        string revision = "";
                        if (string.IsNullOrEmpty(Revision.Text))
                        {
                            revision = "<!--outputRevision-->";
                        }
                        else
                        {
                            revision = Revision.Text;
                        }
                        templateBrand = templateBrand.Replace("outputRevision", revision);//outputRevision
                                                                                          //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                        filename = currPath + "\\Temporary\\" + brandName.SelectedItem.ToString() + ".html";
                        inputBrand = File.ReadAllText(filename);
                        int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                        outputBrand = inputBrand.Insert(insertPos, templateBrand);
                        outputBrand = outputBrand.Insert(outputBrand.LastIndexOf("IndexModel-->"), Model.Text.Trim() + ";" + "\n");
                        File.WriteAllText(filename, outputBrand);
                        var pb = new List<string> { Sub_Get_Id(brandName.SelectedItem.ToString()) };
                        ParentPath = pb;
                        Sub_Delete_Drive(Sub_Get_Id(brandName.SelectedItem.ToString().Trim() + ".html", "html"));

                        Sub_Upload(filename, Sub_Get_Id(brandName.SelectedItem.ToString()), brandName.SelectedItem.ToString() + ".html", "html");
                        //open file Index.html => save Brand index to file 
                        ThongBao = $"Added database of model: {Model.Text}";
                    }
                    catch (Exception ex)
                    {
                        CheckResult.Text = ex.Message;
                    }
                }
            }
            CheckResult.Text = ThongBao;

        }
        private void brandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (brandName.SelectedIndex != -1)
            {
                string brandname = brandName.SelectedItem.ToString();
                if (!File.Exists(currPath + "\\Temporary\\" + brandname + ".html"))
                {
                    //File.Delete(currPath + "\\Temporary\\" + brandname + ".html");
                    Sub_Download(Sub_Get_Id(brandname + ".html", "html"), brandname + ".html");
                    var dir = new DirectoryInfo(currPath + "\\Temporary");
                    dir.Refresh();
                }
            }
            else
            {
                ThongBao = "Choose Brand.";
            }
            CheckResult.Text = ThongBao;
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            Title.Text = "";
            Model.Text = "";
            Revision.Text = "";
            desPdfPath.Text = "";
            newBrand.Text = "";
            CheckResult.Text = "";
            brandName.SelectedIndex = -1;
            Sub_Window_Loaded();
        }
    }
}


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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditDocument : Window
    {
        public string currPath = Directory.GetCurrentDirectory();
        public EditDocument()
        {
            InitializeComponent();
        }
        public string dataId = "";      //id of the folder on the drive, assign when brandName dropdowbox selecedchanged to use for the function delete or change after
        public string brandID = "";     //id of the brandName.html on the drive, assign when brandName dropdowbox selecedchanged to use for the function delete or change after
        public string modelId = "";     // id of the pdf file of the model, assign when model dropdowbox selectchanged to use for delete or change function after
        public string error = "";
        public string pathSource = "";
        public string ThongBao = "";
        public List<string> ParentPath = new List<string>();
        public string ParentId = "";
        public string templateDoc = "";
        public string templateBrandBrand = "";
        public string outputBrand = "";
        //Template string for startTitle, endTitle; startMod, endMod; startRev, endRev; startpdf, endpdf
        public string startTitle = "\"_blank\">"; //8
        public string endTitle = "</a>";
        public string startMod = "\"top\">";//6
        public string endMod = "</td>";
        public string startRev = "\"top\">";//6
        public string endRev = "</td>";
        public string startpdf = "href=\"";//6
        public string endpdf = "target";
        public string ManualDocumentId = "1V1JrUEmwBvrEWNyr8IpAXl09Z8VFZdjO";
        public FileStream stream;

        public UserCredential credential;
        public DriveService service;
        public string ApplicationName = "ManualsLib";
        public string[] Scopes = { DriveService.Scope.Drive };
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SubMainWindow subMainWindow = new SubMainWindow();
            subMainWindow.Show();
            this.Close();
            //SubMainWindow submainWindow = new SubMainWindow();
            //submainWindow.Show();
            //this.Close();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newpdfId = "";
                string pathPdfDrive = desPdfPath.Text;
                string pdfId = pathPdfDrive.Substring(pathPdfDrive.IndexOf("/file/d/") + 8, pathPdfDrive.LastIndexOf("/view") - pathPdfDrive.IndexOf("/file/d/") - 8);
                string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";
                string filename = currPath + "\\Temporary\\" +  brandName.SelectedItem.ToString() + ".html";
                string tempBrand = File.ReadAllText(filename);
                int posModel = tempBrand.IndexOf(templateIndexModel);
                int lastModel = tempBrand.LastIndexOf(templateIndexModel);
                string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
                outputBrand = tempBrand;
                string curTitle = Title.Text;
                string nTitle = newTitle.Text;
                //edit Title
                if (!string.IsNullOrEmpty(newTitle.Text))
                {
                    outputBrand = outputBrand.Replace(curTitle, nTitle);
                }
                //edit Revision
                if (!string.IsNullOrEmpty(newRevision.Text))
                {
                    if (string.IsNullOrEmpty(Revision.Text))
                    {
                        outputBrand = outputBrand.Replace("<!--outputRevision-->", newRevision.Text);
                    }
                    else
                    {
                        outputBrand = outputBrand.Replace(Revision.Text, newRevision.Text);
                    }
                    
                }
                //edit Pdf file
                if (!string.IsNullOrEmpty(newdesPdfPath.Text))
                {
                    string newdesPdfname = newdesPdfPath.Text;
                    newdesPdfname = newdesPdfname.Substring(newdesPdfname.LastIndexOf("\\")+1);
                    Sub_Delete_Drive(pdfId);
                    Sub_Upload(newdesPdfPath.Text, dataId, newdesPdfname, "pdf");
                    newpdfId=Sub_Get_Id(newdesPdfname, "pdf");
                    outputBrand = outputBrand.Replace(pdfId, newpdfId);
                }

                if (!string.IsNullOrEmpty(newModel.Text))
                {
                    string newmodel = newModel.Text;
                    outputBrand = outputBrand.Replace(Model.SelectedItem.ToString(), newmodel);
                }

                File.WriteAllText(filename, outputBrand);
                Title.Text = outputBrand.Substring(outputBrand.IndexOf(startTitle, posModel) + 9, outputBrand.IndexOf(endTitle, posModel) - outputBrand.IndexOf(startTitle, posModel) - 9).Trim();
                Revision.Text = outputBrand.Substring(outputBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) + 6, outputBrand.LastIndexOf(endRev, lastModel, lastModel - posModel - 1) - outputBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) - 6).Trim();
                string newdesPdf = "https://drive.google.com/file/d/pdfIdspath/view?usp=sharing";
                desPdfPath.Text = newdesPdf.Replace("pdfIdspath", newpdfId);
                Sub_Delete_Drive(brandID);
                Sub_Upload(filename, dataId, brandName.SelectedItem.ToString() + ".html", "html");
            }
            catch (Exception ex)
            {
                CheckResult.Text = ex.Message;
            }
            Sub_Window_Loaded();
           
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //search table data of Model selected in brandName.html and delete all code, search pdf name and delete in folder as well, delete index of Model in <!--IndexModel in brandName.html
            string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";
            string filename = currPath + "\\Temporary\\" +  brandName.SelectedItem.ToString() + ".html";
            string tempBrand = File.ReadAllText(filename);
            int posModel = tempBrand.IndexOf(templateIndexModel);
            string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
            string pdfId= pdfname.Substring(pdfname.IndexOf("/file/d/") + 8, pdfname.LastIndexOf("/view") - pdfname.IndexOf("/file/d/") - 8);
            outputBrand = tempBrand.Remove(posModel, tempBrand.LastIndexOf(templateIndexModel) - posModel + templateIndexModel.Length);
            outputBrand = outputBrand.Replace(Model.SelectedItem.ToString() + ";", "");
            File.WriteAllText(filename, outputBrand);
            if (CheckResult.Text != $"Document don't exist in path {pdfname}")
            {
                Sub_Delete_Drive(modelId);
            }
            Sub_Delete_Drive(brandID);
            Sub_Upload(filename, dataId, brandName.SelectedItem.ToString()+".html","html");            
            CheckResult.Text = $"Deleted database of {Model.SelectedItem.ToString()}";
            brandID = Sub_Get_Id(Model.SelectedItem.ToString() + ".html", "html");

            Sub_Window_Loaded();
            //}
            //catch(Exception ex)
            //{
            //    CheckResult.Text = ex.Message;
            //}

        }

        private void desPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDesPdf = new OpenFileDialog();
            openDesPdf.DefaultExt = ".pdf";
            openDesPdf.Filter = "Text Document (.pdf)|*.pdf";
            if (openDesPdf.ShowDialog() == true)
            {
                string filePath = openDesPdf.FileName;
                newdesPdfPath.Text = filePath;
                pathSource = filePath;
            }
        }


        private void DeleteBrand_Click(object sender, RoutedEventArgs e)
        {
            if (brandName.SelectedIndex != -1)
            {
                Sub_Delete_Drive(Sub_Get_Id(brandName.SelectedItem.ToString()));
                string inputBrand = File.ReadAllText(currPath + "\\Temporary\\Brand.txt");
                outputBrand = inputBrand.Replace(";" + brandName.SelectedItem.ToString(), "");
                File.WriteAllText(currPath + "\\Temporary\\Brand.txt", outputBrand);
                Sub_Delete_Drive(Sub_Get_Id("Brand.txt", "txt"));
                File.Delete(currPath + "\\Temporary\\" + brandName.SelectedItem.ToString() + ".html");
                Sub_Upload(currPath + "\\Temporary\\Brand.txt", ManualDocumentId, "Brand.txt", "txt");
                ThongBao = "Deleted " + brandName.SelectedItem.ToString();
                Sub_Window_Loaded();
            }
            else
            {
                ThongBao = "Please select Brand.";
            }
            CheckResult.Text = ThongBao;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Title.Text = "";
            newTitle.Text = "";
            Model.SelectedIndex = -1;
            Revision.Text = "";
            newRevision.Text = "";
            desPdfPath.Text = "";
            newdesPdfPath.Text = "";
            CheckResult.Text = "";
            brandName.SelectedIndex = -1;
            Sub_Window_Loaded();
        }
        private void Sub_Cloud_Create(string name)
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

        private string Sub_Get_Id(string foldername)
        {

            string folderId = "";

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
        private string Sub_Read(string fileId)
        {
            string folderId = "";

            try
            {
                var request = service.Files.Get(fileId).Execute();
                var name = request.MimeType;
                folderId = name;
            }
            catch (Exception)
            {
                folderId = "File not found.";
            }


            return folderId;

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
            if (brandName.SelectedIndex == -1)
            {
                Model.IsEnabled = false;
            }
            else
            {
                Model.IsEnabled = true;
            }
        }

        private string Sub_String_To_String(string a)//input string output string without special character
        {
            string iModel = a;
            string output = "";
            Regex alphabet = new Regex("[a-zA-Z0-9]");
            string[] iStr = alphabet.Split(iModel);
            string sCh = "\\n|";
            string temp = "";
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
                        sCh = sCh + "\\" + ch + "|";
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

            return output;
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
            try
            {
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
            catch (Exception)
            {
                error = "File not found.";
            }

        }

        private void brandName_DropDownClosed(object sender, EventArgs e)
        {
            string brandname = "";
            if (brandName.SelectedIndex != -1)
            {
                Model.IsEnabled = true;
                brandname = brandName.SelectedItem.ToString();
                if (!File.Exists(currPath + "\\Temporary\\" + brandname + ".html"))
                {
                    
                    //File.Delete(currPath + "\\Temporary\\" + brandname + ".html");
                    Sub_Download(brandID, brandname + ".html");
                }
                dataId = Sub_Get_Id(brandname);
                brandID = Sub_Get_Id(brandname + ".html", "html");
            }
            else
            {
                Model.IsEnabled = false;
            }
            var dir = new DirectoryInfo(currPath + "\\Temporary");
            dir.Refresh();
            if(brandName.SelectedIndex !=-1)
            {
                if (File.Exists(currPath + "\\Temporary\\" + brandName.SelectedItem.ToString() + ".html"))
                {
                    string filename = currPath + "\\Temporary\\" + brandName.SelectedItem + ".html";
                    string tempBrand = File.ReadAllText(filename);
                    string IndexModel = tempBrand.Substring(tempBrand.IndexOf("<!--IndexModel") + 14, tempBrand.LastIndexOf("IndexModel-->") - tempBrand.IndexOf("<!--IndexModel") - 14);
                    var arrIndexModel = IndexModel.Trim().Split(";");
                    arrIndexModel = arrIndexModel.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    for (int i = 0; i < arrIndexModel.Length; i++)
                    {
                        arrIndexModel[i] = arrIndexModel[i].Trim();
                    }
                    Model.ItemsSource = arrIndexModel.ToList();
                    
                }
            }
            
            CheckResult.Text = ThongBao;
        }

        private void Model_DropDownClosed(object sender, EventArgs e)
        {
            string filename = currPath + "\\Temporary\\" + brandName.SelectedItem + ".html";
            string tempBrand = File.ReadAllText(filename);
            string IndexModel = tempBrand.Substring(tempBrand.IndexOf("<!--IndexModel") + 14, tempBrand.LastIndexOf("IndexModel-->") - tempBrand.IndexOf("<!--IndexModel") - 14);
            var arrIndexModel = IndexModel.Trim().Split(";");
            arrIndexModel = arrIndexModel.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < arrIndexModel.Length; i++)
            {
                arrIndexModel[i] = arrIndexModel[i].Trim();
            }
            Model.ItemsSource = arrIndexModel.ToList();
            if (Model.SelectedIndex != -1)
            {   
                string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";
                int posModel = tempBrand.IndexOf(templateIndexModel);
                int lastModel = tempBrand.LastIndexOf(templateIndexModel);
                string oldTitle = tempBrand.Substring(tempBrand.IndexOf(startTitle, posModel) + 9, tempBrand.IndexOf(endTitle, posModel) - tempBrand.IndexOf(startTitle, posModel) - 9).Trim();
                Title.Text = oldTitle;
                string oldModel = tempBrand.Substring(tempBrand.IndexOf(startMod, posModel) + 6, tempBrand.IndexOf(endMod, posModel) - tempBrand.IndexOf(startMod, posModel) - 6);
                string oldRev = tempBrand.Substring(tempBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) + 6, tempBrand.LastIndexOf(endRev, lastModel, lastModel - posModel - 1) - tempBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) - 6).Trim();
                if(oldRev== "<!--outputRevision-->")
                {
                    oldRev = "";
                }
                Revision.Text = oldRev;
                string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
                desPdfPath.Text = pdfname;
                string pathPdfDrive = pdfname.Substring(pdfname.IndexOf("/file/d/") + 8, pdfname.LastIndexOf("/view") - pdfname.IndexOf("/file/d/") - 8);
                ThongBao = pathPdfDrive;

                if (Sub_Read(pathPdfDrive) != "application/pdf")
                {
                    ThongBao = $"Document don't exist in path {pdfname}";
                }
                CheckResult.Text = ThongBao;
                modelId = pathPdfDrive;
            }
        }
    }
}
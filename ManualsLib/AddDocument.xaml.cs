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

namespace ManualsLib
{
    /// <summary>
    /// Interaction logic for AddDocument.xaml
    /// </summary>
    public partial class AddDocument : Window
    {
        public AddDocument()
        {
            InitializeComponent();
        }
        public string pathSource = ""; //Parent folder of project
        public string currPath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("Database"));//get the current path of Database folder
        public string ThongBao = "Message show here";
        //public string templateBrand = "";
        public string templateDoc = "";
        public string oCurrBrand = "";
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SubloadTemplate();            
            Sub_Window_Loaded();
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

        private void AddDoc_Click(object sender, RoutedEventArgs e)
        {
            //load file brand based on selection and open, insert templateBrand to file
            string outputBrand = "";
            string inputBrand = "";
            string filename = "";
            //file name.pdf
            string namePdf = pathSource.Substring(pathSource.LastIndexOf("\\") + 1, pathSource.Length - pathSource.LastIndexOf("\\") - 1);
            string templateBrand = "<!--Add new " + Model.Text + "-->" + "\n" + "<tr>" + "\n" + templateDoc + "<!--Add new " + Model.Text + "-->" + "\n";
            if (Title.Text=="" || Model.Text=="" || desPdfPath.Text=="")
            {
                if (Title.Text == "")
                    ThongBao = "Tittle cannot empty.";
                else
                {
                    if (Model.Text == "")
                        ThongBao = "Model cannot empty";
                    else
                    {
                        filename = currPath + "\\Database\\" + brandName.SelectedItem.ToString() + "\\" + brandName.SelectedItem.ToString() + ".html";
                        inputBrand = File.ReadAllText(filename);
                        if (inputBrand.Contains(Model.Text))
                            ThongBao = "Model is already existed.";
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
                try
                {   
                    templateBrand = templateBrand.Replace("outputpdfPath", namePdf);//outputpdfPath in template 
                    templateBrand = templateBrand.Replace("outputTitle", Title.Text);//outputTitle
                    templateBrand = templateBrand.Replace("outputModel", Model.Text);//outputModel
                    templateBrand = templateBrand.Replace("outputRevision", Revision.Text);//outputRevision
                                                                                           //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                    File.Copy(pathSource, currPath + "Database\\" + brandName.SelectedItem.ToString() + "\\" + namePdf);
                    filename = currPath + "\\Database\\" + brandName.SelectedItem.ToString() + "\\" + brandName.SelectedItem.ToString() + ".html";
                    inputBrand = File.ReadAllText(filename);
                    int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                    outputBrand = inputBrand.Insert(insertPos, templateBrand);
                    outputBrand = outputBrand.Insert(outputBrand.LastIndexOf("IndexModel-->"), Model.Text + ";" + "\n");
                    File.WriteAllText(filename, outputBrand);
                    //open file Index.html => save Brand index to file 
                    ThongBao = $"Added database of model: {Model.Text}";                    
                }
                catch (Exception ex)
                {
                    CheckResult.Text = ex.Message;
                }
            }
            CheckResult.Text = ThongBao;
        }
        s
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {
            string outputBrand = "";
            DirectoryInfo di = new DirectoryInfo(currPath + "\\Database\\" + newBrand.Text);
            if (di.Exists)
            {
                ThongBao = "This brand name already exsist.";
            }
            else
            {
                string folderName = newBrand.Text;
                Directory.CreateDirectory(currPath + "\\Database\\" + folderName);
                string inputBrand = File.ReadAllText(currPath + "\\Database\\" + "Template.html");
                outputBrand = inputBrand.Replace("brandnameheader", folderName);
                File.WriteAllText(currPath + "\\Database\\" + folderName + "\\" + folderName +".html",outputBrand);
                string inputIndex = File.ReadAllText(currPath + "\\Database\\" + "Brand.txt");
                string outputIndex = inputIndex + ";" + folderName;
                File.WriteAllText(currPath + "\\Database\\Brand.txt", outputIndex);
                ThongBao = "Added folder database for " + newBrand.Text;
                Sub_Window_Loaded();
            }
            CheckResult.Text = ThongBao;
        }
        private void SubloadTemplate()
        {
            string doc = "<!--template add new document-->";
            string tempAddBrand = File.ReadAllText(currPath + "\\Database\\Template.html");
            string tempAddDoc = tempAddBrand.Substring(tempAddBrand.IndexOf(doc) + doc.Length + 6, tempAddBrand.LastIndexOf(doc) - tempAddBrand.IndexOf(doc) - doc.Length - 11);
            templateDoc = tempAddDoc;
        }
        private void Sub_Window_Loaded()
        {
            string filename = currPath + "Database\\Brand.txt";
            string localBrand = File.ReadAllText(filename);
            var arrlocalBrand = localBrand.Split(';');
            brandName.ItemsSource = arrlocalBrand.ToList();
            
            //Load brand name from Brand.txt
            string brandList = "";
            foreach (var it in arrlocalBrand)
            {
                brandList = brandList + "\"" + it + "\"" + ",";
            }
            brandList = brandList + "\"Edit\"";
            //Open Index.html =>edit => save
            string Database = currPath + "\\Index.html";
            string index = File.ReadAllText(Database);
            int startpoint = index.IndexOf("\"About\"");
            int endpoint = index.IndexOf("\"Edit\"");
            int ttlength = index.Length;
            string indextemp = index.Remove(startpoint, endpoint - startpoint + 6);
            string parentPath = currPath.Replace("\\", "\\\\");
            indextemp = indextemp.Insert(startpoint, brandList);
            indextemp = indextemp.Remove(indextemp.IndexOf("index1 = ")+10, indextemp.IndexOf(";//index1")- indextemp.IndexOf("index1 = ")-11);
            indextemp = indextemp.Insert(indextemp.IndexOf("index1 = ")+10, parentPath+"Database");
            File.WriteAllText(Database, indextemp);
            ThongBao = "Template add document: " + "\n" + templateDoc;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Title.Text = "";
            Model.Text = "";
            Revision.Text = "";
            desPdfPath.Text = "";
            newBrand.Text = "";
            CheckResult.Text = "";
            Sub_Window_Loaded();
        }
    }
}

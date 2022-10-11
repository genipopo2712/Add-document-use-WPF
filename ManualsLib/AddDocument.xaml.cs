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
            //templateBrand string add to brand name
            //string templateBrand = "<!--Add new " + Model.Text + "-->" + "\n" + "<tr>" + "\n";
            //templateBrand = templateBrand + "<td class=\"TableBody\" valigned=\"top\">" + "\n";
            //templateBrand = templateBrand + "<a href=\"" + namePdf + "\" target=\"_blank\">" + Title.Text + "</a>" + "\n";
            //templateBrand = templateBrand + " </td>" + "\n";
            //templateBrand = templateBrand + " <td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            //templateBrand = templateBrand + Model.Text + "\n";
            //templateBrand = templateBrand + "</td>" + "\n";
            //templateBrand = templateBrand + "<td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            //templateBrand = templateBrand + Revision.Text + "\n";
            //templateBrand = templateBrand + "</td>" + "\n";
            //templateBrand = templateBrand + "</tr>" + "\n";
            //templateBrand = templateBrand + "<!--Add new " + Model.Text + "-->" + "\n";
            //open file Brand.html => add => save
            string templateBrand = "<!--Add new " + Model.Text + "-->" + "\n" + "<tr>" + "\n" + templateDoc + "<!--Add new " + Model.Text + "-->" + "\n";
            try
            {
                templateBrand = templateBrand.Replace("outputpdfPath", namePdf);//outputpdfPath in template 
                templateBrand = templateBrand.Replace("outputTitle", Title.Text);//outputTitle
                templateBrand = templateBrand.Replace("outputModel", Model.Text);//outputModel
                templateBrand = templateBrand.Replace("outputRevision", Revision.Text);//outputRevision
                //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                File.Copy(pathSource,currPath+ "Database\\" + Model.Text + "\\" +namePdf);
                filename = currPath + "\\Database\\" + Model.Text + "\\" + brandName.SelectedItem.ToString() + ".html";
                inputBrand = File.ReadAllText(filename);
                int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                outputBrand = inputBrand.Insert(insertPos, templateBrand);
                outputBrand = outputBrand.Insert(outputBrand.LastIndexOf("IndexModel-->"), Model.Text + ";"+ "\n");
                File.WriteAllText(filename, outputBrand);
                //open file Index.html => save Brand index to file 
                ThongBao= $"Added database of model: {Model.Text}";
                CheckResult.Text = ThongBao;
            }
            catch (Exception ex)
            {
                CheckResult.Text = ex.Message;
            }
        }

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
            }
            CheckResult.Text = ThongBao;
            Sub_Window_Loaded();
            this.refresh
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
            CheckResult.Text = CheckResult.Text + startpoint + ";" + endpoint + "\n";
            string indextemp = index.Remove(startpoint, endpoint - startpoint + 6);
            indextemp = indextemp.Insert(startpoint, brandList);
            File.WriteAllText(Database, indextemp);
            ThongBao = "Template add document: " + "\n" + templateDoc;
        }
    }
}

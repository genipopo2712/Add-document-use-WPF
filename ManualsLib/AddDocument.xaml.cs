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
        public string pathSource = "";
        public string templateBrandBrand = "";
        public string currPath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("Database"));//get the current path of Database folder
        




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Read file Brand to creat drop-dow selection box for Category assign to array "arrlocalBrand"
            string filename =currPath + "Database\\Brand.txt";
            string localBrand = File.ReadAllText(filename);
            var arrlocalBrand = localBrand.Split(';');
            brandName.ItemsSource = arrlocalBrand.ToList();
            CheckResult.Text = "";
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
            CheckResult.Text = CheckResult.Text + currPath;
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //load file brand based on selection and open, insert templateBrand to file
            
            string Database = "C:\\Users\\dtran\\Documents\\02.Equipment Manual\\99.ManualsLibrary\\Database\\";
            string outputBrand = "";
            string inputBrand = "";
            string filename = "";
            //file name.pdf
            string namePdf = pathSource.Substring(pathSource.LastIndexOf("\\") + 1, pathSource.Length - pathSource.LastIndexOf("\\") - 1);
            //templateBrand string add to brand name
            string templateBrand = "<!--Add new " + Model.Text + "-->" + "\n" + "<tr>" + "\n";
            templateBrand = templateBrand + "<td class=\"TableBody\" valigned=\"top\">" + "\n";
            templateBrand = templateBrand + "<a href=\"" + namePdf + "\" target=\"_blank\">" + Title.Text + "</a>" + "\n";
            templateBrand = templateBrand + " </td>" + "\n";
            templateBrand = templateBrand + " <td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            templateBrand = templateBrand + Model.Text + "\n";
            templateBrand = templateBrand + "</td>" + "\n";
            templateBrand = templateBrand + "<td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            templateBrand = templateBrand + Revision.Text + "\n";
            templateBrand = templateBrand + "</td>" + "\n";
            templateBrand = templateBrand + "</tr>" + "\n";
            templateBrand = templateBrand + "<!--Add new " + Model.Text + "-->" + "\n";
            //open file Brand.html => add => save
            try
            {
                //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                File.Copy(pathSource,currPath+ "Database\\"+namePdf);
                filename = Database + brandName.SelectedItem.ToString() + ".html";
                inputBrand = File.ReadAllText(filename);
                int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                outputBrand = inputBrand.Insert(insertPos, templateBrand);
                outputBrand = outputBrand.Insert(outputBrand.LastIndexOf("IndexModel-->"), Model.Text + ";"+ "\n");
                File.WriteAllText(filename, outputBrand);
                //open file Index.html => save Brand index to file 
                CheckResult.Text = $"Added database of model: {Model.Text}";
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
    }
}

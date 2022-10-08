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
        public string pathDes = "";
        public string templateBrandBrand = "";





        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Read file Brand to creat drop-dow selection box for Category assign to array "arrlocalBrand"
            string filename = "C:\\Users\\dtran\\Documents\\02.Equipment Manual\\99.ManualsLibrary\\Database\\Brand.txt";
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
            string Database = "C:\\Users\\dtran\\Documents\\02.Equipment Manual\\99.ManualsLibrary\\Index.html";
            string index = File.ReadAllText(Database);
            int startpoint = index.IndexOf("\"About\"");
            int endpoint = index.IndexOf("\"Edit\"");
            int ttlength = index.Length;
            CheckResult.Text = CheckResult.Text + startpoint + ";" + endpoint + "\n";
            string indextemp = index.Remove(startpoint, endpoint - startpoint + 6);
            indextemp = indextemp.Insert(startpoint, brandList);
            File.WriteAllText(Database, indextemp);
            CheckResult.Text = CheckResult.Text + indextemp;
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
                pathDes = filePath;
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
            string namePdf = pathDes.Substring(pathDes.LastIndexOf("\\") + 1, pathDes.Length - pathDes.LastIndexOf("\\") - 1);
            //templateBrand string add to brand name
            string templateBrand = "\\\\Add new" + namePdf + "\n" + "<tr>" + "\n";
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
            //open file Brand.html => add => save
            try
            {
                //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                File.Copy(pathDes, namePdf);
                filename = Database + brandName.SelectedItem.ToString() + ".html";
                inputBrand = File.ReadAllText(filename);
                int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                outputBrand = inputBrand.Insert(insertPos, templateBrand);
                File.WriteAllText(filename, outputBrand);
                //open file Index.html => save Brand index to file 

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

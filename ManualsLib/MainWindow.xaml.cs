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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public string pathDes = "";
        public string templateBrand = "";





        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Read file Brand to creat drop-dow selection box for Category assign to array "arrlocalBrand"
            string filename = "C:\\Users\\dtran\\Documents\\02.Equipment Manual\\99.ManualsLibrary\\Database\\Brand.txt";
            string localBrand = File.ReadAllText(filename);
            var arrlocalBrand = localBrand.Split(';');
            brandName.ItemsSource = arrlocalBrand.ToList();
        }

        private void desPdf_Click(object sender, RoutedEventArgs e)
        {
            //Choose file pdf from
            OpenFileDialog openDesPdf = new OpenFileDialog();
            openDesPdf.DefaultExt = ".pdf";
            openDesPdf.Filter = "Text Document (.pdf)|*.pdf";
            if(openDesPdf.ShowDialog()==true)
            {
                string filePath = openDesPdf.FileName;
                desPdfPath.Text = filePath;
                pathDes = filePath;
            }

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //load file brand based on selection and open, insert template to file
            string Database = "C:\\Users\\dtran\\Documents\\02.Equipment Manual\\99.ManualsLibrary\\Database\\";
            string outputBrand = "";
            string inputBrand = "";
            string filename = "";
            //file name.pdf
            string namePdf = pathDes.Substring(pathDes.LastIndexOf("\\")+1,pathDes.Length- pathDes.LastIndexOf("\\")-1);
            //template string add to brand name
            string template = "\\\\Add new" + namePdf + "\n" + "<tr>" + "\n";
            template = template + "<td class=\"TableBody\" valigned=\"top\">" + "\n";
            template = template + "<a href=\"" + namePdf + "\" target=\"_blank\">" + Title.Text + "</a>" + "\n";
            template = template + " </td>" +"\n";
            template = template + " <td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            template = template + Model.Text + "\n";
            template = template + "</td>" + "\n";
            template = template + "<td class=\"TableBody\" align=\"center\" valigned=\"top\">" + "\n";
            template = template + Revision.Text + "\n";
            template = template + "</td>" + "\n";
            template = template + "</tr>" + "\n";
            //open file Brand.html => add => save
            try
            {
                //Copy file pdf from Source to Database if error dont act  CANNOT OVERWRITE IF FILE PDF ALREADY EXIST 
                File.Copy(pathDes, namePdf);
                filename = Database + brandName.SelectedItem.ToString() + ".html";
                inputBrand = File.ReadAllText(filename);
                int insertPos = (int)inputBrand.LastIndexOf(" </table>");
                outputBrand = inputBrand.Insert(insertPos, template);
                File.WriteAllText(filename, outputBrand);                              
            }
            catch (Exception ex)
            {
                CheckResult.Text = ex.Message;
            }
        }
    }
}

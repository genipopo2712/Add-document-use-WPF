using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ManualsLib
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class EditDocument : Window
    {
        public EditDocument()
        {
            InitializeComponent();
        }
        public string pathSource = "";
        public string templateBrandBrand = "";
        public string outputBrand = "";
        public string currPath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("Database"));//get the current path of Database folder
        //Template string for startTitle, endTitle; startMod, endMod; startRev, endRev; startpdf, endpdf
        public string startTitle = "\"_blank\">"; //8
        public string endTitle = "</a>";
        public string startMod = "\"top\">";//6
        public string endMod = "</td>";
        public string startRev = "\"top\">";//6
        public string endRev = "</td>";
        public string startpdf = "href=\"";//6
        public string endpdf = "target";
        public string ThongBao = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Read file Brand to creat drop-dow selection box for Category assign to array "arrlocalBrand"
                string filename = currPath + "Database\\Brand.txt";
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
                string indextemp = index.Remove(startpoint, endpoint - startpoint + 6);
                indextemp = indextemp.Insert(startpoint, brandList);
                File.WriteAllText(Database, indextemp);
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
        private void brandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filename = currPath + "Database\\" +brandName.SelectedItem.ToString()+ "\\"+ brandName.SelectedItem + ".html";
            string tempBrand = File.ReadAllText(filename);
            string IndexModel = tempBrand.Substring(tempBrand.IndexOf("<!--IndexModel")+14, tempBrand.LastIndexOf("IndexModel-->") - tempBrand.IndexOf("<!--IndexModel")-14);
            var arrIndexModel = IndexModel.Trim().Split(";");
            arrIndexModel = arrIndexModel.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            for (int i = 0; i < arrIndexModel.Length; i++)
            {
                arrIndexModel[i] = arrIndexModel[i].Trim();
            }            
            Model.ItemsSource = arrIndexModel.ToList();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string namePdf = pathSource.Substring(pathSource.LastIndexOf("\\") + 1, pathSource.Length - pathSource.LastIndexOf("\\") - 1);
                string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";
                string filename = currPath + "Database\\" + brandName.SelectedItem.ToString() + "\\" + brandName.SelectedItem.ToString() + ".html";
                string tempBrand = File.ReadAllText(filename);
                int posModel = tempBrand.IndexOf(templateIndexModel);
                int lastModel = tempBrand.LastIndexOf(templateIndexModel);
                string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
                outputBrand = tempBrand;
                string curTitle = Title.Text;
                string nTitle = newTitle.Text;
                //edit Title
                if (newTitle.Text != null)
                {
                    outputBrand = outputBrand.Replace(curTitle, nTitle);
                }
                //edit Revision
                if (newRevision.Text != null)
                {
                    outputBrand = outputBrand.Replace(Revision.Text, newRevision.Text);
                }
                //edit Pdf file
                if (newdesPdfPath.Text != "")
                {
                    File.Delete(currPath + "Database\\" + pdfname);
                    File.Copy(pathSource, currPath + "Database\\" + namePdf);
                    outputBrand = outputBrand.Replace(desPdfPath.Text.Substring(desPdfPath.Text.LastIndexOf("\\") + 1, desPdfPath.Text.Length - desPdfPath.Text.LastIndexOf("\\") - 1), newdesPdfPath.Text.Substring(newdesPdfPath.Text.LastIndexOf("\\") + 1, newdesPdfPath.Text.Length - newdesPdfPath.Text.LastIndexOf("\\") - 1));
                }

                if (newModel.Text != null)
                {

                }

                File.WriteAllText(filename, outputBrand);
                Title.Text = outputBrand.Substring(outputBrand.IndexOf(startTitle, posModel) + 9, outputBrand.IndexOf(endTitle, posModel) - outputBrand.IndexOf(startTitle, posModel) - 9).Trim();
                Revision.Text = outputBrand.Substring(outputBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) + 6, outputBrand.LastIndexOf(endRev, lastModel, lastModel - posModel - 1) - outputBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1) - 6).Trim();
                desPdfPath.Text = currPath + "Database\\" + outputBrand.Substring(outputBrand.IndexOf(startpdf, posModel) + 6, outputBrand.IndexOf(endpdf, posModel) - outputBrand.IndexOf("href=\"", posModel) - 8);
        }
            catch (Exception ex)
            {
                CheckResult.Text = ex.Message;
            }

}

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                //search table data of Model selected in brandName.html and delete all code, search pdf name and delete in folder as well, delete index of Model in <!--IndexModel in brandName.html
                string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";                
                string filename = currPath + "Database\\" + brandName.SelectedItem.ToString() + "\\" + brandName.SelectedItem.ToString() + ".html";
                string tempBrand = File.ReadAllText(filename);
                int posModel = tempBrand.IndexOf(templateIndexModel);
                string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
                File.Delete(currPath + "Database\\" + pdfname);
                outputBrand = tempBrand.Remove(posModel, tempBrand.LastIndexOf(templateIndexModel)-posModel+templateIndexModel.Length);
                outputBrand = outputBrand.Replace(Model.SelectedItem.ToString() + ";", "");
                File.WriteAllText(filename, outputBrand);
                CheckResult.Text = $"Deleted database of {Model.SelectedItem.ToString()}"; 
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

        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Model.SelectedItem!=null)
            {
                string filename = currPath + "Database\\" + brandName.SelectedItem.ToString()+"\\"+ brandName.SelectedItem.ToString() + ".html";
                string tempBrand = File.ReadAllText(filename);
                string templateIndexModel = "<!--Add new " + Model.SelectedItem.ToString() + "-->";
                int posModel = tempBrand.IndexOf(templateIndexModel);
                int lastModel = tempBrand.LastIndexOf(templateIndexModel);
                string oldTitle = tempBrand.Substring(tempBrand.IndexOf(startTitle, posModel) + 9, tempBrand.IndexOf(endTitle, posModel) - tempBrand.IndexOf(startTitle, posModel) - 9).Trim();
                Title.Text = oldTitle;
                string oldModel = tempBrand.Substring(tempBrand.IndexOf(startMod, posModel) + 6, tempBrand.IndexOf(endMod, posModel) - tempBrand.IndexOf(startMod, posModel) - 6);                
                string oldRev = tempBrand.Substring(tempBrand.LastIndexOf(startRev,lastModel,lastModel-posModel-1)+6,tempBrand.LastIndexOf(endRev,lastModel,lastModel-posModel-1)- tempBrand.LastIndexOf(startRev, lastModel, lastModel - posModel - 1)-6).Trim();
                Revision.Text = oldRev;
                string pdfname = tempBrand.Substring(tempBrand.IndexOf(startpdf, posModel) + 6, tempBrand.IndexOf(endpdf, posModel) - tempBrand.IndexOf("href=\"", posModel) - 8);
                desPdfPath.Text = currPath + "Database\\" + brandName.SelectedItem.ToString() + "\\" + pdfname;
                string patha = currPath + "Database\\" + brandName.SelectedItem.ToString() + "\\" + pdfname;
                if (!File.Exists(patha))
                {
                    ThongBao = $"Document don't exist in path {currPath}Database\\{brandName.SelectedItem.ToString()}";
                }
                CheckResult.Text = ThongBao;
            }    
            
        }

        private void DeleteBrand_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(currPath + "Database\\" + brandName.SelectedItem.ToString(),true);
            string inputBrand = File.ReadAllText(currPath + "\\Database\\Brand.txt");
            outputBrand = inputBrand.Replace(brandName.SelectedItem.ToString()+";", "");
            File.WriteAllText(currPath + "\\Database\\Brand.txt", outputBrand);
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
            indextemp = indextemp.Insert(startpoint, brandList);
            File.WriteAllText(Database, indextemp);
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
            Sub_Window_Loaded();
        }
    }
}

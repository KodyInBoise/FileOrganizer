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
using System.IO;
using System.Windows.Forms;
using FileOrganizer.Utilities;

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for EditRule.xaml
    /// </summary>
    public partial class EditRule : Window
    {
        MainWindow MainWin;
        string ActiveDir;
        string Keyword;
        List<FileInfo> FileList;
        DataHelper AppData;

        private Rule ActiveRule;

        public EditRule(MainWindow mainWin)
        {
            InitializeComponent();
            MainWin = mainWin;
            AppData = new DataHelper();
            titleLBL.Content = "New Rule";
            actionCB.SelectedIndex = 0;
            deleteIMG.Visibility = Visibility.Collapsed;
            destDirLBL.Visibility = Visibility.Collapsed;
            destTB.Visibility = Visibility.Collapsed;
            destBrowseBTN.Visibility = Visibility.Collapsed;
        }

        public EditRule(MainWindow mainWin, Rule r)
        {
            InitializeComponent();
            AppData = new DataHelper();
            MainWin = mainWin;
            titleLBL.Content = "Edit Rule";
            ActiveRule = r;
            nameTB.Text = r.Name;
            sourceTB.Text = r.SourceDir;
            destTB.Text = r.DestDir;
            keywordTB.Text = r.Keyword;
            actionCB.Text = r.Action;
        }

        public string GetDownloadsDir()
        {
            try
            {
                string downloadsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                ActiveDir = downloadsPath;

                return downloadsPath;
            }
            catch
            {
                return "Error";
            }
        }

        private void browseBTN_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();

            ActiveDir = folderBrowser.SelectedPath;
            sourceTB.Text = ActiveDir;
        }

        private async Task ScanDirFiles()
        {
            FileList = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(ActiveDir);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo f in files)
            {
                var name = f.ToString().ToLower();
                Keyword = Keyword.ToLower();
                if (name.Contains(Keyword))
                {
                    FileList.Add(f);
                }
            }
        }

        private void CreateNewRule()
        {
            Rule newRule = new Rule
            {
                Name = nameTB.Text,
                ModifiedTimestamp = DateTime.Now.ToString(), 
                SourceDir = sourceTB.Text, 
                DestDir = destTB.Text, 
                Action = actionCB.Text,
                Keyword = keywordTB.Text, 
                Frequency = frequencyCB.Text
            };

            if (newRule.Action == "Delete")
            {
                newRule.DestDir = "Trash";
            }

            AppData.CreateRule(newRule);
            MainWin.ExistingRules.Add(newRule);
            MainWin.rulesDG.Items.Refresh();
            this.Close();
        }

        private void UpdateRule()
        {
            AppData.UpdateRule(ActiveRule);
            MainWin.rulesDG.Items.Refresh();
            this.Close();
        }

        private void DeleteRule()
        {
            AppData.DeleteRule(ActiveRule);
            MainWin.ExistingRules.Remove(ActiveRule);
            MainWin.rulesDG.Items.Refresh();
            this.Close();
        }

        private void FindSourceDir()
        {
            var folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            sourceTB.Text = folderBrowser.SelectedPath;
        }

        private void FindDestDir()
        {
            var folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            destTB.Text = folderBrowser.SelectedPath;
        }

        private void browseBTN_Click_1(object sender, RoutedEventArgs e)
        {
            FindSourceDir();
        }

        private void browseBTN_Copy_Click(object sender, RoutedEventArgs e)
        {
            FindDestDir();
        }

        private async void Finish(object sender, EventArgs e)
        {
            if (ActiveRule == null)
            {              
                CreateNewRule();
            }
            else
            {
                ActiveRule.Name = nameTB.Text; 
                ActiveRule.SourceDir = sourceTB.Text;
                ActiveRule.Action = actionCB.Text;
                ActiveRule.DestDir = destTB.Text;
                ActiveRule.Keyword = keywordTB.Text;
                ActiveRule.Frequency = frequencyCB.Text;

                UpdateRule();
            }
        }

        private void deleteIMG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeleteRule();
        }

        private void actionCB_DropDownClosed(object sender, EventArgs e)
        {
            switch(actionCB.Text)
            {
                case "Delete":
                    destDirLBL.Visibility = Visibility.Collapsed;
                    destTB.Visibility = Visibility.Collapsed;
                    destBrowseBTN.Visibility = Visibility.Collapsed;
                    break;
                case "Move":
                    destDirLBL.Visibility = Visibility.Visible;
                    destTB.Visibility = Visibility.Visible;
                    destBrowseBTN.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}

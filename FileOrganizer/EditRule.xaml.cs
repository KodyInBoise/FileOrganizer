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

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for EditRule.xaml
    /// </summary>
    public partial class EditRule : Window
    {
        string ActiveDir;
        string Keyword;
        List<FileInfo> FileList;

        private Rule ActiveRule;

        public EditRule()
        {
            InitializeComponent();
            titleLBL.Content = "New Rule";
            actionCB.SelectedIndex = 0;
            deleteIMG.Visibility = Visibility.Collapsed;
            destDirLBL.Visibility = Visibility.Collapsed;
            destTB.Visibility = Visibility.Collapsed;
            destBrowseBTN.Visibility = Visibility.Collapsed;
        }

        public EditRule(Rule r)
        {
            InitializeComponent();
            titleLBL.Content = "Edit Rule";
            ActiveRule = r;
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

        private async Task CreateNewRule()
        {
            Rule tmpRule = new Rule
            {
                ModifiedTimestamp = DateTime.Now.ToString(), 
                SourceDir = sourceTB.Text, 
                DestDir = destTB.Text, 
                Action = actionCB.Text,
                Keyword = keywordTB.Text, 
                Frequency = frequencyCB.Text
            };

            if (tmpRule.Action == "Delete")
            {
                tmpRule.DestDir = "Trash";
            }

            Storage.SaveRule(tmpRule);
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

        private async void testSearchBTN_Click(object sender, RoutedEventArgs e)
        {
            testSearchBTN.Visibility = Visibility.Collapsed;
            ActiveDir = sourceTB.Text;
            Keyword = keywordTB.Text;
            await Task.Run(ScanDirFiles);
            System.Windows.MessageBox.Show(FileList.Count.ToString());

            testSearchBTN.Visibility = Visibility.Visible;
        }

        private async void saveIMG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ActiveRule == null)
            {
                await CreateNewRule();
                this.Close();
            }
            else
            {
                ActiveRule.SourceDir = sourceTB.Text;
                ActiveRule.Action = actionCB.Text;
                ActiveRule.DestDir = destTB.Text;
                ActiveRule.Keyword = keywordTB.Text;
                ActiveRule.Frequency = frequencyCB.Text;
                
                Storage.UpdateRule(ActiveRule);
                this.Close();
            }
        }

        private void deleteIMG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Storage.DeleteRule(ActiveRule);
            this.Close();
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

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
using FileOrganizer.Utilities;

namespace FileOrganizer.Windows
{
    /// <summary>
    /// Interaction logic for FilePromptWindow.xaml
    /// </summary>
    public partial class FilePromptWindow : Window
    {
        public FilePromptWindow Instance;

        DirectoryInfo SourceDirectory;
        List<FileInfo> NewFileList;
        List<DirectoryInfo> SubDirectories;

        public FilePromptWindow(string source)
        {
            InitializeComponent();
            
            ShowWindow(source);
        }

        private void ShowWindow(string source)
        {
            try
            {
                Instance = this;

                var settings = SettingsHelper.LoadSettings();
                Instance.Left = settings.PromptWindowLeft;
                Instance.Top = settings.PromptWindowTop;

                SourceDirectory = new DirectoryInfo(source);
                SubDirectories = SourceDirectory.GetDirectories().ToList();
                NewFileList = SourceDirectory.GetFiles().ToList();

                SubDirectories.ForEach(d => newFilesListBox.Items.Add($"{d.Name} (Dir)"));
                NewFileList.ForEach(f => newFilesListBox.Items.Add($"{f.Name}"));

                sourceLabel.Content = SourceDirectory.FullName.Length > 20 ? $"Source: ...{SourceDirectory.FullName.Substring(3, 17)}" : $"Source: {SourceDirectory.FullName}";
                contentsLabel.Content = $"Contents: {newFilesListBox.Items.Count}";

                addButton.Click += (s, e) => SelectFile();
                removeButton.Click += (s, e) => UnselectFile();

                Instance.ShowDialog();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        private void SelectFile()
        {
            selectedFilesListBox.Items.Add(newFilesListBox.SelectedItem);
            newFilesListBox.Items.Remove(newFilesListBox.SelectedItem);
        }

        private void UnselectFile()
        {
            newFilesListBox.Items.Add(selectedFilesListBox.SelectedItem);
            selectedFilesListBox.Items.Remove(selectedFilesListBox.SelectedItem);
        }

        private List<FileInfo> GetSelectedFiles()
        {
            var fileNames = new List<string>();
            foreach (var item in selectedFilesListBox.Items)
            {
                if (!item.ToString().Contains("(Dir)"))
                {
                    fileNames.Add(item.ToString());
                }
            }

            var fileList = new List<FileInfo>();
            foreach (var fileName in fileNames)
            {
               foreach(var file in NewFileList)
                {
                    if (file.Name.Contains(fileName)) fileList.Add(file);
                }
            }

            return fileList;
        }

        private List<DirectoryInfo> GetSelectedDirectories()
        {
            var dirNames = new List<string>();

            foreach (var item in selectedFilesListBox.Items)
            {
                if (item.ToString().Contains("(Dir)"))
                {
                    var dirContents = item.ToString().Split(' ').ToList();
                    dirContents.RemoveAt(dirContents.Count - 1);

                    var dirName = "";
                    dirContents.ForEach(d => dirName += d);
                    
                    dirNames.Add(dirName.TrimEnd(' '));
                }
            }

            var dirList = new List<DirectoryInfo>();
            foreach (var dirName in dirNames)
            {
                try
                {
                    var dir = new DirectoryInfo(SourceDirectory.FullName + $"\\{dirName}");
                    dirList.Add(dir);
                }
                catch (Exception ex) { LogHelper.LogError(ex); }
            }

            return dirList;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void DeleteSelected()
        {
            ScanHelper.DeleteDirectories(GetSelectedDirectories());
            ScanHelper.DeleteFiles(GetSelectedFiles());

            contentsLabel.Content = $"Count: {newFilesListBox.Items.Count}";

            selectedFilesListBox.Items.Clear();
        }

        private void MoveSelected()
        {
            var destBrowser = ElementHelper.DirectoryBrowser();
            destBrowser.ShowDialog();

            if (!String.IsNullOrEmpty(destBrowser.SelectedPath))
            {
                var dest = new DirectoryInfo(destBrowser.SelectedPath);
                var directories = GetSelectedDirectories();
                var files = GetSelectedFiles();

                //Move selected directories
                foreach (var dir in directories.ToList())
                {
                    try
                    {
                        dir.MoveTo($"{dest.FullName}\\{dir.Name}");
                    }
                    catch (Exception ex) { LogHelper.LogError(ex); }
                }

                //Move selected files
                foreach (var file in files.ToList())
                {
                    try
                    {
                        file.MoveTo($"{dest.FullName}\\{file.Name}");
                    }
                    catch (Exception ex) { LogHelper.LogError(ex); }
                }

                contentsLabel.Content = $"Count: {newFilesListBox.Items.Count}";

                selectedFilesListBox.Items.Clear();
            }
        }

        private void moveButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelected();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            try
            {
                SettingsHelper.SavePromptWindowLocation(Instance.Left, Instance.Top);

                Instance.Close();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);

                Instance.Close();
            }
        }
    }
}

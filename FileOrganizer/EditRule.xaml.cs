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
        DataHelper AppData;

        private Rule ActiveRule;

        public EditRule(MainWindow mainWin)
        {
            InitializeComponent();
            MainWin = mainWin;
            AppData = new DataHelper();
            titleLBL.Content = "New Rule";
            actionCB.SelectedIndex = 0;
            frequencyCB.SelectedIndex = 0;
            deleteBTN.Visibility = Visibility.Collapsed;
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
            frequencyCB.Text = r.Frequency;
            
            FrequencyComboBoxChanged();
            daysTB.Text = r.DayLimit.ToString();
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
            if (newRule.Frequency == "After Days")
            {
                newRule.DayLimit = Convert.ToInt32(daysTB.Text);
            }

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
            var result = System.Windows.MessageBox.Show("Are you sure you want to delete this rule?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                AppData.DeleteRule(ActiveRule);
                MainWin.ExistingRules.Remove(ActiveRule);
                MainWin.rulesDG.Items.Refresh();
                this.Close();
            }
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
                if (ActiveRule.Frequency == "After Days")
                {
                    ActiveRule.DayLimit = Convert.ToInt32(daysTB.Text);
                }

                UpdateRule();
            }
        }

        private void deleteBTN_Clicked(object sender, RoutedEventArgs e)
        {
            DeleteRule();
        }

        private void FrequencyComboBoxChanged()
        {
            switch (frequencyCB.Text)
            {
                case "After Days":
                    daysLBL.Visibility = Visibility.Visible;
                    daysTB.Visibility = Visibility.Visible;

                    keywordLBL.Margin = new Thickness(46, 245, 0, 0);
                    keywordTB.Margin = new Thickness(165, 255, 0, 0);
                    sourceLBL.Margin = new Thickness(65, 287, 0, 0);
                    sourceTB.Margin = new Thickness(165, 295, 0, 0);
                    browseBTN.Margin = new Thickness(499, 295, 0, 0);
                    destDirLBL.Margin = new Thickness(14, 328, 0, 0);
                    destTB.Margin = new Thickness(165, 335, 0, 0);
                    destBrowseBTN.Margin = new Thickness(499, 335, 0, 0);
                    break;
                default:
                    daysLBL.Visibility = Visibility.Collapsed;
                    daysTB.Visibility = Visibility.Collapsed;

                    keywordLBL.Margin = new Thickness(46, 205, 0, 0);
                    keywordTB.Margin = new Thickness(165, 215, 0, 0);
                    sourceLBL.Margin = new Thickness(65, 247, 0, 0);
                    sourceTB.Margin = new Thickness(165, 255, 0, 0);
                    browseBTN.Margin = new Thickness(499, 255, 0, 0);
                    destDirLBL.Margin = new Thickness(14, 288, 0, 0);
                    destTB.Margin = new Thickness(165, 295, 0, 0);
                    destBrowseBTN.Margin = new Thickness(499, 295, 0, 0);
                    break;
            }
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
                default:
                    destDirLBL.Visibility = Visibility.Visible;
                    destTB.Visibility = Visibility.Visible;
                    destBrowseBTN.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void frequencyCB_DropDownClosed(object sender, EventArgs e)
        {
            FrequencyComboBoxChanged();
        }
    }
}

using FileOrganizer.Utilities;
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

namespace FileOrganizer.Windows
{
    /// <summary>
    /// Interaction logic for RuleWindow.xaml
    /// </summary>
    public partial class RuleWindow : Window
    {
        public RuleWindow Instance;
        Rule ActiveRule;
        List<CheckBox> CurrentConfigOptions;
        ToolTip HelpToolTip;

        private const double CONFIG_PADDING_TOP = 10;
        private const double CONFIG_PADDING_LEFT = 10;

        public RuleWindow(Rule rule = null)
        {
            InitializeComponent();
            Instance = this;

            if (rule == null)
            {
                ShowNewRule();
            }
            else DisplayExistingRule(rule);

            HelpToolTip = new ToolTip
            {
                Content = ElementHelper.HelpToolTipText(),
                FontWeight = FontWeights.Bold,
                
            };
            helpLabel.ToolTip = HelpToolTip;

            MainWindow.Instance.ExistingRules.ForEach(x => ruleComboBox.Items.Add(x));
         
            Top = MainWindow.Instance.Top;
            Left = MainWindow.Instance.Left;

            Instance.Show();
        }

        private void ShowMove()
        {
            ShowConfig(Rule.ActionEnum.Move);
        }

        private void ShowCopy()
        {
            ShowConfig(Rule.ActionEnum.Copy);
        }

        private void ShowDelete()
        {
            destTextBox.Text = "Trash";
            ShowConfig(Rule.ActionEnum.Delete);
        }

        private void ShowDropbox()
        {
            ShowConfig(Rule.ActionEnum.DropboxCleanup);
        }

        private void ShowCompress()
        {
            ShowConfig(Rule.ActionEnum.CompressContents);
        }

        private void actionComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ActionDropDown_Closed();
        }

        void ActionDropDown_Closed()
        {
            switch (actionComboBox.Text)
            {
                case "Move":
                    ShowMove();
                    break;
                case "Copy":
                    ShowCopy();
                    break;
                case "Delete":
                    ShowDelete();
                    break;
                case "Dropbox Cleanup":
                    ShowDropbox();
                    break;
                case "Compress Contents":
                    ShowCompress();
                    break;
                default:
                    break;
            }
        }

        private void ShowConfig(Rule.ActionEnum ruleType)
        {
            CurrentConfigOptions?.ForEach(x => MainGrid.Children.Remove(x));
            CurrentConfigOptions = new List<CheckBox>();

            switch (ruleType)
            {
                case Rule.ActionEnum.Move:
                    CurrentConfigOptions = MoveConfigOptions();
                    break;
                case Rule.ActionEnum.Copy:
                    CurrentConfigOptions = CopyConfigOptions();
                    break;
                case Rule.ActionEnum.Delete:
                    CurrentConfigOptions = DeleteConfigOptions();
                    break;
                case Rule.ActionEnum.DropboxCleanup:
                    CurrentConfigOptions = DropboxConfigOptions();
                    break;
                case Rule.ActionEnum.CompressContents:
                    CurrentConfigOptions = CompressConfigOptions();
                    break;
                default:
                    //If not called with a rule type but selection is not empty, try using text
                    if (!String.IsNullOrEmpty(actionComboBox.Text))
                        switch (actionComboBox.Text)
                        {
                            case "Move":
                                CurrentConfigOptions = MoveConfigOptions();
                                break;
                            case "Copy":
                                CurrentConfigOptions = CopyConfigOptions();
                                break;
                            case "Delete":
                                CurrentConfigOptions = DeleteConfigOptions ();
                                break;
                            case "Dropbox Cleanup":
                                CurrentConfigOptions = DropboxConfigOptions();
                                break;
                            case "Compress Contents":
                                CurrentConfigOptions = CompressConfigOptions();
                                break;
                            default:
                                break;
                        }
                    break;
            }

            double nextY = -115;
            var left = configGroupBox.Margin.Left + CONFIG_PADDING_LEFT;
            foreach (var option in CurrentConfigOptions)
            {
                option.Margin = new Thickness(left, nextY, 0, 0);
                nextY = option.Margin.Top + option.Height + CONFIG_PADDING_TOP * 3;

                option.IsChecked = GetCheckBoxValue(option.Name);
                MainGrid.Children.Add(option);
            }

            var purgatoryCheckBox = CurrentConfigOptions.Find(x => x.Name == ElementHelper.DeletePurgatoryCheckBox);
            if (purgatoryCheckBox != null) purgatoryCheckBox.ToolTip = ElementHelper.PurgatoryToolTip();
        }

        List<CheckBox> MoveConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: ActiveRule.IncludeSubDirectories),
            };
        }

        List<CheckBox> CopyConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: ActiveRule.IncludeSubDirectories),
            };
        }

        List<CheckBox> DeleteConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: ActiveRule.IncludeSubDirectories),
                CheckBoxTemplate(ElementHelper.DeletePurgatoryCheckBox, "Send to purgatory"),
            };
        }

        List<CheckBox> DropboxConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include subdirectories", isChecked: ActiveRule.IncludeSubDirectories),
                CheckBoxTemplate(ElementHelper.ExcludeEmptyCheckBox, "Exclude empty directories"),
            };
        }

        List<CheckBox> CompressConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include subdirectories", isChecked: ActiveRule.IncludeSubDirectories),
                CheckBoxTemplate(ElementHelper.ExcludeEmptyCheckBox, "Exclude empty directories", isChecked: ActiveRule.ExcludeEmptyDirectories),
                CheckBoxTemplate(ElementHelper.DeleteIfSuccessfulCheckbox, "Delete contents if successful", isChecked: ActiveRule.DeleteIfSuccessful)
            };
        }

        CheckBox CheckBoxTemplate(string name, string optionText, bool isChecked = false)
        {          
            return new CheckBox
            {
                Name = name,
                Content = optionText,
                Height = 20,
                FontSize = 14,
                FontFamily = new FontFamily("Verdana"),
                IsChecked = isChecked
            };
        }

        private void FrequencyToggled ()
        {
            const double PADDING_TOP = 40;
            double nextY = 240;
            double textBoxX = 100;

            switch (frequencyComboBox.Text)
            {
                case "After Days":
                    ShowAfterDays();
                    break;
                default:
                    HideAfterDays();
                    break;
            }

            void ShowAfterDays()
            {
                daysLabel.Margin = new Thickness(
                    45, nextY - 3, 0, 0);
                daysTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                nextY += PADDING_TOP;

                sourceLabel.Margin = new Thickness(
                    29, nextY - 3, 0, 0);
                sourceTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                sourceBrowseButton.Margin = new Thickness(
                    295, nextY + 4, 0, 0);
                nextY += PADDING_TOP;

                destLabel.Margin = new Thickness(
                    48, nextY-3, 0, 0);
                destTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                destBrowseButton.Margin = new Thickness(
                    295, nextY + 4, 0, 0);
                nextY += PADDING_TOP;
                    

                daysLabel.Visibility = Visibility.Visible;
                daysTextBox.Visibility = Visibility.Visible;
            }

            void HideAfterDays()
            {
                daysLabel.Visibility = Visibility.Collapsed;
                daysTextBox.Visibility = Visibility.Collapsed;

                sourceLabel.Margin = new Thickness(
                29, nextY - 3, 0, 0);
                sourceTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                sourceBrowseButton.Margin = new Thickness(
                    295, nextY + 4, 0, 0);
                nextY += PADDING_TOP;

                destLabel.Margin = new Thickness(
                    48, nextY - 3, 0, 0);
                destTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                destBrowseButton.Margin = new Thickness(
                    295, nextY + 4, 0, 0);
                nextY += PADDING_TOP;
            }
        }

        private void frequencyComboBox_DropDownClosed(object sender, EventArgs e)
        {
            FrequencyToggled();
        }

        private bool GetCheckBoxValue(string checkBoxName)
        {
            var configOption = CurrentConfigOptions.Find(x => x.Name == checkBoxName);
            return configOption.IsChecked.HasValue && configOption.IsChecked.Value == true;
        }

        private void finishButton_Clicked(object sender, RoutedEventArgs e)
        {
            SaveActiveRule(false);
        }

        private void SaveActiveRule(bool isClosing = false)
        {           
            ActiveRule.Name = nameTextBox.Text;
            ActiveRule.SetAction(actionComboBox.Text);
            ActiveRule.SetFrequency(frequencyComboBox.Text);
            ActiveRule.SourceDir = sourceTextBox.Text;
            ActiveRule.DestDir = destTextBox.Text;
            ActiveRule.SetKeywords(keywordsTextBox.Text);

            if (ActiveRule.Frequency == Rule.FrequencyEnum.AfterDays)
            {
                ActiveRule.DayLimit = Convert.ToInt32(daysTextBox.Text);
            }

            //Set config options by checking active check box values
            ActiveRule.IncludeSubDirectories = GetCheckBoxValue(ElementHelper.SubDirCheckBox);
            ActiveRule.ExcludeEmptyDirectories = GetCheckBoxValue(ElementHelper.ExcludeEmptyCheckBox);
            ActiveRule.DeleteIfSuccessful = GetCheckBoxValue(ElementHelper.DeleteIfSuccessfulCheckbox);

            if (ActiveRule.ID > 0) MainWindow.Instance.AppData.UpdateRule(ActiveRule);
            else
            {
                MainWindow.Instance.AppData.CreateRule(ActiveRule);
                MainWindow.Instance.ExistingRules.Add(ActiveRule);
                ruleComboBox.Items.Add(ActiveRule);
            }

            MainWindow.Instance.RulesDataGrid.Items.Refresh();

            if (isClosing) CloseWindow();
            else DisplayExistingRule(ActiveRule);
        }

        private void sourceBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceBrowser = ElementHelper.DirectoryBrowser(sourceTextBox.Text);
            sourceBrowser.ShowDialog();

            sourceTextBox.Text = sourceBrowser.SelectedPath;         
        }

        private void destBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var destBrowser = ElementHelper.DirectoryBrowser(sourceTextBox.Text);
            destBrowser.ShowDialog();

            destTextBox.Text = destBrowser.SelectedPath;
        }

        private void CloseWindow()
        {
            MainWindow.Instance.RulesDataGrid.Items.Refresh();

            this.Close();
        }

        private void ruleComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (ruleComboBox.Text)
            {
                case "New Rule":
                    ShowNewRule();
                    break;
                default:
                    DisplayExistingRule((Rule) ruleComboBox.SelectedItem);
                    break;
            }

            FrequencyToggled();
        }

        private void ShowNewRule()
        {
            ruleComboBox.SelectedIndex = 0;

            titleLabel.Content = "Create Rule";
            ActiveRule = new Rule();

            actionComboBox.SelectedIndex = 0;
            ShowMove();

            frequencyComboBox.SelectedIndex = 3;
            FrequencyToggled();

            deleteButton.Visibility = Visibility.Collapsed;
            saveButton.Visibility = Visibility.Visible;

            nameTextBox.Text = $"New {actionComboBox.Text} Rule";
            sourceTextBox.Text = "";
            destTextBox.Text = "";
            keywordsTextBox.Text = "";

            nameTextBox.Focus();
            nameTextBox.SelectAll();
        }

        private void DisplayExistingRule(Rule rule)
        {
            titleLabel.Content = "Edit Rule";
            ActiveRule = rule;

            ruleComboBox.SelectedItem = ActiveRule;
            nameTextBox.Text = ActiveRule.Name;
            sourceTextBox.Text = ActiveRule.SourceDir;
            destTextBox.Text = ActiveRule.DestDir;

            switch (ActiveRule.Action)
            {
                case Rule.ActionEnum.Move:
                    actionComboBox.SelectedIndex = 0;
                    ShowMove();
                    break;
                case Rule.ActionEnum.Copy:
                    actionComboBox.SelectedIndex = 1;
                    ShowCopy();
                    break;
                case Rule.ActionEnum.Delete:
                    actionComboBox.SelectedIndex = 2;
                    ShowDelete();
                    break;
                case Rule.ActionEnum.DropboxCleanup:
                    actionComboBox.SelectedIndex = 3;
                    ShowDropbox();
                    break;
                case Rule.ActionEnum.CompressContents:
                    actionComboBox.SelectedIndex = 4;
                    ShowCompress();
                    break;
            }

            switch (ActiveRule.Frequency)
            {
                case Rule.FrequencyEnum.AfterDays:
                    frequencyComboBox.SelectedIndex = 0;
                    daysTextBox.Text = ActiveRule.DayLimit.ToString();
                    break;
                case Rule.FrequencyEnum.Hourly:
                    frequencyComboBox.SelectedIndex = 1;
                    break;
                case Rule.FrequencyEnum.Daily:
                    frequencyComboBox.SelectedIndex = 2;
                    break;
                case Rule.FrequencyEnum.Weekly:
                    frequencyComboBox.SelectedIndex = 3;
                    break;
                case Rule.FrequencyEnum.Monthly:
                    frequencyComboBox.SelectedIndex = 4;
                    break;
            }
            FrequencyToggled();

            var keywords = string.Empty;
            foreach (var keyword in ActiveRule.Keywords)
            {
                if (!String.IsNullOrEmpty(keyword)) keywords += $"{keyword}, ";
            }
            keywords.TrimEnd(' ');
            keywordsTextBox.Text = keywords;

            deleteButton.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Visible;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveRule != null)
            {
                if (ActiveRule.ID > 0) DeleteActiveRule();
                else ShowNewRule();
            }
        }

        private void DeleteActiveRule()
        {
            MainWindow.Instance.AppData.DeleteRule(ActiveRule);
            MainWindow.Instance.ExistingRules.Remove(ActiveRule);
            MainWindow.Instance.RulesDataGrid.Items.Refresh();
            ruleComboBox.Items.Remove(ActiveRule);

            ShowNewRule();
        }
    }
}

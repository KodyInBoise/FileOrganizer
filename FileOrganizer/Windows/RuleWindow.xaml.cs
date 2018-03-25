﻿using FileOrganizer.Utilities;
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
                ruleComboBox.SelectedIndex = 0;
                ActiveRule = ShowNewRule();
            }
            else ActiveRule = rule;

            HelpToolTip = new ToolTip
            {
                Content = ElementHelper.HelpToolTip(),
                FontWeight = FontWeights.SemiBold,
            };
            helpLabel.ToolTip = HelpToolTip;

            Instance.Show();
        }

        private void ShowMove()
        {
            nameTextBox.Text = "Move";
            ShowConfig(Rule.ActionEnum.Move);
        }

        private void ShowCopy()
        {
            nameTextBox.Text = "Copy";
            ShowConfig(Rule.ActionEnum.Copy);
        }

        private void ShowDelete()
        {
            nameTextBox.Text = "Delete";
            ShowConfig(Rule.ActionEnum.Delete);
        }

        private void ShowDropbox()
        {
            nameTextBox.Text = "Dropbox";
            ShowConfig(Rule.ActionEnum.DropboxCleanup);
        }

        private void actionComboBox_DropDownClosed(object sender, EventArgs e)
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
                MainGrid.Children.Add(option);
            }
        }

        List<CheckBox> MoveConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: true),
            };
        }

        List<CheckBox> CopyConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: true),
            };
        }

        List<CheckBox> DeleteConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include Subdirectories", isChecked: true),
                CheckBoxTemplate(ElementHelper.DeletePurgatoryCheckBox, "Send to purgatory"),
            };
        }

        List<CheckBox> DropboxConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementHelper.SubDirCheckBox, "Include subdirectories"),
                CheckBoxTemplate(ElementHelper.ExcludeEmptyCheckBox, "Exclude empty directories"),
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

        private Rule ShowNewRule()
        {
            actionComboBox.SelectedIndex = 0;
            ShowMove();

            frequencyComboBox.SelectedIndex = 0;
            FrequencyToggled();

            return new Rule();
        }

        private bool GetCheckBoxValue(string checkBoxName)
        {
            var configOption = CurrentConfigOptions.Find(x => x.Name == checkBoxName);
            return configOption.IsChecked.HasValue && configOption.IsChecked.Value == true;
        }

        private void finishButton_Clicked(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
        }

        private void CreateNewRule()
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

            ActiveRule.IncludeSubDirectories = GetCheckBoxValue(ElementHelper.SubDirCheckBox);

            MainWindow.Instance.AppData.CreateRule(ActiveRule);
            MainWindow.Instance.ExistingRules.Add(ActiveRule);
            
            CloseWindow();
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
            MainWindow.Instance.rulesDG.Items.Refresh();

            this.Close();
        }
    }
}

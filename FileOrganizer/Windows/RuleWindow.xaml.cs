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

        private const double CONFIG_PADDING_TOP = 10;
        private const double CONFIG_PADDING_LEFT = 10;

        public RuleWindow(Rule rule = null)
        {
            InitializeComponent();
            Instance = this;
            ActiveRule = rule ?? new Rule();
            if (rule == null)
            {
                ActiveRule = new Rule();
                ruleComboBox.SelectedIndex = 0;
                actionComboBox.SelectedIndex = 0;
                ShowMove();
            }

            Instance.Show();
        }

        private void ShowMove()
        {
            nameTextBox.Text = "Move";
            ShowConfig(Rule.RuleType.Move);
        }

        private void ShowCopy()
        {
            nameTextBox.Text = "Copy";
            ShowConfig(Rule.RuleType.Copy);
        }

        private void ShowDelete()
        {
            nameTextBox.Text = "Delete";
            ShowConfig(Rule.RuleType.Delete);
        }

        private void ShowDropbox()
        {
            nameTextBox.Text = "Dropbox";
            ShowConfig(Rule.RuleType.DropboxCleanup);
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

        private void ShowConfig(Rule.RuleType ruleType)
        {
            CurrentConfigOptions?.ForEach(x => MainGrid.Children.Remove(x));
            CurrentConfigOptions = new List<CheckBox>();

            switch (ruleType)
            {
                case Rule.RuleType.Move:
                    CurrentConfigOptions = MoveConfigOptions();
                    break;
                case Rule.RuleType.Copy:
                    CurrentConfigOptions = CopyConfigOptions();
                    break;
                case Rule.RuleType.Delete:
                    CurrentConfigOptions = DeleteConfigOptions();
                    break;
                case Rule.RuleType.DropboxCleanup:
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

            double nextY = -100;//configGroupBox.Margin.Top + CONFIG_PADDING_TOP;
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
                CheckBoxTemplate(ElementNames.SubDirCheckBox, "Include Subdirectories"),
            };
        }

        List<CheckBox> CopyConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementNames.SubDirCheckBox, "Include Subdirectories"),
            };
        }

        List<CheckBox> DeleteConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementNames.SubDirCheckBox, "Include Subdirectories"),
                CheckBoxTemplate(ElementNames.DeletePurgatoryCheckBox, "Send to purgatory"),
            };
        }

        List<CheckBox> DropboxConfigOptions()
        {
            return new List<CheckBox>()
            {
                CheckBoxTemplate(ElementNames.SubDirCheckBox, "Include subdirectories"),
                CheckBoxTemplate(ElementNames.ExcludeEmptyDirs, "Exclude empty directories"),
            };
        }

        CheckBox CheckBoxTemplate(string name, string optionText)
        {          
            return new CheckBox
            {
                Name = name,
                Content = optionText,
                Height = 15,
                FontSize = 14,
                FontFamily = new FontFamily("Verdana")
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ShowConfig();
        }

        private void FrequencyToggled ()
        {
            const double PADDING_TOP = 40;
            double nextY = 280;
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
                nextY += PADDING_TOP;

                destLabel.Margin = new Thickness(
                    48, nextY-3, 0, 0);
                destTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                nextY += PADDING_TOP;
                    

                daysLabel.Visibility = Visibility.Visible;
                daysTextBox.Visibility = Visibility.Visible;
            }

            void HideAfterDays()
            {
                sourceLabel.Margin = new Thickness(
                29, nextY - 3, 0, 0);
                sourceTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                nextY += PADDING_TOP;

                destLabel.Margin = new Thickness(
                    48, nextY - 3, 0, 0);
                destTextBox.Margin = new Thickness(
                    textBoxX, nextY, 0, 0);
                nextY += PADDING_TOP;


                daysLabel.Visibility = Visibility.Collapsed;
                daysTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void frequencyComboBox_DropDownClosed(object sender, EventArgs e)
        {
            FrequencyToggled();
        }
    }
}

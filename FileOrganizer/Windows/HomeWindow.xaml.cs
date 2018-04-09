using FileOrganizer.Utilities;
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
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow Instance;

        public List<Rule> ExistingRules;

        public HomeWindow()
        {
            InitializeComponent();

            ShowWindow();
        }

        private void ShowWindow()
        {
            Instance = this;

            UpdateRulesListBox(true);
            Instance.Show();
        }

        private async void UpdateRulesListBox(bool reloadRules = false)
        {
            if (reloadRules) ExistingRules = await DataHelper.GetAllRules();

            ExistingRules.ForEach(x => rulesListBox.Items.Add(CreateRuleItem(x)));
        }

        private ListBoxItem CreateRuleItem(Rule rule)
        {
            var ruleDisplayString = "";

            ruleDisplayString += $"{rule.Name} | ";
            ruleDisplayString += $"Source: {TrimPath(rule.SourceDir)} | ";
            ruleDisplayString += $"Dest: {TrimPath(rule.DestDir)} | ";

            return new ListBoxItem
            {
                Content = ruleDisplayString
            };
        }

        private string TrimPath(string path)
        {
            var pathLength = path.Length;
            if (pathLength > 17)
            {
                path = path.Remove(0, (pathLength - 17));
                path = "..." + path;
            }

            return path;
        }
    }
}

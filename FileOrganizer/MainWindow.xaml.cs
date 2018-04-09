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
using System.Windows.Forms;
using System.Windows.Threading;
using System.Drawing;
using System.Net;
using System.Threading;
using FileOrganizer.Utilities;
using FileOrganizer.Windows;

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }

        private NotifyIcon ProgramIcon;
        public ScanHelper Scanner;
        public List<Rule> ExistingRules;

        CollectionViewSource itemCollectionViewSource;
        DispatcherTimer ScanTimer;


        public DataHelper AppData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Startup();
        }

        private void CreateRuleGrid()
        {
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = ExistingRules;
        }

        private void editBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRule = GetSelectedRule();

                var ruleWindow = new RuleWindow(selectedRule);
            }
            catch (Exception ex)
            {
                ErrorHelper.Handle(ex);
            }
        }

        private Rule GetSelectedRule()
        {
            try
            {
                DataGridCellInfo cellInfo = RulesDataGrid.SelectedCells[0];
                if (cellInfo == null) return null;

                DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
                if (column == null) return null;

                FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
                BindingOperations.SetBinding(element, TagProperty, column.Binding);
                var name = element.Tag.ToString();

                foreach (Rule r in ExistingRules)
                {
                    if (r.Name == name)
                    {
                        return r;
                    }
                }

                return null;
            }
            catch (Exception ex) { LogHelper.LogError(ex); return null; }
        }


        private void newRuleBTN_Click(object sender, RoutedEventArgs e)
        {
            var ruleWindow = new RuleWindow();
            CreateRuleGrid();
        }

        private async void runBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rule = GetSelectedRule();
                var result = await ExecuteRule(rule);
                if (result == "Success")
                {
                    AppData.UpdateRule(rule);
                }
            }
            catch (Exception ex)
            {
                HandleError(exception: ex);
            }
        }

        private void StartTimer()
        {
            ScanTimer = new DispatcherTimer();
            ScanTimer.Interval = new TimeSpan(0, 0, 5);
            ScanTimer.Tick += (s, e) =>
            {
                Task.Run(ScanRules);
            };
            ScanTimer.Start();
        }

        private async Task ScanRules()
        {
            try
            {
                foreach (var rule in ExistingRules)
                {
                    var threshold = rule.GetThreshold();
                    if (threshold > 0 && rule.Counter >= threshold)
                    {
                        rule.ExecuteAction();
                    }
                    rule.Counter++;
                    AppData.UpdateAllRules(ExistingRules);
                }
            }
            catch (Exception ex) { LogHelper.LogError(ex); }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Collapsed;
        }

        private async void Startup()
        {
            try
            {
                Instance = this;
                AppData = new DataHelper();
                Scanner = new ScanHelper();
                ExistingRules = await DataHelper.GetAllRules();

                var trayIconPath = $"{Directory.GetCurrentDirectory()}\\main.ico";
                ProgramIcon = new NotifyIcon
                {
                    Icon = new Icon(trayIconPath),
                    Visible = true
                };
                ProgramIcon.Click += new EventHandler(trayIcon_Clicked);

                CreateRuleGrid();
                StartTimer();
            }
            catch (Exception ex) { LogHelper.LogError(ex); }
        }

        private void trayIcon_Clicked(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void quitBTN_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void Shutdown()
        {
            ProgramIcon.Dispose();
            Environment.Exit(0);
        }

        private async Task<string> ExecuteRule(Rule rule)
        {
            try
            {
                rule.ExecuteAction();
                return "Success";
            }
            catch
            {
                return "Failed";
            }
        }

        public void HandleError(Exception exception = null, Rule rule = null)
        {
            Task.Run(() => LogHelper.LogError(exception: exception, ruleName: rule?.Name));
        }

        public void LogActivity(Rule rule = null, bool success = true, string message = "")
        {
            Task.Run(() => LogHelper.LogActivity(rule: rule, success: success, message: message));
        }

        private void logsButton_Click(object sender, RoutedEventArgs e)
        {
            var logsWindow = new LogWindow();
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

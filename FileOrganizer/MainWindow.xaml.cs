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

namespace FileOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon ProgramIcon;
        public ScanHelper FileScanner;
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
            StartTimer();
        }

        private void editBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRule = GetSelectedRule();

                EditRule editRuleWin = new EditRule(this, selectedRule) { Owner = this };
                editRuleWin.ShowDialog();
                CreateRuleGrid();
            }
            catch
            {

            }
        }

        private Rule GetSelectedRule()
        {
            DataGridCellInfo cellInfo = rulesDG.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, TagProperty, column.Binding);
            var timestamp = element.Tag.ToString();

            foreach (Rule r in ExistingRules)
            {
                if (r.ModifiedTimestamp == timestamp)
                {
                    return r;
                }
            }

            return null;
        }

        private void newRuleBTN_Click(object sender, RoutedEventArgs e)
        {
            EditRule newRuleWin = new EditRule(this) { Owner = this };
            newRuleWin.ShowDialog();
            CreateRuleGrid();
        }

        private void viewFilesBTN_Click(object sender, RoutedEventArgs e)
        {
            var active = GetSelectedRule();
            var files = active.GetFiles();
            string s = "";

            foreach (FileInfo f in files)
            {
                s += f.FullName + Environment.NewLine;
            }

            System.Windows.MessageBox.Show(s);
        }

        private async void runBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rule = GetSelectedRule();
                var result = await ExecuteRule(rule);
                if (result == "Success")
                {
                    AppData.UpdateRule(rule, true);
                }
            }
            catch
            {
                //Need to handle silently when ready to release
                throw;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Collapsed;
        }

        private async void Startup()
        {
            AppData = new DataHelper();
            ExistingRules = await AppData.GetAllRules();

            var trayIconPath = $"{Directory.GetCurrentDirectory()}\\main.ico";
            ProgramIcon = new NotifyIcon
            {
                Icon = new Icon(trayIconPath),
                Visible = true
            };
            ProgramIcon.Click += new EventHandler(trayIcon_Clicked);

            CreateRuleGrid();
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
    }
}

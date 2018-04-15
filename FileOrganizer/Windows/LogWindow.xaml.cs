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
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow Instance;

        public LogWindow()
        {
            InitializeComponent();
            Instance = this;

            Instance.Left = MainWindow.Instance.Left;
            Instance.Top = MainWindow.Instance.Top;

            Instance.Show();
            DisplayActivityLog();
        }

        List<LogHelper.LogEntry> _activityEntries;
        public void DisplayActivityLog()
        {
            _activityEntries = DataHelper.GetActivityEntries();
            _activityEntries.Reverse();

            logDataGrid.ItemsSource = _activityEntries;
            logDataGrid.Items.Refresh();
        }

        List<LogHelper.LogEntry> _errorEntries;
        public void DisplayErrorLog()
        {
            _errorEntries = DataHelper.GetErrorEntries();
            _errorEntries.Reverse();

            logDataGrid.ItemsSource = _errorEntries;
            logDataGrid.Items.Refresh();
        }

        bool _activitySelected = false;
        private void activity_Focused(object sender, RoutedEventArgs e)
        {
            if (!_activitySelected)
            {
                DisplayActivityLog();
                _activitySelected = true;
                _errorsSelected = false;
            }
        }

        bool _errorsSelected = false;
        private void errors_Focused(object sender, RoutedEventArgs e)
        {
           if (!_errorsSelected)
            {
                DisplayErrorLog();
                _errorsSelected = true;
                _activitySelected = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileOrganizer.Utilities
{
    class ErrorHelper
    {
        public static void Handle(Exception ex = null)
        {
            MessageBox.Show($"An error has occurred: \r {ex.Message}");
        }
    }
}

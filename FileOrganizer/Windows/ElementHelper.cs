﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileOrganizer.Windows
{
    public class ElementHelper
    {
        public static string SubDirCheckBox = "subDirCheckBox";
        public static string DeletePurgatoryCheckBox = "purgatoryCheckBox";
        public static string ExcludeEmptyCheckBox = "excludeEmptyCheckBox";
        public static string DeleteIfSuccessfulCheckbox = "deleteIfSuccessfulCheckBox";

        public static string HelpToolTipText()
        {
            var toolTipText = string.Empty;

            toolTipText += "-Use commas to separate multiple keywords" + Environment.NewLine;
            toolTipText += "-Leave blank for all files and / or directories";

            return toolTipText;
        }

        public static System.Windows.Controls.ToolTip PurgatoryToolTip()
        {
            var toolTip = new System.Windows.Controls.ToolTip();
            toolTip.Content = "Items sent to Purgatory will be sent to"  +
                "a temp directory for 30 days located at:" + Environment.NewLine +
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "File Organizer", "Purgatory");

            return toolTip;
        }

        public static FolderBrowserDialog DirectoryBrowser(string path = "")
        {
            var dialog = new FolderBrowserDialog();

            if (!String.IsNullOrEmpty(path))
            {
                var defaultDirectory = new DirectoryInfo(path);
                dialog.SelectedPath = defaultDirectory.Exists ? defaultDirectory.FullName : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            return dialog;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Utilities
{
    class FileHelper
    {
        private string Directory { get; set; }

        public FileHelper(Rule rule)
        {
            Directory = rule.SourceDir;
        }

        public static List<FileInfo> GetFiles(string directory)
        {
            return new DirectoryInfo(directory).GetFiles().ToList();
        }
    }
}

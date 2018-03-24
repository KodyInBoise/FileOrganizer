using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOrganizer.Utilities
{
    public class ScanHelper
    {
        public bool IsActive { get; set; }
        public List<Rule> RuleList { get; set; }

        public static string GetDropboxPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Dropbox");

            if (Directory.Exists(path)) return path;
            else return string.Empty;
        }

        public static List<DirectoryInfo> GetDropboxDirectories(string keyword = "", bool excludeEmpty = false)
        {
            var dropboxDir = new DirectoryInfo(GetDropboxPath());
            var subDirs = dropboxDir.GetDirectories().ToList();

            var dirCache = subDirs.Find(x => x.Name.Contains("dropbox.cache"));
            if (dirCache != null) subDirs.Remove(dirCache);

            if (!String.IsNullOrEmpty(keyword))
            {
                subDirs = subDirs.FindAll(x => x.Name.Contains(keyword));
            }

            if (excludeEmpty)
            {
                var emptyDirs = subDirs.FindAll(x => x.GetFileSystemInfos().Count() == 0);
                emptyDirs.ForEach(x => subDirs.Remove(x));
            }

            return subDirs;
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

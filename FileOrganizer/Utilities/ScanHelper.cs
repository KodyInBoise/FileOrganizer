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

        public static List<DirectoryInfo> GetDropboxDirectories(bool excludeEmpty = false)
        {
            var dropboxDir = new DirectoryInfo(GetDropboxPath());
            var subDirs = dropboxDir.GetDirectories().ToList();

            var dirCache = subDirs.Find(x => x.Name.Contains("dropbox.cache"));
            if (dirCache != null) subDirs.Remove(dirCache);

            if (excludeEmpty)
            {
                var emptyDirs = subDirs.FindAll(x => x.GetFileSystemInfos().Count() == 0);
                emptyDirs.ForEach(x => subDirs.Remove(x));
            }

            return subDirs;
        }


    }
}

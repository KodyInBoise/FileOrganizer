using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace FileOrganizer.Utilities
{
    public class ScanHelper
    {
        public bool IsActive { get; set; }
        public List<Rule> RuleList { get; set; }

        public static string GetPurgatoryPath()
        {
            var purgatoryPath = Path.Combine(DataHelper.GetRootPath(), "Purgatory");
            if (!Directory.Exists(purgatoryPath)) Directory.CreateDirectory(purgatoryPath);

            return purgatoryPath;
        }

        public static string DefaultDropboxPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Dropbox");

            if (Directory.Exists(path)) return path;
            else return string.Empty;
        }

        public static List<DirectoryInfo> GetDropboxDirectories(string path = "", string keyword = "", bool excludeEmpty = false)
        {
            var dropboxDir = new DirectoryInfo(!String.IsNullOrEmpty(path) ? path : DefaultDropboxPath());
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
                file.CopyTo(temppath, true);
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

        public async void CopyToPurgatory(string sourceDir)
        {
            try
            {
                var directory = new DirectoryInfo(sourceDir);
                if (directory.Exists && Directory.Exists(GetPurgatoryPath()))
                {
                    await Task.Run(() => CopyDirectory(sourceDir, GetPurgatoryPath(), true));

                    MainWindow.Instance.LogActivity(success: true, message: $"Directory moved to Purgatory: {sourceDir}");
                }
            }
            catch (Exception ex)
            {
                var message = $"An error occurred moving directory to purgatory: {ex.Message}";
                MainWindow.Instance.HandleError(exception: new Exception(message));
            }
        }

        public List<DirectoryInfo> GetSubDirectories(string source)
        {
            try
            {
                var directory = new DirectoryInfo(source);

                if (directory.Exists) return directory.GetDirectories().ToList();
                else return new List<DirectoryInfo>();
            }
            catch (Exception ex)
            {
                MainWindow.Instance.HandleError(exception: ex);

                return new List<DirectoryInfo>();
            }
        }

        public List<FileInfo> GetDirectoryFiles(string source, List<string> keywords = null)
        {
            try
            {
                var directory = new DirectoryInfo(source);
                if (!directory.Exists) return new List<FileInfo>();

                if (keywords == null)
                {
                    return directory.GetFiles().ToList();
                }
                else
                {
                    var allFiles = directory.GetFiles().ToList();
                    var matchingFiles = new List<FileInfo>();

                    foreach (var file in allFiles)
                    {
                        var matches = 0;
                        foreach (var keyword in keywords)
                        {
                            if (file.Name.Contains(keyword))
                            {
                                matches++;
                                if (matches == keywords.Count) matchingFiles.Add(file);
                            }
                        }
                    }

                    return matchingFiles;
                }
            }
            catch (Exception ex)
            {
                MainWindow.Instance.HandleError(exception: ex);

                return new List<FileInfo>();
            }
        }

        public static void CompressDirectory(string source, string dest)
        {
            ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Optimal, false);
        }

        public static void DeleteFiles(List<FileInfo> files)
        {
            foreach (var file in files.ToList())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex) { LogHelper.LogError(ex); }
            }
        }

        public static void DeleteDirectories(List<DirectoryInfo> directories)
        {
            foreach (var dir in directories.ToList())
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception ex) { LogHelper.LogError(ex); }
            }
        }
    }
}

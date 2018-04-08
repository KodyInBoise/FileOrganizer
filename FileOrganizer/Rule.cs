using FileOrganizer.Utilities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FileOrganizer
{
    public class Rule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime ModifiedTimestamp { get; set; }
        public string SourceDir { get; set; }
        public string DestDir { get; set; }
        public string Keyword { get; set; }
        public int Counter { get; set; }
        public int DayLimit { get; set; }
        public ActionEnum Action { get; set; }
        public FrequencyEnum Frequency { get; set; }
        public List<string> Keywords { get; set; }
        public bool IncludeSubDirectories { get; set; }

        public enum ActionEnum
        {
            Move,
            Copy,
            Delete,
            DropboxCleanup,
            CompressFiles
        }

        public enum FrequencyEnum
        {
            AfterDays,
            Hourly,
            Daily,
            Weekly,
            Monthly
        }

        [BsonIgnore]
        public List<FileInfo> FileList { get; set; }
        [BsonIgnore]
        public DispatcherTimer Timer { get; set; }
        [BsonIgnore]
        public string FrequencyString { get; set; }
        [BsonIgnore]
        public string ActionString { get; set; }

        public int GetThreshold()
        {
            var threshold = 0;
            switch (FrequencyString)
            {
                case "After Days":
                    threshold = 1440;
                    break;
                case "Hourly":
                    threshold = 60;
                    break;
                case "Daily":
                    threshold = 1440;
                    break;
                case "Weekly":
                    threshold = 10080;
                    break;
                case "Monthly":
                    threshold = 43800;
                    break;
                default:
                    threshold = -1;
                    break;
            }

            return threshold;
        }

        public List<FileInfo> GetFiles()
        {
            List<FileInfo> tmpList = new List<FileInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo(SourceDir);
            var files = dirInfo.GetFiles().ToList();

            foreach (var subDir in dirInfo.GetDirectories())
            {
                var subDirFiles = subDir.GetFiles().ToList();
                files.AddRange(subDirFiles);
            }

            foreach (FileInfo f in files)
            {
                var name = f.ToString().ToLower();
                Keyword = Keyword.ToLower();
                if (name.Contains(Keyword))
                {
                    tmpList.Add(f);
                }
            }

            return tmpList;
        }

        public List<FileInfo> GetAllFiles()
        {
            var allFiles = new List<FileInfo>();

            try
            {
            var baseDir = new DirectoryInfo(SourceDir);
            var subDirs = baseDir.GetDirectories();

                allFiles.AddRange(baseDir.GetFiles());
                foreach (var dir in subDirs)
                {
                    var subFiles = dir.GetFiles();
                    allFiles.AddRange(subFiles);
                }
            }

            catch { }

            return allFiles;
        }

        public async void ExecuteAction()
        {
            try
            {
                FileList = GetAllFiles();

                switch (Action)
                {
                    case ActionEnum.Move:
                        await Task.Run(MoveFiles);
                        break;
                    case ActionEnum.Copy:
                        await Task.Run(CopyFiles);
                        break;
                    case ActionEnum.Delete:
                        await Task.Run(DeleteFiles);
                        break;
                    case ActionEnum.DropboxCleanup:
                        await Task.Run(CleanupDropbox);
                        break;
                    case ActionEnum.CompressFiles:
                        await Task.Run(CompressContents);
                        break;
                    default:
                        break;
                }

                Counter = 0;

                MainWindow.Instance.LogActivity(rule: this, success: true, message: "Rule executed successfully");
            }

            catch (Exception ex) { MainWindow.Instance.HandleError(exception: ex, rule: this); }
        }

        private async Task MoveFiles()
        {
            foreach (FileInfo f in FileList)
            {
                try
                {
                    f.CopyTo($"{DestDir}\\{f.Name}", true);
                    f.Delete();
                }
                catch { }
            }
        }

        private async Task DeleteFiles()
        {
            foreach (FileInfo f in FileList)
            {
                try
                {
                    f.Delete();
                }
                catch { }
            }
        }

        private async Task CopyFiles()
        {
            foreach (FileInfo f in FileList)
            {
                try
                {
                    if (FrequencyString == "After Days")
                    {
                        if (!FileOldEnough(f)) break;
                    }
                    f.CopyTo($"{DestDir}\\{f.Name}", true);
                }
                catch { }
            }
        }

        private async Task CleanupDropbox()
        {
            if (!Directory.Exists(DestDir)) throw new Exception("Destination directory not available");

            var dropboxDirs = ScanHelper.GetDropboxDirectories(path: SourceDir, keyword: Keyword, excludeEmpty: true);

            foreach (var dir in dropboxDirs)
            {
                ScanHelper.CopyDirectory(dir.FullName, Path.Combine(DestDir, dir.Name), true);
                dir.Attributes = FileAttributes.Normal;
                dir.Delete(true);
            }
        }

        private async Task CompressContents()
        {
            ScanHelper.CompressDirectory(SourceDir, DestDir + "\\test.zip");
        }

        private bool FileOldEnough(FileInfo file)
        {
            try
            {
                var threshold = file.CreationTime.AddDays(DayLimit);
                if (DateTime.Now > threshold) return true;
                else return false;
            }
            catch { return false; }
        }

        public void SetFrequency(string frequency)
        {
            switch (frequency)
            {
                case "After Days":
                    Frequency = FrequencyEnum.AfterDays;
                    break;
                case "Hourly":
                    Frequency = FrequencyEnum.Hourly;
                    break;
                case "Daily":
                    Frequency = FrequencyEnum.Daily;
                    break;
                case "Weekly":
                    Frequency = FrequencyEnum.Weekly;
                    break;
                case "Monthly":
                    Frequency = FrequencyEnum.Monthly;
                    break;
            }
        }

        public void SetAction(string action)
        {
            switch (action)
            {
                case "Move":
                    Action = ActionEnum.Move;
                    break;
                case "Copy":
                    Action = ActionEnum.Copy;
                    break;
                case "Delete":
                    Action = ActionEnum.Delete;
                    break;
                case "Dropbox Cleanup":
                    Action = ActionEnum.DropboxCleanup;
                    break;
                case "Compress Contents":
                    Action = ActionEnum.CompressFiles;
                    break;
            }
            Counter = 0;
        }

        public void SetKeywords(string keywordsText)
        {
            Keywords = new List<string>();

            var keywords = keywordsText.Split(',').ToList();
            foreach (var keyword in keywords)
            {
                if (!String.IsNullOrWhiteSpace(keyword)) Keywords.Add(keyword);
            }
        }

        public Rule SaveFormat()
        {
            return new Rule
            {
                ID = this.ID,
                Name = this.Name,
                Action = this.Action,
                Frequency = this.Frequency,
                DayLimit = this.DayLimit,
                Counter = this.Counter,
                SourceDir = this.SourceDir,
                DestDir = this.DestDir,
                Keywords = this.Keywords,
                IncludeSubDirectories = this.IncludeSubDirectories,
                ModifiedTimestamp = this.ModifiedTimestamp, 
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

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
        public string ModifiedTimestamp { get; set; }
        public string SourceDir { get; set; }
        public string DestDir { get; set; }
        public string Action { get; set; }
        public string Keyword { get; set; }
        public string Frequency { get; set; }
        public int Counter { get; set; }

        [BsonIgnore]
        public List<FileInfo> FileList { get; set; }
        public DispatcherTimer Timer { get; set; }

        public int GetThreshold()
        {
            var threshold = 0;
            switch (Frequency)
            {
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
                FileList = GetFiles();

                switch (Action)
                {
                    case "Delete":
                        await Task.Run(DeleteFiles);
                        break;
                    case "Move":
                        await Task.Run(MoveFiles);
                        break;
                    case "Copy":
                        await Task.Run(CopyFiles);
                        break;
                }

                Counter = 0;

                await Task.Run(() => LogHelper.LogAction(this, true, $"{FileList?.Count} files affected"));
            }

            catch (Exception ex) { await Task.Run(() => LogHelper.LogAction(this, false, ex.Message)); }
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
                    f.CopyTo($"{DestDir}\\{f.Name}", true);
                }
                catch { }
            }
        }
    }
}

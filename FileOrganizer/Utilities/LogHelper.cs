using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileOrganizer.Utilities;
using LiteDB;

namespace FileOrganizer
{
    public class LogHelper
    {
        public class LogEntry
        {
            public int ID { get; set; }
            public DateTime TimeStamp { get; set; }
            public string RuleName { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public static LiteDatabase GetDatabase()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "File Organizer\\data.fo");
            return new LiteDatabase(path);
        }

        public static async Task LogActivity(Rule rule, bool success = true, string message = "")
        {
            var data = GetDatabase();
            var newEntry = new LogEntry
            {
                TimeStamp = DateTime.Now,
                RuleName = rule.Name,
                Success = success,
                Message = message,
            };

            using (data)
            {
                var actionEntries = data.GetCollection<LogEntry>("activity");
                var entryCount = actionEntries.Count();
                actionEntries.Insert(newEntry);

                if (actionEntries.Count() > 100)
                {
                    try
                    {
                        actionEntries.Delete(0);
                    }
                    catch { }
                }
            }
        }

        public static void LogError(Exception exception = null, string ruleName = "")
        {
            try
            {
                var entry = new LogEntry
                {
                    TimeStamp = DateTime.Now,
                    RuleName = ruleName,
                    Success = false,
                    Message = exception.Message ?? ""
                };

                var data = GetDatabase();
                using (data)
                {
                    var errorEntries = data.GetCollection<LogEntry>("errors");
                    errorEntries.Insert(entry);

                    if (errorEntries.Count() == 101) errorEntries.Delete(100);
                }
            }
            catch { }
        }

        public static void LogActivity(string message = "", bool success = true, string ruleName = "")
        {
            try
            {
                var entry = new LogEntry
                {
                    TimeStamp = DateTime.Now,
                    RuleName = ruleName,
                    Success = success,
                    Message = message
                };

                var data = GetDatabase();
                using (data)
                {
                    var activityEntries = data.GetCollection<LogEntry>("activity");
                    activityEntries.Insert(entry);

                    if (activityEntries.Count() >= 101) activityEntries.Delete(100);
                }
            }
            catch { }
        }
    }
}

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

        public static async Task LogAction(Rule rule, bool success = true, string message = "")
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
                var actionEntries = data.GetCollection<LogEntry>("actionlogs");
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

        public static void LogException(Exception ex)
        {
            try
            {
                using (var data = GetDatabase())
                {
                    var exceptions = data.GetCollection<Exception>("exceptions");
                    exceptions.Insert(ex);

                    if (exceptions.Count() > 100)
                    {
                        try
                        {
                            //exceptions.Delete(ex);
                        }
                        catch { }
                    }
                }
            }
            catch
            {

            }
        }
    }
}

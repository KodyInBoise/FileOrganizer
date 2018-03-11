using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileOrganizer.Utilities;

namespace FileOrganizer
{
    public class LogHelper
    {
        public class LogEntry
        {
            public DateTime TimeStamp { get; set; }
            public int RuleID { get; set; }
            public string Status { get; set; }
        }

        public static void SaveAction(string text)
        {
            
        }

        public static async Task LogEntrySuccess(int id)
        {
            try
            {
                var entry = new LogEntry
                {
                    TimeStamp = DateTime.Now,
                    RuleID = id,
                    Status = "Success"
                };
                DataHelper.AddLogEntry(entry);
            }
            catch(Exception ex)
            {

            }
        }
    }
}

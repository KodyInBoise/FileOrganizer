using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LiteDB;

namespace FileOrganizer.Utilities
{
    public class DataHelper
    {
        private LiteDatabase _data { get; set; }
        private List<Rule> _rules { get; set; }

        public static string GetRootPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "File Organizer");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }

        private static string GetDataPath()
        {
            return Path.Combine(GetRootPath(), "data.fo");
        }

        public DataHelper()
        {
            _data = new LiteDatabase(GetDataPath());
        }

        public async Task<List<Rule>> GetAllRules()
        {
            using (_data)
            {
                return _data.GetCollection<Rule>("rules").FindAll().ToList();
            }
        }

        public void CreateRule(Rule rule)
        {
            try
            {
                rule.ModifiedTimestamp = DateTime.Now;
                _data = new LiteDatabase(GetDataPath());

                using (_data)
                {
                    var rules = _data.GetCollection<Rule>("rules");
                    rules.Insert(rule);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRule(Rule rule)
        {
            try
            {
                _data = new LiteDatabase(GetDataPath());
                using (_data)
                {
                    var rules = _data.GetCollection<Rule>("rules");
                    rules.Update(rule);
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }

        }

        public void DeleteRule(Rule rule)
        {
            try
            {
                _data = new LiteDatabase(GetDataPath());
                using (_data)
                {
                    var rules = _data.GetCollection<Rule>("rules");
                    rules.Delete(rule.ID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateAllRules(List<Rule> ruleList)
        {
            _data = new LiteDatabase(GetDataPath());
            using (_data)
            {
                var rules = _data.GetCollection<Rule>("rules");
                foreach (var rule in ruleList)
                {
                    rules.Update(rule);
                }
            }
        }

        public static void AddLogEntry(LogHelper.LogEntry entry)
        {
            var data = new LiteDatabase(GetDataPath());
            using (data)
            {
                var logEntries = data.GetCollection<LogHelper.LogEntry>("activitylog");
                logEntries.Insert(entry);
            }
        }

        public static List<LogHelper.LogEntry> GetActivityEntries()
        {
            var data = new LiteDatabase(GetDataPath());
            using (data)
            {
                return data.GetCollection<LogHelper.LogEntry>("activity").FindAll().ToList();
            }
        }

        public static List<LogHelper.LogEntry> GetErrorEntries()
        {
            var data = new LiteDatabase(GetDataPath());
            using (data)
            {
                return data.GetCollection<LogHelper.LogEntry>("errors").FindAll().ToList();
            }
        }

    }
}

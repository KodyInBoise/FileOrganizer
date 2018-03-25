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

        private static string GetDataPath()
        {
            var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "File Organizer");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            return Path.Combine(dataDir, "data.fo");
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
    }
}

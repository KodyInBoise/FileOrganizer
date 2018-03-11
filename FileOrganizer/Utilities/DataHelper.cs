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
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "File Organizer\\data.fo");
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
    }
}

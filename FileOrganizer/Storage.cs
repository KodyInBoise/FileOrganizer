using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace FileOrganizer
{
    class Storage
    {

        public static string GetDataLocation()
        {
            var path = $"{Environment.GetEnvironmentVariable("LocalAppData")}\\File Organizer\\";

            return path;
        }

        public static void Initialize()
        {
            var path = GetDataLocation();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + "data.sqlite"))
            {
                SQLiteConnection.CreateFile(path + "data.sqlite");
                CreateTables();
            }
        }

        private static void CreateTables()
        {
            string sql = "CREATE TABLE rules (modified VARCHAR, sourcedir VARCHAR, destdir VARCHAR, action VARCHAR, keyword VARCHAR, frequency VARCHAR)";
            string sql2 = "CREATE TABLE actionlog (timestamp VARCHAR, action VARCHAR, sourcedir VARCHAR, destdir VARCHAR, status VARCHAR, files VARCHAR, message VARCHAR)";
            Task.Run(() => ExecuteCommand(sql));
            Task.Run(() => ExecuteCommand(sql2));
        }

        public static async void SaveRule(Rule r)
        {
            string sql = $"INSERT INTO rules (modified, sourcedir, destdir, action, keyword, frequency) VALUES (@timestamp, @sourcedir, @destdir, @action, @keyword, @frequency)";
            List<SQLiteParameter> tmpList = new List<SQLiteParameter>();
            SQLiteParameter param1 = new SQLiteParameter("timestamp", r.ModifiedTimestamp); tmpList.Add(param1);
            SQLiteParameter param2 = new SQLiteParameter("sourcedir", r.SourceDir); tmpList.Add(param2);
            SQLiteParameter param3 = new SQLiteParameter("destdir", r.DestDir); tmpList.Add(param3);
            SQLiteParameter param4 = new SQLiteParameter("action", r.Action); tmpList.Add(param4);
            SQLiteParameter param5 = new SQLiteParameter("keyword", r.Keyword); tmpList.Add(param5);
            SQLiteParameter param6 = new SQLiteParameter("frequency", r.Frequency); tmpList.Add(param6);

            await Task.Run(() => ExecuteCommand(sql, tmpList));
        }

        public static async void UpdateRule(Rule r)
        {
            string sql = $"UPDATE rules SET modified=@timestamp, sourcedir=@sourcedir, destdir=@destdir, action=@action, keyword=@keyword, frequency=@frequency WHERE modified='{r.ModifiedTimestamp}'";
            List<SQLiteParameter> tmpList = new List<SQLiteParameter>();
            SQLiteParameter param1 = new SQLiteParameter("timestamp", DateTime.Now.ToString()); tmpList.Add(param1);
            SQLiteParameter param2 = new SQLiteParameter("sourcedir", r.SourceDir); tmpList.Add(param2);
            SQLiteParameter param3 = new SQLiteParameter("destdir", r.DestDir); tmpList.Add(param3);
            SQLiteParameter param4 = new SQLiteParameter("action", r.Action); tmpList.Add(param4);
            SQLiteParameter param5 = new SQLiteParameter("keyword", r.Keyword); tmpList.Add(param5);
            SQLiteParameter param6 = new SQLiteParameter("frequency", r.Frequency); tmpList.Add(param6);

            await Task.Run(() => ExecuteCommand(sql, tmpList));
        }

        public static async void DeleteRule(Rule r)
        {
            string sql = $"DELETE FROM rules WHERE modified=@timestamp";
            List<SQLiteParameter> tmpList = new List<SQLiteParameter>();
            SQLiteParameter param1 = new SQLiteParameter("timestamp", r.ModifiedTimestamp); tmpList.Add(param1);

            await Task.Run(() => ExecuteCommand(sql, tmpList));
        }

        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={GetDataLocation()}data.sqlite;Version=3;");
        }

        private static async Task ExecuteCommand(string sql, List<SQLiteParameter> paramList = null)
        {
            var con = GetConnection();
            SQLiteCommand comm = new SQLiteCommand(sql, con);
            if (paramList != null)
            {
                foreach (SQLiteParameter p in paramList)
                {
                    comm.Parameters.Add(p);
                }
            }

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            con.Close();
        }

        public static List<Rule> LoadRuleList()
        {
            List<Rule> tmpList = new List<Rule>();
            var con = GetConnection();
            string sql = "SELECT * FROM rules";
            SQLiteCommand comm = new SQLiteCommand(sql, con);
            using (con)
            {
                con.Open();
                using (SQLiteDataReader rdr = comm.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Rule tmpRule = new Rule
                        {
                            ModifiedTimestamp = rdr[0].ToString(),
                            SourceDir = rdr[1].ToString(),
                            DestDir = rdr[2].ToString(),
                            Action = rdr[3].ToString(),
                            Keyword = rdr[4].ToString(),
                            Frequency = rdr[5].ToString()
                        };
                        tmpList.Add(tmpRule);
                    }
                }
                con.Close();
            }

            return tmpList;
        }

        public static void LogAction(Rule r, Exception ex = null)
        {
            string sql = $"INSERT INTO actionlog (timestamp, action, sourcedir, destdir, status, files) VALUES (@timestamp, @action, @sourcedir, @destdir, @status, @files)";
            List<SQLiteParameter> tmpList = new List<SQLiteParameter>();
            SQLiteParameter param1 = new SQLiteParameter("timestamp", DateTime.Now.ToString()); tmpList.Add(param1);
            SQLiteParameter param2 = new SQLiteParameter("action", r.Action); tmpList.Add(param2);
            SQLiteParameter param3 = new SQLiteParameter("sourcedir", r.SourceDir); tmpList.Add(param3);
            SQLiteParameter param4 = new SQLiteParameter("destdir", r.DestDir); tmpList.Add(param4);
            string message = "";
            bool successful = true;
            if (ex != null)
            {
                successful = false;
                message = ex.Message;
            }
            SQLiteParameter param5 = new SQLiteParameter("status", successful); tmpList.Add(param5);
            string filesString = "";
            if (r.FileList != null)
            {
                foreach (FileInfo f in r.FileList)
                {
                    filesString += f.FullName + " | ";
                }
            }
            SQLiteParameter param6 = new SQLiteParameter("files", filesString); tmpList.Add(param6);

            Task.Run(() => ExecuteCommand(sql, tmpList));
        }
    }
}

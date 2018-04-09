using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileOrganizer.Utilities
{
    public class SettingsHelper
    {
        public class FileOrganizerSettings
        {
            public double MainWindowLeft { get; set; }
            public double MainWindowTop { get; set; }

            public double PromptWindowLeft { get; set; }
            public double PromptWindowTop { get; set; }
        }

        public static void SavePromptWindowLocation(double left, double top)
        {
            var settings = LoadSettings();
            settings.PromptWindowLeft = left;
            settings.PromptWindowTop = top;

            SaveSettings(settings);
        }

        public static void SaveMainWindowLocation(double left, double top)
        {
            var settings = LoadSettings();
            settings.MainWindowLeft = left;
            settings.MainWindowTop = top;

            SaveSettings(settings);
        }

        public static void SaveSettings(FileOrganizerSettings settings)
        {
            try
            {
                File.WriteAllText(SettingsPath(), JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex) { LogHelper.LogError(ex); }
        }

        public static FileOrganizerSettings LoadSettings()
        {
            if (File.Exists(SettingsPath()))
            {
                return JsonConvert.DeserializeObject<FileOrganizerSettings>(File.ReadAllText(SettingsPath()));
            }
            else return new FileOrganizerSettings();
        }

        private static string SettingsPath()
        {
            return Path.Combine(DataHelper.GetRootPath(), "settings.json");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileOrganizer
{
    class LogHelper
    {
        public static void SaveAction(string text)
        {
            var path = Storage.GetDataLocation() + "\\actionlog";
            File.AppendAllText(path, text);
        }
    }
}

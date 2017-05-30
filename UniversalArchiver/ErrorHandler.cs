using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalArchiver
{
    class ErrorHandler
    {
        /// <summary>
        /// Creates a log file from the console
        /// </summary>
        /// <returns>
        /// A value that represents the success of the log file.
        /// <para>True if log was succesfully created, otherwise false.</para>
        /// </returns>
        public static bool WriteLogFile()
        {
            if (Globals.DebugConsole == null)
            {
                return false;
            }

            try
            {
                File.WriteAllText(Path.Combine(Application.StartupPath, $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log"), Globals.DebugConsole.ConsoleText);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}

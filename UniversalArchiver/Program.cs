using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversalArchiver.DEBUG;

namespace UniversalArchiver
{
    static class Program
    {
        public static bool ConsoleCreated;
        public static readonly string TempPath = Path.Combine(Application.StartupPath, "TEMP");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                if (Directory.Exists(Path.Combine(Application.StartupPath, "TEMP")))
                {
                    Directory.Delete(Path.Combine(Application.StartupPath, "TEMP"), true);
                }
            };

            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "TEMP"));

#if DEBUG
            DebugConsole console = new DebugConsole();

            Thread consoleThread = new Thread(() =>
            {
                Application.Run(console);

                Environment.Exit(0);
            });
            consoleThread.IsBackground = true;
            consoleThread.Name = "Console Thread";
            consoleThread.Start();

            while (!ConsoleCreated)
            {
            }

            Application.Run(new RarArchiveView(Path.Combine(Application.StartupPath, "test.rar")));
#endif

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                Console.Out.WriteLine(arg);
            }

            Console.Out.WriteLine(Environment.GetCommandLineArgs().Length);

            // If ran directly on the executable, assume install/uninstall
            if (Environment.GetCommandLineArgs().Length == 1)
            {
                Application.Run(new Installer());
            }

            // Open file for viewing
            else if (File.Exists(Environment.GetCommandLineArgs()[1]) && Environment.GetCommandLineArgs().Length == 2)
            {
                Application.Run(new RarArchiveView(Environment.GetCommandLineArgs()[1]));
            }

            // Open file for either viewing or extracting (Based on 2nd command-line arg)
            else if (File.Exists(Environment.GetCommandLineArgs()[1]) && Environment.GetCommandLineArgs().Length == 3)
            {
                switch (Environment.GetCommandLineArgs()[2])
                {
                    case "Open":
                    {
                        
                    }

                        break;
                    case "Extract":
                    {
                        
                    }

                        break;
                }
            }
        }
    }
}

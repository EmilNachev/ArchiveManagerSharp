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
        private static bool shouldDoPreprocess = true;
        private static int OpenTempNum = 0;

        public static bool ConsoleCreated;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                if (shouldDoPreprocess)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                    {
                        CleanupTempFolders();
                    };

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

                    Globals.DebugConsole = console;

                    shouldDoPreprocess = false;
                }

                Console.Out.WriteLine("Command-Line Args:");

                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    Console.Out.WriteLine(arg);
                }

                Console.Out.Write("Command-Line Args Length: ");

                Console.Out.WriteLine(Environment.GetCommandLineArgs().Length);

                if (args.Length > 0 && args[0].StartsWith("resume"))
                {
                    SelectArchiveVeiwer(args[0].Split(':')[1]);
                }

                // If ran directly on the executable, assume install/uninstall
                if (Environment.GetCommandLineArgs().Length == 1)
                {
                    Application.Run(new ArchiveSelector());
                }

                // Open file for viewing
                else if (File.Exists(Environment.GetCommandLineArgs()[1]) && Environment.GetCommandLineArgs().Length == 2)
                {
                    Application.Run(new ArchiveView(Environment.GetCommandLineArgs()[1]));
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (!ErrorHandler.WriteLogFile())
                {
                    MessageBox.Show("Failed to create log file!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Generic error caught! Please create an issue on the GitHub repo with the log file linked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (MessageBox.Show("Would you like to restart the application?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    if (Globals.LastFile != string.Empty)
                    {
                        Main(new string[] { $"resume:{Globals.LastFile}" });
                    }
                    else
                    {
                        Application.Restart();
                    }
                }
                else
                {
                    Environment.Exit(666);
                }
            }
        }

        private static void SelectArchiveVeiwer(string file)
        {
            switch (Path.GetExtension(file))
            {
                case ".rar":
                {
                    Application.Run(new ArchiveView(file));
                }
                    break;
                default:
                {
                    
                }
                    break;
            }
        }

        public static void CleanupTempFolders(int tempNum = -1)
        {
            if (tempNum != -1)
            {

                return;
            }

            foreach (string directory in Directory.GetDirectories(Application.StartupPath))
            {
                if (directory.Contains("TEMP"))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        public static string TempPath(int requestedTempNum)
        {
            if (!Directory.Exists(Path.Combine(Application.StartupPath, $"TEMP{requestedTempNum}")))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, $"TEMP{requestedTempNum}"));
            }

            return Path.Combine(Application.StartupPath, $"TEMP{requestedTempNum}");
        }

        public static int RequestTempNum()
        {
            return OpenTempNum++;
        }
    }
}

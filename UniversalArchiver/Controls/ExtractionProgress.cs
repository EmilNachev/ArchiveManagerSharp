using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace UniversalArchiver.Controls
{
    public partial class ExtractionProgress : Form
    {
        private FileSystemInfo currentFile;
        private bool hasIntializedExtraction;
        private int fileNum;
        private int fileCount;
        private int fileProgress;
        private long expectedFileSize;
        private string extractionLocation;
        private string lastFilePath;
        private string currentProcess;
        private Thread fileSizeThread;
        private Timer updatePbTimer;

        public FileSystemInfo CurrentFile
        {
            get { return this.currentFile; }
            set { this.currentFile = value; }
        }

        public long ExpectedFileSize
        {
            get { return this.expectedFileSize; }
            set { this.expectedFileSize = value; }
        }

        public int FileCount
        {
            get { return this.fileCount; }
            set { this.fileCount = value; }
        }

        public string CurrentProcess
        {
            get { return this.currentProcess; }
            set { this.currentProcess = value; }
        }

        public string ExtractionLocation
        {
            get { return this.extractionLocation; }
            set { this.extractionLocation = value; }
        }

        public ExtractionProgress()
        {
            this.InitializeComponent();

            this.updatePbTimer = new Timer();
            this.updatePbTimer.Tick += this.Timer_Tick;
            this.updatePbTimer.Start();

            this.fileSizeThread = new Thread(() =>
            {
                while (this.currentFile == null || this.expectedFileSize == 0 || !this.currentFile.Exists)
                {
                }

                while (true)
                {
                    this.fileProgress = new FileInfo(this.currentFile.FullName).Length / this.expectedFileSize * 100 > 100 ? 100 : Convert.ToInt32((double)new FileInfo(this.currentFile.FullName).Length / this.expectedFileSize * 100);
                }
            });

            this.fileSizeThread.Start();
        }

        public new void Hide()
        {
            if (this.fileSizeThread != null && this.fileSizeThread.IsAlive)
            {
                this.fileSizeThread.Abort();
            }

            base.Hide();
        }

        private void BeginExtraction()
        {

            int initialProgress = 1 / this.fileCount * 100;

            this.pbCurrentExtraction.Value = initialProgress;

            this.lblCurrentFile.Text = string.Empty;
            this.lblExtractionProgress.Text = $"Processing file 1/{fileCount} {initialProgress}%";
            this.Show();
        }

        private void UpdateProcessText(string process)
        {
            this.lblCurrentProcess.Text = process;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.currentFile == null)
            {
                return;
            }

            if (!this.hasIntializedExtraction)
            {
                this.BeginExtraction();
                this.hasIntializedExtraction = true;
            }

            this.lblCurrentFile.Text = $"Processing {this.currentFile.Name} {this.fileProgress}%";

            this.pbCurrentFile.Value = this.fileProgress;

            this.UpdateProcessText(this.currentProcess);

            if (this.currentFile.FullName != this.lastFilePath)
            {
                this.fileNum++;

                this.pbCurrentExtraction.Value = (int)((double)this.fileNum / this.fileCount * 100);

                this.lblExtractionProgress.Text = $"Processing file {this.fileNum}/{this.fileCount} {this.pbCurrentExtraction.Value}%";

                this.lastFilePath = this.currentFile.FullName;
            }
        }
    }
}

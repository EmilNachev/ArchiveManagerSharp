namespace UniversalArchiver.Controls
{
    partial class ExtractionProgress
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbCurrentFile = new System.Windows.Forms.ProgressBar();
            this.pbCurrentExtraction = new System.Windows.Forms.ProgressBar();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblCurrentProcess = new System.Windows.Forms.Label();
            this.lblExtractionProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbCurrentFile
            // 
            this.pbCurrentFile.Location = new System.Drawing.Point(30, 60);
            this.pbCurrentFile.Name = "pbCurrentFile";
            this.pbCurrentFile.Size = new System.Drawing.Size(271, 23);
            this.pbCurrentFile.TabIndex = 0;
            // 
            // pbCurrentExtraction
            // 
            this.pbCurrentExtraction.Location = new System.Drawing.Point(30, 123);
            this.pbCurrentExtraction.Name = "pbCurrentExtraction";
            this.pbCurrentExtraction.Size = new System.Drawing.Size(271, 23);
            this.pbCurrentExtraction.TabIndex = 0;
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.Location = new System.Drawing.Point(27, 86);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(274, 23);
            this.lblCurrentFile.TabIndex = 1;
            this.lblCurrentFile.Text = "Processing file.ext 0%";
            this.lblCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentProcess
            // 
            this.lblCurrentProcess.Location = new System.Drawing.Point(27, 187);
            this.lblCurrentProcess.Name = "lblCurrentProcess";
            this.lblCurrentProcess.Size = new System.Drawing.Size(274, 74);
            this.lblCurrentProcess.TabIndex = 1;
            this.lblCurrentProcess.Text = "Initiating extraction...";
            // 
            // lblExtractionProgress
            // 
            this.lblExtractionProgress.Location = new System.Drawing.Point(27, 149);
            this.lblExtractionProgress.Name = "lblExtractionProgress";
            this.lblExtractionProgress.Size = new System.Drawing.Size(274, 23);
            this.lblExtractionProgress.TabIndex = 1;
            this.lblExtractionProgress.Text = "Processing file 0/999 0%";
            this.lblExtractionProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ExtractionProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 270);
            this.Controls.Add(this.lblCurrentProcess);
            this.Controls.Add(this.lblExtractionProgress);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.pbCurrentExtraction);
            this.Controls.Add(this.pbCurrentFile);
            this.Name = "ExtractionProgress";
            this.ShowIcon = false;
            this.Text = "Extracting...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbCurrentFile;
        private System.Windows.Forms.ProgressBar pbCurrentExtraction;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Label lblCurrentProcess;
        private System.Windows.Forms.Label lblExtractionProgress;
    }
}

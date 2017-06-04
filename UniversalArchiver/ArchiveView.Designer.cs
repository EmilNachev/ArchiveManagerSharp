namespace UniversalArchiver
{
    partial class ArchiveView
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvFileList = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmPacked = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbArchivePath = new System.Windows.Forms.TextBox();
            this.pbUpALevel = new System.Windows.Forms.PictureBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.tipExtract = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbUpALevel)).BeginInit();
            this.SuspendLayout();
            // 
            // lvFileList
            // 
            this.lvFileList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmSize,
            this.clmPacked,
            this.clmType,
            this.clmModified});
            this.lvFileList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvFileList.Location = new System.Drawing.Point(0, 120);
            this.lvFileList.Name = "lvFileList";
            this.lvFileList.Size = new System.Drawing.Size(971, 448);
            this.lvFileList.TabIndex = 0;
            this.lvFileList.UseCompatibleStateImageBehavior = false;
            this.lvFileList.View = System.Windows.Forms.View.Details;
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            this.clmName.Width = 244;
            // 
            // clmSize
            // 
            this.clmSize.Text = "Size";
            this.clmSize.Width = 90;
            // 
            // clmPacked
            // 
            this.clmPacked.Text = "Packed";
            this.clmPacked.Width = 87;
            // 
            // clmType
            // 
            this.clmType.Text = "MIME Type";
            this.clmType.Width = 177;
            // 
            // clmModified
            // 
            this.clmModified.Text = "Last Modified";
            this.clmModified.Width = 371;
            // 
            // tbArchivePath
            // 
            this.tbArchivePath.Location = new System.Drawing.Point(19, 101);
            this.tbArchivePath.Name = "tbArchivePath";
            this.tbArchivePath.Size = new System.Drawing.Size(952, 20);
            this.tbArchivePath.TabIndex = 2;
            // 
            // pbUpALevel
            // 
            this.pbUpALevel.BackColor = System.Drawing.Color.Transparent;
            this.pbUpALevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbUpALevel.Image = global::UniversalArchiver.Properties.Resources.upalevel;
            this.pbUpALevel.Location = new System.Drawing.Point(0, 101);
            this.pbUpALevel.Name = "pbUpALevel";
            this.pbUpALevel.Size = new System.Drawing.Size(20, 20);
            this.pbUpALevel.TabIndex = 3;
            this.pbUpALevel.TabStop = false;
            this.pbUpALevel.Click += new System.EventHandler(this.PbUpALevel_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnExtract.FlatAppearance.BorderSize = 0;
            this.btnExtract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtract.Image = global::UniversalArchiver.Properties.Resources.ExtractIcon;
            this.btnExtract.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExtract.Location = new System.Drawing.Point(12, 12);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 75);
            this.btnExtract.TabIndex = 1;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.BtnExtract_Click);
            // 
            // RarArchiveView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 568);
            this.Controls.Add(this.pbUpALevel);
            this.Controls.Add(this.tbArchivePath);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.lvFileList);
            this.MinimumSize = new System.Drawing.Size(16, 607);
            this.Name = "RarArchiveView";
            this.Text = "ArchiveView";
            ((System.ComponentModel.ISupportInitialize)(this.pbUpALevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFileList;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmSize;
        private System.Windows.Forms.ColumnHeader clmPacked;
        private System.Windows.Forms.ColumnHeader clmType;
        private System.Windows.Forms.ColumnHeader clmModified;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.TextBox tbArchivePath;
        private System.Windows.Forms.PictureBox pbUpALevel;
        private System.Windows.Forms.ToolTip tipExtract;
    }
}
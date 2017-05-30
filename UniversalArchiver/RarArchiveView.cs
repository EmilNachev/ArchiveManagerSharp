using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MimeTypes;
using NUnrar.Archive;
using NUnrar.Common;

namespace UniversalArchiver
{
    public partial class RarArchiveView : Form
    {
        private ImageList iconList;
        private string currentArchive;
        private string currentFolder = "\0";
        private int lastSortedColumn = -1;
        private int folderLevel;

        // DragDrop variables
        private Point startDragDropPoint;

        public RarArchiveView(string file)
        {
            this.InitializeComponent();

            this.iconList = new ImageList();
            this.lvFileList.SmallImageList = this.iconList;
            this.lvFileList.View = View.Details;
            this.lvFileList.Sorting = SortOrder.Ascending;
            this.lvFileList.AllowColumnReorder = false;
            this.lvFileList.FullRowSelect = true;

            this.lvFileList.ColumnClick += this.LvFileList_ColumnClick;
            this.lvFileList.DoubleClick += this.LvFileList_DoubleClick;
            this.lvFileList.MouseDown += this.LvFileList_MouseDown;
            this.lvFileList.MouseMove += this.LvFileList_MouseMove;

            this.tbArchivePath.Text = file + "\\";
            this.tbArchivePath.ReadOnly = true;

            ToolTip folderLevelTip = new ToolTip();
            folderLevelTip.SetToolTip(this.pbUpALevel, "Enter parent folder");

            this.currentArchive = file;

            this.Resize += this.RarArchiveView_Resize;
            this.Load += this.OnLoad;

            this.Closed += this.RarArchiveView_Closed;
        }

        private void LvFileList_MouseMove(object sender, MouseEventArgs e)
        {
            Point distanceVector = new Point(this.startDragDropPoint.X - e.X, this.startDragDropPoint.Y - e.Y);

            if (e.Button == MouseButtons.Left &&
                Math.Abs(distanceVector.X) > 10 &&
                Math.Abs(distanceVector.Y) > 10)
            {
                if (this.lvFileList.SelectedItems.Count <= 0)
                {
                    return;
                }

                List<string> filesList = new List<string>();

                foreach (ListViewItem selectedItem in this.lvFileList.SelectedItems)
                {

                    if ((selectedItem.Tag as RarArchiveEntry).IsDirectory)
                    {
                        (selectedItem.Tag as RarArchiveEntry).WriteToDirectory(Path.Combine(Program.TempPath, (selectedItem.Tag as RarArchiveEntry).FilePath.Replace(this.currentFolder, string.Empty)));

                        filesList.Add(Path.Combine(Program.TempPath, (selectedItem.Tag as RarArchiveEntry).FilePath.Replace(this.currentFolder, string.Empty)));
                    }
                    else
                    {
                        (selectedItem.Tag as RarArchiveEntry).WriteToFile(Path.Combine(Program.TempPath, (selectedItem.Tag as RarArchiveEntry).FilePath.Replace(this.currentFolder, string.Empty)));

                        filesList.Add(Path.Combine(Program.TempPath, (selectedItem.Tag as RarArchiveEntry).FilePath.Replace(this.currentFolder, string.Empty)));
                    }
                }

                string dataFormat = DataFormats.FileDrop;
                DataObject dataObject = new DataObject(dataFormat, filesList.ToArray());
                this.DoDragDrop(dataObject, DragDropEffects.Copy);
            }
        }

        private void LvFileList_MouseDown(object sender, MouseEventArgs e)
        {
            this.startDragDropPoint = e.Location;
        }

        private void RarArchiveView_Closed(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.Combine(Application.StartupPath, "TEMP")))
            {
                Directory.Delete(Path.Combine(Application.StartupPath, "TEMP"), true);
            }
        }

        private void RarArchiveView_Resize(object sender, EventArgs e)
        {
            this.lvFileList.Size = new Size(this.lvFileList.Width, 448 + (this.Height - 607));

            this.tbArchivePath.Width = this.lvFileList.Width - 20;
        }

        private void LvFileList_DoubleClick(object sender, EventArgs e)
        {
            if (this.lvFileList.SelectedItems.Count > 0 && this.lvFileList.SelectedItems.Count < 2)
            {
                if ((this.lvFileList.SelectedItems[0].Tag as RarEntry).IsDirectory)
                {
                    this.EnterFolder(this.lvFileList.SelectedItems[0].Tag as RarEntry);
                }
                else
                {
                    (this.lvFileList.SelectedItems[0].Tag as RarArchiveEntry).WriteToFile(Path.Combine(Program.TempPath, Path.GetFileName((this.lvFileList.SelectedItems[0].Tag as RarArchiveEntry).FilePath)));
                    Process.Start(Path.Combine(Program.TempPath, Path.GetFileName((this.lvFileList.SelectedItems[0].Tag as RarArchiveEntry).FilePath)));
                }
            }
        }

        private void LvFileList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.lastSortedColumn == e.Column && this.lvFileList.Sorting == SortOrder.Descending)
            {
                this.lvFileList.Sorting = SortOrder.Ascending;
            }
            else if (this.lastSortedColumn == e.Column && this.lvFileList.Sorting == SortOrder.Ascending)
            {
                this.lvFileList.Sorting = SortOrder.Descending;
            }

            this.lvFileList.ListViewItemSorter = new FileFolderComparer(e.Column, this.lvFileList.Sorting);
            this.lvFileList.Sort();

            this.lastSortedColumn = e.Column;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.lvFileList.BeginUpdate();
            this.lvFileList.Items.Clear();
            this.lvFileList.EndUpdate();

            switch (Path.GetExtension(this.currentArchive))
            {
                case ".rar":
                    {
                        RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), RarOptions.GiveDirectoryEntries);
                        foreach (RarArchiveEntry entry in rar.Entries)
                        {
                            if (entry.IsDirectory && !entry.FilePath.Contains("\\"))
                            {
                                this.AddRarFolder(entry);
                                Console.Out.WriteLine(entry.FilePath);
                            }

                            else if (!entry.FilePath.Contains("\\"))
                            {
                                this.AddRarFile(entry);
                            }
                        }
                    }
                    break;
                default:
                    {
                        // TODO Add link to page to request file-type
                        MessageBox.Show("Unknown file type!");

                        Environment.Exit(0);
                    }
                    break;
            }

        }

        private void EnterFolder(RarEntry folder)
        {
            this.folderLevel++;

            this.lvFileList.BeginUpdate();
            this.lvFileList.Items.Clear();
            this.lvFileList.EndUpdate();

            int slashes = folder.FilePath.ToCharArray().ToList().FindAll(e => e == '\\').Count + 1;
            this.currentFolder = folder.FilePath + "\\";
            Console.Out.WriteLine(this.currentFolder.Split('\\')[this.currentFolder.Split('\\').Length - 2]);

            RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), RarOptions.GiveDirectoryEntries);
            foreach (RarArchiveEntry entry in rar.Entries)
            {
                if (entry.IsDirectory && entry.FilePath.Contains($"{folder.FilePath}\\"))
                {
                    if (entry.FilePath.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                    {
                        continue;
                    }

                    this.AddRarFolder(entry);
                    Console.Out.WriteLine(entry.FilePath);
                }
                else if (entry.FilePath.Contains($"{folder.FilePath}\\"))
                {
                    if (entry.FilePath.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                    {
                        continue;
                    }

                    this.AddRarFile(entry);
                    Console.Out.WriteLine(entry.FilePath);
                }
            }

            this.tbArchivePath.Text = this.currentArchive + "\\" + this.currentFolder;
        }

        private void ParentFolder()
        {
            this.lvFileList.BeginUpdate();
            this.lvFileList.Items.Clear();
            this.lvFileList.EndUpdate();

            this.currentFolder = this.currentFolder.Replace(this.currentFolder.Split('\\')[this.currentFolder.Split('\\').Length - 2] + "\\", string.Empty);
            Console.Out.WriteLine($"Parsed Parent folder: {this.currentFolder}");
            int slashes = this.currentFolder.ToCharArray().ToList().FindAll(e => e == '\\').Count;

            if (this.currentFolder == string.Empty)
            {
                this.currentFolder = "\0";

                this.OnLoad(this, EventArgs.Empty);

                this.tbArchivePath.Text = this.currentArchive + "\\" + this.currentFolder;

                return;
            }

            RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), RarOptions.GiveDirectoryEntries);
            foreach (RarArchiveEntry entry in rar.Entries)
            {

                if (entry.IsDirectory && entry.FilePath.Contains($"{this.currentFolder}"))
                {
                    if (entry.FilePath.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                    {
                        continue;
                    }

                    this.AddRarFolder(entry);
                    Console.Out.WriteLine(entry.FilePath);
                }
                else if (entry.FilePath.Contains($"{this.currentFolder}"))
                {
                    if (entry.FilePath.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                    {
                        continue;
                    }

                    this.AddRarFile(entry);
                    Console.Out.WriteLine(entry.FilePath);
                }
            }

            this.tbArchivePath.Text = this.currentArchive + "\\" + this.currentFolder;
        }

        private void AddRarFolder(RarEntry folderEntry)
        {
            this.lvFileList.BeginUpdate();

            ListViewItem item = new ListViewItem(folderEntry.FilePath.Replace(this.currentFolder, string.Empty), 1);
            item.Tag = folderEntry;
            item.SubItems.Add(string.Empty);
            item.SubItems.Add(string.Empty);
            item.SubItems.Add("folder");

            if (folderEntry.LastModifiedTime.HasValue)
            {
                item.SubItems.Add($"{folderEntry.LastModifiedTime.Value.ToLocalTime().ToLongDateString()} {folderEntry.LastModifiedTime.Value.ToLocalTime().ToLongTimeString()}");
            }

            if (!this.iconList.Images.ContainsKey("folder"))
            {
                this.iconList.Images.Add("folder", IconHelper.GetFolderIcon(IconHelper.IconSize.Small, IconHelper.FolderType.Closed));
            }

            item.ImageKey = "folder";
            this.lvFileList.Items.Add(item);

            this.lvFileList.EndUpdate();
        }

        private void AddRarFile(RarEntry fileEntry)
        {
            this.lvFileList.BeginUpdate();

            ListViewItem item = new ListViewItem(fileEntry.FilePath.Replace(this.currentFolder, string.Empty), 1);
            item.Tag = fileEntry;
            item.SubItems.Add(fileEntry.Size.ToString());
            item.SubItems.Add(fileEntry.CompressedSize.ToString());
            item.SubItems.Add(MimeTypeMap.GetMimeType(Path.GetExtension(fileEntry.FilePath)));

            if (fileEntry.LastModifiedTime.HasValue)
            {
                item.SubItems.Add($"{fileEntry.LastModifiedTime.Value.ToLocalTime().ToLongDateString()} {fileEntry.LastModifiedTime.Value.ToLocalTime().ToLongTimeString()}");
            }


            if (!this.iconList.Images.ContainsKey(Path.GetExtension(fileEntry.FilePath)))
            {
                this.iconList.Images.Add(Path.GetExtension(fileEntry.FilePath), IconHelper.GetFileIcon(Path.GetExtension(fileEntry.FilePath), IconHelper.IconSize.Small, false));
            }

            item.ImageKey = Path.GetExtension(fileEntry.FilePath);
            this.lvFileList.Items.Add(item);

            this.lvFileList.EndUpdate();
        }

        private void PbUpALevel_Click(object sender, EventArgs e)
        {
            if (this.folderLevel > 0)
            {
                this.ParentFolder();
                this.folderLevel--;
            }
        }

        private class FileFolderComparer : IComparer
        {
            private int col;
            private SortOrder order;

            public FileFolderComparer(int column, SortOrder order)
            {
                this.col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal = -1;

                if (this.col == 4)
                {
                    returnVal = DateTime.Compare(Convert.ToDateTime(((ListViewItem)x).SubItems[this.col].Text), Convert.ToDateTime(((ListViewItem)y).SubItems[this.col].Text));

                    if (this.order == SortOrder.Descending)
                    {
                        // Invert the value returned by String.Compare.
                        returnVal *= -1;
                    }

                    return returnVal;
                }

                if (this.col != 0)
                {
                    returnVal = string.CompareOrdinal(((ListViewItem)x).SubItems[this.col].Text, ((ListViewItem)y).SubItems[this.col].Text);

                    // Determine whether the sort order is descending.
                    if (this.order == SortOrder.Descending)
                    {
                        // Invert the value returned by String.Compare.
                        returnVal *= -1;
                    }

                    return returnVal;
                }

                if (((RarEntry)((ListViewItem)x).Tag).IsDirectory == ((RarEntry)((ListViewItem)y).Tag).IsDirectory)
                {
                    returnVal = string.CompareOrdinal(((ListViewItem)x).SubItems[this.col].Text, ((ListViewItem)y).SubItems[this.col].Text);

                    // Determine whether the sort order is descending.
                    if (this.order == SortOrder.Descending)
                    {
                        // Invert the value returned by String.Compare.
                        returnVal *= -1;
                    }

                    return returnVal;
                }

                if (((RarEntry)((ListViewItem)x).Tag).IsDirectory)
                {
                    returnVal = 1;
                }
                else
                {
                    returnVal = -1;
                }

                if (this.order == SortOrder.Descending)
                {
                    returnVal *= -1;
                }

                return returnVal;
            }
        }
    }
}

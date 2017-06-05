using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MimeTypes;
using SharpCompress.Archives;
using UniversalArchiver.Controls;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Readers;

namespace UniversalArchiver
{
    public partial class ArchiveView : Form, IArchiver
    {
        private ImageList iconList;
        private string currentArchive;
        private string currentFolder = "\0";
        private string archivePassword;
        private int lastSortedColumn = -1;
        private int folderLevel;
        private int tempFolderNum;
        private int passwordAttempts;
        private bool hasAttemptedPassword;
        private ExtractionProgress extractionProgress;
        private ArchiveType currentArchiveType;

        // DragDrop variables
        private Point startDragDropPoint;

        private enum ArchiveType
        {
            Rar,
            Zip,
            Tar,
            SevenZip,
            GZip,
            Unknown
        }

        public ArchiveView(string file)
        {
            this.InitializeComponent();

            this.iconList = new ImageList();
            this.lvFileList.SmallImageList = this.iconList;
            this.lvFileList.View = View.Details;
            this.lvFileList.Sorting = SortOrder.Ascending;
            this.lvFileList.AllowColumnReorder = false;
            this.lvFileList.FullRowSelect = true;

            this.tipExtract.SetToolTip(this.btnExtract, "Extract...");

            this.lvFileList.ColumnClick += this.LvFileList_ColumnClick;
            this.lvFileList.DoubleClick += this.LvFileList_DoubleClick;
            this.lvFileList.MouseDown += this.LvFileList_MouseDown;
            this.lvFileList.MouseMove += this.LvFileList_MouseMove;

            this.tbArchivePath.Text = file + "\\";
            this.tbArchivePath.ReadOnly = true;

            ToolTip folderLevelTip = new ToolTip();
            folderLevelTip.SetToolTip(this.pbUpALevel, "Enter parent folder");

            this.currentArchive = file;
            Globals.LastFile = file;

            this.tempFolderNum = Program.RequestTempNum();

            this.Resize += this.RarArchiveView_Resize;
            this.Load += this.OnLoad;

            this.Closed += this.RarArchiveView_Closed;
        }

        private void OnLoad(object sender, EventArgs e) => this.OnLoad(sender, e, "");

        private void OnLoad(object sender, EventArgs e, string password)
        {
            if (this.passwordAttempts == 5)
            {
                MessageBox.Show("You have reached the max amount of password attempts! If there is a non-password related issue, a log file will be created.");

                ErrorHandler.WriteLogFile();

                Environment.Exit(445);
            }

            this.TopMost = true;
            this.TopMost = false;

            this.lvFileList.BeginUpdate();
            this.lvFileList.Items.Clear();
            this.lvFileList.EndUpdate();

            try
            {
                this.archivePassword = password;

                switch (GetArchiveType(Path.GetExtension(this.currentArchive)))
                {
                    case ArchiveType.Rar:
                        {
                            RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = password
                            });
                            foreach (IEntry entry in rar.Entries)
                            {
                                if (entry.IsDirectory && !entry.Key.Contains("\\"))
                                {
                                    this.AddFolder(entry);
                                    Console.Out.WriteLine(entry.Key);
                                }

                                else if (!entry.Key.Contains("\\"))
                                {
                                    this.AddFile(entry);
                                }
                            }

                            this.currentArchiveType = (ArchiveType)Convert.ToInt32(rar.Type);
                        }
                        break;
                    case ArchiveType.Zip:
                        {
                            ZipArchive zipArchive = ZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = password
                            });
                            foreach (IEntry entry in zipArchive.Entries)
                            {
                                if (entry.IsDirectory && !entry.Key.Contains("\\"))
                                {
                                    this.AddFolder(entry);
                                    Console.Out.WriteLine(entry.Key);
                                }

                                else if (!entry.Key.Contains("\\"))
                                {
                                    this.AddFile(entry);
                                }
                            }

                            this.currentArchiveType = (ArchiveType)Convert.ToInt32(zipArchive.Type);
                        }
                        break;
                    case ArchiveType.Tar:
                        break;
                    case ArchiveType.SevenZip:
                        {
                            SevenZipArchive archive = SevenZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = password
                            });
                            foreach (IEntry entry in archive.Entries)
                            {
                                if (entry.IsDirectory && !entry.Key.Contains("\\"))
                                {
                                    this.AddFolder(entry);
                                    Console.Out.WriteLine(entry.Key);
                                }

                                else if (!entry.Key.Contains("\\"))
                                {
                                    this.AddFile(entry);
                                }
                            }

                            this.currentArchiveType = (ArchiveType)Convert.ToInt32(archive.Type);
                        }
                        break;
                    case ArchiveType.GZip:
                        break;
                    default:
                        {
                            MessageBox.Show("Unknown file type!");

                            Environment.Exit(0);
                        }
                        break;
                }
            }
            catch (PasswordProtectedException exception)
            {
                Console.Out.WriteLine(exception);

                this.passwordAttempts++;

                PasswordDialog dialog = new PasswordDialog(this.hasAttemptedPassword);

                // TODO: [Enter password Control]
                if (this.hasAttemptedPassword && dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;
                }
                else if (dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;

                    this.hasAttemptedPassword = true;
                }

                this.OnLoad(this, new EventArgs(), password);
            }
            catch (CryptographicException exception)
            {
                Console.Out.WriteLine(exception);

                this.passwordAttempts++;

                PasswordDialog dialog = new PasswordDialog(this.hasAttemptedPassword);

                // TODO: [Enter password Control]
                if (this.hasAttemptedPassword && dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;
                }
                else if (dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;

                    this.hasAttemptedPassword = true;
                }

                this.OnLoad(this, new EventArgs(), password);
            }
            catch (InvalidFormatException exception)
            {
                Console.Out.WriteLine(exception);

                this.passwordAttempts++;

                PasswordDialog dialog = new PasswordDialog(this.hasAttemptedPassword);

                // TODO: [Enter password Control]
                if (this.hasAttemptedPassword && dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;
                }
                else if (dialog.ShowDialog() == DialogResult.OK)
                {
                    password = dialog.Result;

                    this.hasAttemptedPassword = true;
                }

                this.OnLoad(this, new EventArgs(), password);
            }
            catch (ExtractionException exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show($"Error opening archive {this.currentArchive}. Invalid format.\nIf you believe this is not the case, please create an issue on the GitHub repo with the log file linked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!ErrorHandler.WriteLogFile())
                {
                    MessageBox.Show("Failed to create log file!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Environment.Exit(451);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Generic Exception caught!\nPlease create an issue on the GitHub repo with the log file linked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!ErrorHandler.WriteLogFile())
                {
                    MessageBox.Show("Failed to create log file!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Environment.Exit(666);
            }

        }

        private static ArchiveType GetArchiveType(string extension)
        {
            switch (extension.Replace(".", string.Empty))
            {
                case "rar": 
                    return ArchiveType.Rar;
                case "zip":
                case "zipx":
                    return ArchiveType.Zip;
                case "7z":
                    return ArchiveType.SevenZip;
                default:
                    return ArchiveType.Unknown;
            }
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

                if (Directory.GetFileSystemEntries(Program.TempPath(this.tempFolderNum)).Length == this.lvFileList.SelectedItems.Count)
                {
                    filesList = Directory.GetFileSystemEntries(Program.TempPath(this.tempFolderNum)).ToList();
                }

                else
                {
                    foreach (ListViewItem selectedItem in this.lvFileList.SelectedItems)
                    {

                        if (((IEntry)selectedItem.Tag).IsDirectory)
                        {
                            IEntry directoryEntry = (IEntry)selectedItem.Tag;

                            this.RecursiveExtract(Path.Combine(Program.TempPath(this.tempFolderNum), directoryEntry.Key.Replace(this.currentFolder, string.Empty)), this.currentArchiveType, directoryEntry.Key);
                        }
                        else
                        {
                            ((IArchiveEntry)selectedItem.Tag).WriteToFile(Path.Combine(Program.TempPath(this.tempFolderNum), ((IEntry)selectedItem.Tag).Key.Replace(this.currentFolder, string.Empty)));

                            filesList.Add(Path.Combine(Program.TempPath(this.tempFolderNum), ((IEntry)selectedItem.Tag).Key.Replace(this.currentFolder, string.Empty)));
                        }
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

            Directory.Delete(Program.TempPath(this.tempFolderNum), true);
            Directory.CreateDirectory(Program.TempPath(this.tempFolderNum));
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
                if (((IEntry)this.lvFileList.SelectedItems[0].Tag).IsDirectory)
                {
                    this.EnterFolder((IArchiveEntry)this.lvFileList.SelectedItems[0].Tag, this.currentArchiveType);
                }
                else
                {
                    if (GetArchiveType(((IEntry)this.lvFileList.SelectedItems[0].Tag).Key) == ArchiveType.Unknown)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ((IArchiveEntry)this.lvFileList.SelectedItems[0].Tag).WriteToFile(
                            Path.Combine(Program.TempPath(this.tempFolderNum),
                                Path.GetFileName(((IEntry)this.lvFileList.SelectedItems[0].Tag).Key)));
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Process.Start(Path.Combine(Program.TempPath(this.tempFolderNum),
                            Path.GetFileName(((IEntry)this.lvFileList.SelectedItems[0].Tag).Key)));
                    }
                    else
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ((IArchiveEntry)this.lvFileList.SelectedItems[0].Tag).WriteToFile(
                            Path.Combine(Program.TempPath(this.tempFolderNum),
                                Path.GetFileName(((IEntry)this.lvFileList.SelectedItems[0].Tag).Key)));
                        // ReSharper disable once AssignNullToNotNullAttribute
                        new ArchiveView(Path.Combine(Program.TempPath(this.tempFolderNum),
                            Path.GetFileName(((IEntry)this.lvFileList.SelectedItems[0].Tag).Key))).Show();
                    }
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

        private void RecursiveExtract(string destinationFolder, ArchiveType type, string folderEntry = "")
        {
            Directory.CreateDirectory(destinationFolder);

            switch (type)
            {
                case ArchiveType.Rar:
                {
                        RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IArchiveEntry entry in rar.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains(folderEntry) && entry.Key.Length > folderEntry.Length)
                            {
                                this.RecursiveExtract(Path.Combine(destinationFolder, new DirectoryInfo(entry.Key).Name), (ArchiveType)Convert.ToInt32(rar.Type), entry.Key);
                            }
                            else if (!entry.IsDirectory && entry.Key.Contains(folderEntry))
                            {
                                entry.WriteToFile(Path.Combine(destinationFolder, new FileInfo(entry.Key).Name));
                            }
                        }
                    }
                    break;
                case ArchiveType.Zip:
                {
                        ZipArchive archive = ZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IArchiveEntry entry in archive.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains(folderEntry) && entry.Key.Length > folderEntry.Length)
                            {
                                this.RecursiveExtract(Path.Combine(destinationFolder, new DirectoryInfo(entry.Key).Name), (ArchiveType)Convert.ToInt32(archive.Type), entry.Key);
                            }
                            else if (!entry.IsDirectory && entry.Key.Contains(folderEntry))
                            {
                                entry.WriteToFile(Path.Combine(destinationFolder, new FileInfo(entry.Key).Name));
                            }
                        }
                    }
                    break;
                case ArchiveType.Tar:
                {
                    
                }
                    break;
                case ArchiveType.SevenZip:
                {
                        SevenZipArchive archive = SevenZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IArchiveEntry entry in archive.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains(folderEntry) && entry.Key.Length > folderEntry.Length)
                            {
                                this.RecursiveExtract(Path.Combine(destinationFolder, new DirectoryInfo(entry.Key).Name), (ArchiveType)Convert.ToInt32(archive.Type), entry.Key);
                            }
                            else if (!entry.IsDirectory && entry.Key.Contains(folderEntry))
                            {
                                entry.WriteToFile(Path.Combine(destinationFolder, new FileInfo(entry.Key).Name));
                            }
                        }
                    }
                    break;
                case ArchiveType.GZip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void AddFilesAndCompress(ArchiveType type)
        {
            switch (type)
            {
                case ArchiveType.Rar:
                    {
                        // TODO Deny creation and show EULA: http://www.rarlab.com/license.htm
                        // TODO Check if user-purchased binary can legally be used from command-line
                    }
                    break;
                case ArchiveType.Zip:
                    break;
                case ArchiveType.Tar:
                    break;
                case ArchiveType.SevenZip:
                    break;
                case ArchiveType.GZip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void EnterFolder(IEntry folder, ArchiveType type)
        {
            this.folderLevel++;
            this.lvFileList.BeginUpdate();
            this.lvFileList.Items.Clear();
            this.lvFileList.EndUpdate();

            int slashes = folder.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count + 1;
            this.currentFolder = folder.Key + "\\";
            Console.Out.WriteLine(this.currentFolder.Split('\\')[this.currentFolder.Split('\\').Length - 2]);

            switch (type)
            {
                case ArchiveType.Rar:
                    {
                        RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in rar.Entries)
                        {
                            if (entry.IsDirectory && entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.Zip:
                {
                        ZipArchive archive = ZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in archive.Entries)
                        {
                            if (entry.IsDirectory && entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.Tar:
                    break;
                case ArchiveType.SevenZip:
                {
                        SevenZipArchive archive = SevenZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in archive.Entries)
                        {
                            if (entry.IsDirectory && entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{folder.Key}\\"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.GZip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            this.tbArchivePath.Text = this.currentArchive + "\\" + this.currentFolder;
        }

        private void ParentFolder(ArchiveType type)
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

            switch (type)
            {
                case ArchiveType.Rar:
                {
                        RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in rar.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.Zip:
                {
                        ZipArchive archive = ZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in archive.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.Tar:
                    break;
                case ArchiveType.SevenZip:
                {
                        SevenZipArchive archive = SevenZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                        {
                            Password = this.archivePassword
                        });
                        foreach (IEntry entry in archive.Entries)
                        {

                            if (entry.IsDirectory && entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFolder(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                            else if (entry.Key.Contains($"{this.currentFolder}"))
                            {
                                if (entry.Key.ToCharArray().ToList().FindAll(e => e == '\\').Count != slashes)
                                {
                                    continue;
                                }

                                this.AddFile(entry);
                                Console.Out.WriteLine(entry.Key);
                            }
                        }
                    }
                    break;
                case ArchiveType.GZip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            this.tbArchivePath.Text = this.currentArchive + "\\" + this.currentFolder;
        }

        private void AddFolder(IEntry folderEntry)
        {
            this.lvFileList.BeginUpdate();

            ListViewItem item = new ListViewItem(folderEntry.Key.Replace(this.currentFolder, string.Empty), 1)
            {
                Tag = folderEntry
            };

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

        private void AddFile(IEntry fileEntry)
        {
            this.lvFileList.BeginUpdate();

            ListViewItem item = new ListViewItem(fileEntry.Key.Replace(this.currentFolder, string.Empty), 1)
            {
                Tag = fileEntry
            };

            item.SubItems.Add(fileEntry.Size.ToString());
            item.SubItems.Add(fileEntry.CompressedSize.ToString());
            item.SubItems.Add(MimeTypeMap.GetMimeType(Path.GetExtension(fileEntry.Key)));

            if (fileEntry.LastModifiedTime.HasValue)
            {
                item.SubItems.Add($"{fileEntry.LastModifiedTime.Value.ToLocalTime().ToLongDateString()} {fileEntry.LastModifiedTime.Value.ToLocalTime().ToLongTimeString()}");
            }


            if (!this.iconList.Images.ContainsKey(Path.GetExtension(fileEntry.Key)))
            {
                this.iconList.Images.Add(Path.GetExtension(fileEntry.Key), IconHelper.GetFileIcon(Path.GetExtension(fileEntry.Key), IconHelper.IconSize.Small, false));
            }

            item.ImageKey = Path.GetExtension(fileEntry.Key);
            this.lvFileList.Items.Add(item);

            this.lvFileList.EndUpdate();
        }

        private void PbUpALevel_Click(object sender, EventArgs e)
        {
            if (this.folderLevel > 0)
            {
                this.ParentFolder(this.currentArchiveType);
                this.folderLevel--;
            }
        }

        private void BtnExtract_Click(object sender, EventArgs e)
        {
            this.extractionProgress = new ExtractionProgress();

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.DoWork += this.BackgroundExtractor_DoWork;
            worker.RunWorkerCompleted += this.BackgroundExtractor_RunWorkerCompleted;

            FolderBrowserDialog fldDialog = new FolderBrowserDialog
            {
                Description = "Select folder to extract to...",
                ShowNewFolderButton = true
            };

            if (fldDialog.ShowDialog() == DialogResult.OK)
            {
                worker.RunWorkerAsync(fldDialog);
                
                this.extractionProgress.Show();
            }

            this.Enabled = false;
        }

        private void BackgroundExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            this.extractionProgress.Hide();
        }

        private void BackgroundExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                FolderBrowserDialog fldDialog = e.Argument as FolderBrowserDialog;

                switch (this.currentArchiveType)
                {
                    case ArchiveType.Rar:
                        {
                            RarArchive rar = RarArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = this.archivePassword
                            });

                            int fileCount = 0;
                            foreach (IEntry rarArchiveEntry in rar.Entries)
                            {
                                if (!rarArchiveEntry.IsDirectory)
                                {
                                    fileCount++;
                                }
                            }

                            if (fldDialog != null)
                            {
                                this.extractionProgress.FileCount = fileCount;
                                this.extractionProgress.ExtractionLocation = fldDialog.SelectedPath;

                                foreach (IEntry rarArchiveEntry in rar.Entries)
                                {
                                    if (rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Creating folder {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        Directory.CreateDirectory(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }

                                foreach (IArchiveEntry rarArchiveEntry in rar.Entries)
                                {
                                    if (!rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Writing file {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        this.extractionProgress.CurrentFile = new FileInfo(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                        this.extractionProgress.ExpectedFileSize = rarArchiveEntry.Size;
                                        rarArchiveEntry.WriteToFile(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }
                            }
                            else
                            {
                                throw new NullReferenceException("The value for \"fldDialog\" was not intialized.");
                            }
                        }
                        break;
                    case ArchiveType.Zip:
                    {
                            ZipArchive archive = ZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = this.archivePassword
                            });

                            int fileCount = 0;
                            foreach (IEntry rarArchiveEntry in archive.Entries)
                            {
                                if (!rarArchiveEntry.IsDirectory)
                                {
                                    fileCount++;
                                }
                            }

                            if (fldDialog != null)
                            {
                                this.extractionProgress.FileCount = fileCount;
                                this.extractionProgress.ExtractionLocation = fldDialog.SelectedPath;

                                foreach (IEntry rarArchiveEntry in archive.Entries)
                                {
                                    if (rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Creating folder {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        Directory.CreateDirectory(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }

                                foreach (IArchiveEntry rarArchiveEntry in archive.Entries)
                                {
                                    if (!rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Writing file {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        this.extractionProgress.CurrentFile = new FileInfo(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                        this.extractionProgress.ExpectedFileSize = rarArchiveEntry.Size;
                                        rarArchiveEntry.WriteToFile(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }
                            }
                            else
                            {
                                throw new NullReferenceException("The value for \"fldDialog\" was not intialized.");
                            }
                        }
                        break;
                    case ArchiveType.Tar:
                        break;
                    case ArchiveType.SevenZip:
                    {
                            SevenZipArchive archive = SevenZipArchive.Open(new FileInfo(this.currentArchive), new ReaderOptions
                            {
                                Password = this.archivePassword
                            });

                            int fileCount = 0;
                            foreach (IEntry rarArchiveEntry in archive.Entries)
                            {
                                if (!rarArchiveEntry.IsDirectory)
                                {
                                    fileCount++;
                                }
                            }

                            if (fldDialog != null)
                            {
                                this.extractionProgress.FileCount = fileCount;
                                this.extractionProgress.ExtractionLocation = fldDialog.SelectedPath;

                                foreach (IEntry rarArchiveEntry in archive.Entries)
                                {
                                    if (rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Creating folder {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        Directory.CreateDirectory(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }

                                foreach (IArchiveEntry rarArchiveEntry in archive.Entries)
                                {
                                    if (!rarArchiveEntry.IsDirectory)
                                    {
                                        this.extractionProgress.CurrentProcess = $"Writing file {Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key)}...";
                                        this.extractionProgress.CurrentFile = new FileInfo(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                        this.extractionProgress.ExpectedFileSize = rarArchiveEntry.Size;
                                        rarArchiveEntry.WriteToFile(Path.Combine(fldDialog.SelectedPath, rarArchiveEntry.Key));
                                    }
                                }
                            }
                            else
                            {
                                throw new NullReferenceException("The value for \"fldDialog\" was not intialized.");
                            }
                        }
                        break;
                    case ArchiveType.GZip:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                if (!ErrorHandler.WriteLogFile())
                {
                    MessageBox.Show("Failed to create log file!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Generic error caught! Please create an issue on the GitHub repo with the log file linked.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                int returnVal;

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

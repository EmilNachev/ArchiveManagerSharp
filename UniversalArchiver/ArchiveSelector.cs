using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversalArchiver.DEBUG;

namespace UniversalArchiver
{
    public partial class ArchiveSelector : Form
    {
        private bool allowVisibilityChange;

        public ArchiveSelector(string file = "")
        {
            this.InitializeComponent();

            this.Load += this.ArchiveSelector_Load;

            if (file != string.Empty)
            {
                Application.Run(new ArchiveView(file));
            }
            else
            {
                FileDialog dia = new OpenFileDialog();
                dia.Filter = "Supported Archives|*.rar; *.zip";

                if (dia.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new ArchiveView(dia.FileName));
                }
            }

            if (!Application.OpenForms.OfType<IArchiver>().Any())
            {
                Environment.Exit(0);
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(this.allowVisibilityChange ? value : this.allowVisibilityChange);
        }

        private void ArchiveSelector_Load(object sender, EventArgs e)
        {
        }
    }
}

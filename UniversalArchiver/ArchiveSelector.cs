using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalArchiver
{
    public partial class ArchiveSelector : Form
    {
        public ArchiveSelector()
        {
            this.InitializeComponent();

            this.Load += this.ArchiveSelector_Load;
        }

        private void ArchiveSelector_Load(object sender, EventArgs e)
        {
            FileDialog dia = new OpenFileDialog();
            dia.Filter = "Supported Archives|*.rar";

            if (dia.ShowDialog() == DialogResult.OK)
            {
                this.Hide();
                new RarArchiveView(dia.FileName).Show();
            }
        }
    }
}

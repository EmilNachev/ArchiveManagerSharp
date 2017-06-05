using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalArchiver.Controls
{
    public partial class PasswordDialog : Form
    {
        public string Result
        {
            get;
            private set;
        }

        public PasswordDialog(bool hasAttempted)
        {
            this.InitializeComponent();

            this.lblIncorrect.Visible = hasAttempted;
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            this.Result = this.tbPassword.Text;
        }
    }
}

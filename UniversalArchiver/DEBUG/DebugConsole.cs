using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalArchiver.DEBUG
{
    public partial class DebugConsole : Form
    {

        private bool allowVisibilityChange;
        private TextBoxStreamWriter writer;

        public DebugConsole()
        {
            this.InitializeComponent();

            this.allowVisibilityChange = false;

#if DEBUG
            this.allowVisibilityChange = true;
#endif

            this.Load += this.OnLoad;

            this.writer = new TextBoxStreamWriter(this);
            Console.SetOut(this.writer);

            Program.ConsoleCreated = true;
        }

        public string ConsoleText => this.writer.Text;

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(this.allowVisibilityChange ? value : this.allowVisibilityChange);
        }

        private void OnLoad(object sender, EventArgs e)
        {
        }

        private void AddTexttoConsole(char text)
        {
            if (this.Created)
            {
                this.BeginInvoke(new Action<char>(this.AddTexttoConsole), text);
                return;
            }

#if DEBUG
            this.rtbConsole.AppendText(text.ToString());
#endif
        }


        private class TextBoxStreamWriter : TextWriter
        {
            private readonly DebugConsole output = null;

            public string Text
            {
                get;
                private set;
            }

            public TextBoxStreamWriter(DebugConsole output)
            {
                this.output = output;
                this.Text = string.Empty;
            }

            public override void Write(char value)
            {
                base.Write(value);
                this.output.AddTexttoConsole(value);
                this.Text += value;
            }

            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}

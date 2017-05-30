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
        public DebugConsole()
        {
            this.InitializeComponent();

            this.Load += this.OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            TextWriter writer = new TextBoxStreamWriter(this);
            Console.SetOut(writer);

            Program.ConsoleCreated = true;
        }

        private void AddTexttoConsole(char text)
        {
            this.Invoke(new Action(() =>
            {
                this.rtbConsole.AppendText(text.ToString());
            }));
        }


        private class TextBoxStreamWriter : TextWriter
        {
            private readonly DebugConsole output = null;

            public TextBoxStreamWriter(DebugConsole output)
            {
                this.output = output;
            }

            public override void Write(char value)
            {
                base.Write(value);
                this.output.AddTexttoConsole(value);
            }

            public override Encoding Encoding => System.Text.Encoding.UTF8;
        }
    }
}

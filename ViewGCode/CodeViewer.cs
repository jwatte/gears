using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ViewGCode
{
    public partial class CodeViewer : Form
    {
        public CodeViewer()
        {
            InitializeComponent();
        }

        private void openGcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] text = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                gCode3DControl1.Lines = text;
                richTextBox1.Lines = text;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        Point lastMousePos;

        private void gCode3DControl1_MouseDown(object sender, MouseEventArgs e)
        {
            lastMousePos = e.Location;
            gCode3DControl1.Capture = true;
        }

        private void gCode3DControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (gCode3DControl1.Capture)
            {
                gCode3DControl1.MouseDelta(e.Location.X - lastMousePos.X, e.Location.Y - lastMousePos.Y);
                lastMousePos = e.Location;
            }
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            int start = richTextBox1.SelectionStart;
            int length = richTextBox1.SelectionLength;
            if (length <= 0)
            {
                gCode3DControl1.UnhiliteLines();
            }
            else
            {
                gCode3DControl1.HiliteLines(
                    richTextBox1.GetLineFromCharIndex(start),
                        //  don't include newline
                    richTextBox1.GetLineFromCharIndex(start+length-1));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gCode3DControl1.Blink();
        }
    }
}

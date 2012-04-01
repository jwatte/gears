using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace Gears
{
    public partial class GearDesign : Form
    {
        public GearDesign()
        {
            InitializeComponent();
            ApplySettings(new RegistryLoad(@"\Software\Enchanted Age\Gear Generator\Gears"));
        }

        private GearInfo verifyGearUi(bool changeUi)
        {
            GearInfo gi = readGearInfo(changeUi);
            if (gi.pitchGpi < 1 || gi.pitchGpi > 150)
            {
                if (changeUi)
                {
                    textPitch.Focus();
                    textPitch.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            if (gi.aNumTeeth < 6 || gi.aNumTeeth > 1000)
            {
                if (changeUi)
                {
                    textGearsA.Focus();
                    textGearsA.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            if (gi.bNumTeeth < 6 || gi.bNumTeeth > 1000)
            {
                if (changeUi)
                {
                    textGearsB.Focus();
                    textGearsB.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            if (gi.pressureAngle > 32 || gi.pressureAngle < 3)
            {
                if (changeUi)
                {
                    textPressureAngle.Focus();
                    textPressureAngle.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            if (gi.stubRatio < 0.25 || gi.stubRatio > 1.5)
            {
                if (changeUi)
                {
                    textStubRatio.Focus();
                    textStubRatio.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            if (gi.profileShift < -1 || gi.profileShift > 1)
            {
                if (changeUi)
                {
                    textProfileShift.Focus();
                    textProfileShift.SelectAll();
                }
                labelError.Show();
                gi.ok = false;
                return gi;
            }
            labelError.Hide();
            return gi;
        }

        private double fieldDouble(TextBox text, ref bool ok, bool changeUi)
        {
            if (!ok)
            {
                return 0;
            }
            try
            {
                double r = double.Parse(text.Text);
                return r;
            }
            catch (Exception x)
            {
                GC.KeepAlive(x);
                if (changeUi)
                {
                    text.Focus();
                    text.SelectAll();
                }
                ok = false;
                return 0;
            }
        }

        private int fieldInt(TextBox text, ref bool ok, bool changeUi)
        {
            if (!ok)
            {
                return 0;
            }
            try
            {
                int i = int.Parse(text.Text);
                return i;
            }
            catch (Exception x)
            {
                GC.KeepAlive(x);
                if (changeUi)
                {
                    text.Focus();
                    text.SelectAll();
                }
                ok = false;
                return 0;
            }
        }

        private GearInfo readGearInfo(bool changeUi)
        {
            GearInfo gi = new GearInfo();
            gi.ok = true;
            gi.pitchGpi = fieldDouble(textPitch, ref gi.ok, changeUi);
            gi.aNumTeeth = fieldInt(textGearsA, ref gi.ok, changeUi);
            gi.bNumTeeth = fieldInt(textGearsB, ref gi.ok, changeUi);
            gi.pressureAngle = fieldDouble(textPressureAngle, ref gi.ok, changeUi);
            gi.stubRatio = fieldDouble(textStubRatio, ref gi.ok, changeUi);
            gi.profileShift = fieldDouble(textProfileShift, ref gi.ok, changeUi);
            return gi;
        }

        private void Form1_Validating(object sender, CancelEventArgs e)
        {
            verifyGearUi(true);
        }

        private void textPitch_Validating(object sender, CancelEventArgs e)
        {
            verifyGearUi(false);
        }

        private void textGearsA_Validating(object sender, CancelEventArgs e)
        {
            verifyGearUi(false);
        }

        private void textGearsB_Validating(object sender, CancelEventArgs e)
        {
            verifyGearUi(false);
        }

        private void textToothDepth_Validating(object sender, CancelEventArgs e)
        {
            verifyGearUi(false);
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void configureEverything()
        {
            this.Validate();
            ExtractSettings(new RegistrySave(@"\Software\Enchanted Age\Gear Generator\Gears"));
            GearInfo gi = verifyGearUi(true);
            gearDrawing1.setInfo(gi);
            toothForm1.setProfile(gi, true);
            toothForm2.setProfile(gi, false);
            if (export != null)
            {
                export.setInfo(toothForm1.Info, toothForm1.Calc, toothForm2.Calc, toothForm1.Profile, toothForm2.Profile);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!toothForm1.IsValid)
            {
                MessageBox.Show("The current parameters do not make a valid gear.");
                return;
            }
            if (export == null)
            {
                export = new ExportForm();
                export.FormClosed += new FormClosedEventHandler(export_FormClosed);
                export.setInfo(toothForm1.Info, toothForm1.Calc, toothForm2.Calc, toothForm1.Profile, toothForm2.Profile);
            }
            export.Show();
        }

        void export_FormClosed(object sender, FormClosedEventArgs e)
        {
            export = null;
        }

        ExportForm export;

        private void GearDesign_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (export != null)
            {
                export.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fn = openFileDialog1.FileName;
            string dir = Path.GetDirectoryName(fn);
            string file = Path.GetFileNameWithoutExtension(fn);
            Regex.Replace(file, "P-[0-9]* *", "");
            file = "P-" + textPitch.Text + " " + file;
            saveFileDialog1.FileName = Path.Combine(dir, file);
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveSettingsToFile(saveFileDialog1.FileName);
            }
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadSettingsFromFile(openFileDialog1.FileName);
            }
        }

        private void LoadSettingsFromFile(string name)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(name);
            XmlLoad xl = new XmlLoad(xd);
            ApplySettings(xl);
        }

        private void ApplySettings(IControlLoad load)
        {
            foreach (Control c in groupParameters.Controls)
            {
                SaveLoad.loadControl(load, c);
            }
            configureEverything();
        }

        private void SaveSettingsToFile(string name)
        {
            try
            {
                StreamWriter sw = new StreamWriter(name);
                sw.WriteLine("<?xml version='1.0'?>");
                sw.WriteLine("<settings version='1.0'>");
                XmlSave xs = new XmlSave(sw);
                ExtractSettings(xs);
                sw.WriteLine("</settings>");
                sw.Close();
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Could not save " + name + "\n" + x.Message);
            }
        }

        private void ExtractSettings(IControlSave save)
        {
            foreach (Control c in groupParameters.Controls)
            {
                SaveLoad.saveControl(save, c);
            }
        }

        private void textPitch_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void textGearsA_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void textGearsB_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void textPressureAngle_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void textStubRatio_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }

        private void textProfileShift_Leave(object sender, EventArgs e)
        {
            configureEverything();
        }
    }
}

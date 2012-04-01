using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Gears
{
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
            labelDirectory.Text = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            rxInch = new Regex("^\\s*(\\d+)\\s*/\\s*(\\d+)\\s*(?:in(?:ch)?\\.?)?\\s*$");
            comboFaceMilling.Text = doNotMill;
            string path = Microsoft.Win32.Registry.GetValue(
                Microsoft.Win32.Registry.CurrentUser.Name + @"\Software\Enchanted Age\Gear Generator\Export",
                @"lastSettingsFile",
                null) as string;
            if (path != null && File.Exists(path))
            {
                saveFileDialog1.FileName = path;
                openFileDialog1.FileName = path;
                loadSettingsFromFile(path);
            }
        }

        Regex rxInch;
        GearInfo info_;
        GearCalc calcPinion_;
        GearCalc calcGear_;
        PointF[] profileA_;
        PointF[] profileB_;

        public void setInfo(GearInfo info, GearCalc gearA, GearCalc gearB, PointF[] profileA, PointF[] profileB)
        {
            info_ = info;
            calcPinion_ = gearA;
            calcGear_ = gearB;
            profileA_ = profileA;
            profileB_ = profileB;
            labelExample.Text = makeFileName(true, false);
        }

        private double validateInch(object sender, CancelEventArgs args)
        {
            (sender as Control).BackColor = SystemColors.Window;
            string v = ((TextBox)sender).Text;
            if (rxInch.IsMatch(v))
            {
                Match m = rxInch.Match(v);
                int a = 0, b = 0;
                if (!Int32.TryParse(m.Groups[1].Value, out a)
                    || !Int32.TryParse(m.Groups[2].Value, out b))
                {
                    args.Cancel = true;
                    (sender as Control).BackColor = Color.Yellow;
                    (sender as Control).Focus();
                    if (sender is TextBox)
                    {
                        (sender as TextBox).SelectAll();
                    }
                    return 0;
                }
                if (b < 2 || a < 1)
                {
                    (sender as Control).BackColor = Color.Yellow;
                    (sender as Control).Focus();
                    if (sender is TextBox)
                    {
                        (sender as TextBox).SelectAll();
                    }
                    args.Cancel = true;
                    return 0;
                }
                return (double)a / (double)b;
            }
            double dv = 0;
            if (!Double.TryParse(v, out dv) || dv < 0)
            {
                (sender as Control).BackColor = Color.Yellow;
                (sender as Control).Focus();
                if (sender is TextBox)
                {
                    (sender as TextBox).SelectAll();
                }
                args.Cancel = true;
            }
            return dv;
        }
        
        private void textPlateThickness_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textRidgeDepth_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textStockThickness_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textHoleDiameter_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textHoleWallThickness_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textRidgeThickness_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textPlateMargin_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void comboFaceMilling_Validating(object sender, CancelEventArgs e)
        {
        }

        private void textBoxEndMillDiameter_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        private void textToothMillDiameter_Validating(object sender, CancelEventArgs e)
        {
            validateInch(sender, e);
        }

        Regex fileNamePattern = new Regex("{\\d+}");
        Regex fileNameBadPattern = new Regex("{\\D*}");

        private void textBoxFileNamePattern_Validating(object sender, CancelEventArgs e)
        {
            string v = textBoxFileNamePattern.Text;
            if (v.Length < 4)
            {
                e.Cancel = true;
                return;
            }
            string ext = System.IO.Path.GetExtension(v);
            if (ext == null || ext == "")
            {
                e.Cancel = true;
                return;
            }
            if (fileNameBadPattern.IsMatch(v))
            {
                e.Cancel = true;
                return;
            }
            MatchCollection mc = fileNamePattern.Matches(textBoxFileNamePattern.Text);
            if (mc != null && mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    int i;
                    if (!Int32.TryParse(m.Value.Substring(1, m.Value.Length - 2), out i))
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (i < 0 || i > 7)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void setToothMillEnabled(bool enabled)
        {
            Color c = (enabled ? SystemColors.ControlText : SystemColors.GrayText);
            labelToothCuttingDepth.ForeColor = c;
            labelToothCuttingDepthUnit.ForeColor = c;
            labelToothMillDiameter.ForeColor = c;
            labelToothMillDiameterUnit.ForeColor = c;
            labelToothPassDepth.ForeColor = c;
            labelToothPassDepthUnit.ForeColor = c;
            labelToothStepOver.ForeColor = c;
            labelToothStepOverUnit.ForeColor = c;
            textToothCuttingDepth.Enabled = enabled;
            textToothMillDiameter.Enabled = enabled;
            textToothPassDepth.Enabled = enabled;
            textToothStepOver.Enabled = enabled;
            if (enabled)
            {
                textToothMillDiameter.Focus();
                textToothMillDiameter.SelectAll();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            setToothMillEnabled(checkUseToothMill.Checked);
        }

        private void buttonDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                labelDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void textToothCuttingDepth_Validating(object sender, CancelEventArgs e)
        {
            if (checkUseToothMill.Checked)
            {
                validateInch(sender, e);
            }
        }

        private void textToothPassDepth_Validating(object sender, CancelEventArgs e)
        {
            if (checkUseToothMill.Checked)
            {
                validateInch(sender, e);
            }
        }

        private void textToothStepOver_Validating(object sender, CancelEventArgs e)
        {
            if (checkUseToothMill.Checked)
            {
                validateInch(sender, e);
            }
        }

        private void textToothThickness_Validating(object sender, CancelEventArgs e)
        {
            if (checkUseToothMill.Checked)
            {
                validateInch(sender, e);
            }
        }

        private void textBoxFileNamePattern_Validated(object sender, EventArgs e)
        {
            labelExample.Text = makeFileName(true, false);
        }

        private string makeFileName(bool pinion, bool toothProfile)
        {
            return String.Format(textBoxFileNamePattern.Text, 
                (int)info_.pitchGpi,
                (int)(pinion ? info_.aNumTeeth : info_.bNumTeeth),
                String.Format("{0:#.##}", info_.pressureAngle),
                String.Format("{0:#.##}", info_.profileShift),
                String.Format("{0:#.###}", (pinion ? calcPinion_.radius + calcPinion_.addendum : calcGear_.radius + calcGear_.addendum)),
                String.Format("{0:#.###}", validateInch(toothProfile ? textToothMillDiameter : textBoxEndMillDiameter, new CancelEventArgs())),
                String.Format("{0:#.##}", validateInch(textStockThickness, new CancelEventArgs())),
                pinion ? "Pinion" : "Gear");
        }

        private void buttonFilePatternHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "{0}: Pitch\n{1}: Num Teeth\n{2}: Pressure Angle\n{3}: Profile Shift\n{4}: Outer diameter\n{5}: Stock thickness\n{6}: Mill diameter\n{7}: Pinion/Gear",
                "Help on pattern format codes");
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            labelExample.Text = makeFileName(true, false);
            GCodeWorkOrder[] orders = makeWorkOrders();
            if (orders != null)
            {
                if (executeWorkOrders(orders))
                {
                    MessageBox.Show("Files were exported into " + orders[0].fileInfo.directory);
                }
            }
        }

        private bool executeWorkOrders(GCodeWorkOrder[] orders)
        {
            try
            {
                foreach (GCodeWorkOrder wo in orders)
                {
                    executeWorkOrder(wo);
                }
            }
            catch (System.Exception x)
            {
                MessageBox.Show(
                    "Could not export gcode.\n" + x.Message,
                    "Error Exporting");
                return false;
            }
            return true;
        }

        private GCodeWorkOrder[] makeWorkOrders()
        {
            string error = "";
            string kind = "pinion";
            GCodeWorkOrder woPinion = new GCodeWorkOrder();
            GCodeWorkOrder woGear = new GCodeWorkOrder();
            if (!gatherWorkOrder(true, ref woPinion))
            {
                goto showError;
            }
            Control errControl;
            if (!verifyWorkOrder(woPinion, ref error, out errControl))
            {
                if (errControl is TextBox)
                {
                    (errControl as TextBox).SelectAll();
                }
                goto showError;
            }
            kind = "gear";
            error = "";
            if (!gatherWorkOrder(false, ref woGear))
            {
                goto showError;
            }
            if (!verifyWorkOrder(woGear, ref error, out errControl))
            {
                if (errControl != null)
                {
                    if (errControl is TextBox)
                    {
                        (errControl as TextBox).SelectAll();
                    }
                }
                goto showError;
            }
            return new GCodeWorkOrder[] { woPinion, woGear };
        showError:
            MessageBox.Show(
                "Cannot create Gcode for the " + kind + " because of incorrect parameters.\nPlease correct the error and try again.\n" + error,
                "Incorrect parameter");
            return null;
        }

        internal bool verifyWorkOrder(GCodeWorkOrder wo, ref string error, out Control errControl)
        {
            errControl = null;
            error = "";

            //  verify that the various circular features can fit
            double baseDiameter = wo.calc.radius - wo.calc.dedendum;
            double circularSum = wo.dimensions.ridgeMargin + wo.dimensions.ridgeThickness + 
                wo.dimensions.holeWallThickness + wo.dimensions.holeDiameter * 0.5f;
            if (circularSum > baseDiameter)
            {
                error = "The specified ridge, wall, hole and margin thicknesses sum up ("
                    + circularSum.ToString("#.##") + ") to more than the base diameter (" 
                    + baseDiameter.ToString("#.##") + ").";
                errControl = textRidgeThickness;
                return false;
            }

            //  verify that the end mill can cut deep enough
            if (wo.dimensions.holeDiameter > 0 && wo.mainMill.cuttingDepth < wo.dimensions.stockThickness)
            {
                error = "The end mill cannot cut through the stock, and a hole is specified.";
                errControl = textHoleDiameter;
                return false;
            }
            double ridgeTop = wo.dimensions.plateThickness + wo.dimensions.toothThickness +
                wo.dimensions.ridgeHeight;
            if (wo.dimensions.stockThickness - ridgeTop > wo.mainMill.cuttingDepth)
            {
                error = "The end mill cannot cut down to the top of the ridge features or teeth.";
                errControl = textMaxCuttingDepth;
                return false;
            }
            if (wo.dimensions.ridgeHeight > wo.mainMill.cuttingDepth)
            {
                error = "The end mill cannot cut down the extent of the ridge height.";
                errControl = textRidgeHeight;
                return false;
            }
            if (wo.dimensions.toothThickness > wo.mainMill.cuttingDepth)
            {
                error = "The end mill cannot cut down the extent of the teeth.";
                errControl = textToothThickness;
                return false;
            }
            if (wo.dimensions.plateThickness + wo.dimensions.toothThickness > wo.mainMill.cuttingDepth)
            {
                error = "The end mill cannot cut down the extent of the plate and tooth depth.";
                errControl = textPlateThickness;
                return false;
            }
            if (wo.dimensions.plateThickness + wo.dimensions.toothThickness + wo.dimensions.ridgeHeight -
                wo.dimensions.ridgeMargin > wo.mainMill.cuttingDepth)
            {
                error = "The end mill cannot cut down the extent of ridge plus tooth plus plate, " +
                    "and enough ridge margin is not specified.";
                errControl = textRidgeMargin;
                return false;
            }

            //  verify that face milling will work
            if (wo.dimensions.faceMilling != FaceMilling.DoNotMill)
            {
                if (wo.calc.radius - wo.calc.dedendum - wo.dimensions.ridgeMargin - wo.dimensions.ridgeThickness
                    - wo.dimensions.holeWallThickness - wo.dimensions.holeDiameter * 0.5f <
                    wo.mainMill.diameter)
                {
                    error = "The end mill is bigger than the face width, and face milling is specified.";
                    errControl = comboFaceMilling;
                    return false;
                }
            }
            if (wo.dimensions.faceMilling == FaceMilling.MillToPlate && wo.dimensions.plateThickness == 0)
            {
                error = "Face milling is set to mill to plate, but no plate thickness is specified.";
                errControl = comboFaceMilling;
                return false;
            }
            if (wo.dimensions.faceMilling == FaceMilling.MillToPlate && 
                wo.dimensions.holeWallThickness > 0 &&
                wo.mainMill.cuttingDepth < wo.dimensions.stockThickness - wo.dimensions.plateThickness)
            {
                error = "Face milling is set to mill to plate, and a hole wall is specified, but the end mill cannot cut the stock down to the plate.";
                errControl = comboFaceMilling;
                return false;
            }
            if (wo.dimensions.faceMilling == FaceMilling.MillToPlate &&
                wo.dimensions.ridgeHeight + wo.dimensions.toothThickness > wo.mainMill.cuttingDepth)
            {
                error = "Face milling is set to mill to plate, but the end mill cannot cut the extent of the ridge height plus tooth height.";
                errControl = comboFaceMilling;
                return false;
            } 

            //  verify that hole milling will work
            if (wo.dimensions.holeDiameter > 0 && wo.mainMill.diameter > wo.dimensions.holeDiameter)
            {
                error = "The end mill is bigger than the specified hole.";
                errControl = textHoleDiameter;
                return false;
            }

            //  verify that tooth milling has a chance of working
            double oneThirdToothSpacing = (wo.calc.radius - wo.calc.dedendum) * 2 * Math.PI / wo.calc.numTeeth / 3;
            if (!wo.useToothMill && wo.mainMill.diameter > oneThirdToothSpacing)
            {
                error = "The end mill is bigger than one-third the tooth spacing ("
                    + oneThirdToothSpacing.ToString("#.##") +"), and no tooth specific mill is specified.";
                errControl = textBoxEndMillDiameter;
                return false;
            }
            if (!wo.useToothMill &&
                wo.mainMill.cuttingDepth + wo.dimensions.ridgeMargin < wo.dimensions.toothThickness + wo.dimensions.ridgeHeight)
            {
                error = "There is not sufficient clearance for the mill between the tooth and the ridge.";
                errControl = textRidgeMargin;
                return false;
            }
            if (wo.useToothMill && wo.toothMill.diameter > oneThirdToothSpacing)
            {
                error = "The tooth mill is bigger than one-third the tooth spacing ("
                    + oneThirdToothSpacing.ToString("#.##") + ") and cannot reliably mill tooth shapes.";
                errControl = textToothMillDiameter;
                return false;
            }
            if (wo.useToothMill && wo.toothMill.cuttingDepth < wo.dimensions.toothThickness)
            {
                error = "The tooth mill cannot cut the full tooth thickness.";
                errControl = textToothCuttingDepth;
                return false;
            }
            if (wo.useToothMill &&
                wo.toothMill.cuttingDepth + wo.dimensions.ridgeMargin < wo.dimensions.toothThickness + wo.dimensions.ridgeHeight)
            {
                error = "There is not sufficient clearance for the tooth mill to the ridge.";
                errControl = textRidgeMargin;
                return false;
            }

            if (wo.useToothMill && wo.toothMill.diameter.ToString(".##") == wo.mainMill.diameter.ToString(".##"))
            {
                error = "The tooth mill diameter is the same as the main mill diameter.";
                errControl = textToothMillDiameter;
                return false;
            }

            //  verify feeds and speeds (just loosely)
            if (wo.mainMill.feed < 0.1f || wo.mainMill.feed > 50)
            {
                error = "The feed is out of range [0.1, 50].";
                errControl = textMillFeed;
                return false;
            }
            if (wo.mainMill.speed < 250 || wo.mainMill.speed > 25000)
            {
                error = "The speed is out of range [250, 25000].";
                errControl = textMillSpeed;
                return false;
            }

            if (wo.useToothMill && (wo.mainMill.feed < 0.1f || wo.mainMill.feed > 50))
            {
                error = "The tooth mill feed is out of range [0.1, 50].";
                errControl = textToothFeed;
                return false;
            }
            if (wo.useToothMill && (wo.mainMill.speed < 250 || wo.mainMill.speed > 25000))
            {
                error = "The tooth mill speed is out of range [250, 25000].";
                errControl = textToothSpeed;
                return false;
            }

            if (!Directory.Exists(wo.fileInfo.directory))
            {
                error = "The output directory does not exist.\n" + wo.fileInfo.directory;
                errControl = buttonDirectory;
                return false;
            }

            return true;
        }

        internal bool gatherWorkOrder(bool pinion, ref GCodeWorkOrder wo)
        {
            wo.ok = false;
            wo.info = info_;
            if (!wo.info.ok)
            {
                return false;
            }
            wo.calc = pinion ? calcPinion_ : calcGear_;
            if (!wo.calc.ok)
            {
                return false;
            }
            wo.profile = pinion ? profileA_ : profileB_;
            if (wo.profile == null)
            {
                return false;
            }
            wo.numTeeth = pinion ? info_.aNumTeeth : info_.bNumTeeth;
            if (!getDimensions(ref wo.dimensions))
            {
                return false;
            }
            if (!getMillInfo(ref wo.mainMill))
            {
                return false;
            }
            wo.useToothMill = checkUseToothMill.Checked;
            if (wo.useToothMill)
            {
                if (!getToothMillInfo(ref wo.toothMill))
                {
                    return false;
                }
            }
            getFileInfo(pinion, ref wo.fileInfo);
            wo.ok = true;
            return true;
        }

        internal void getFileInfo(bool pinion, ref WorkFileInfo fi)
        {
            fi.directory = labelDirectory.Text;
            fi.fileName = makeFileName(pinion, false);
            fi.toothFileName = makeFileName(pinion, true);
            fi.fileFormat = comboFileFormat.Text == "svg" ? FileFormat.svg : FileFormat.gCode;
        }

        internal bool getDimensions(ref Dimensions dim)
        {
            CancelEventArgs cea = new CancelEventArgs();
            cea.Cancel = false;

            dim.faceMilling = stringToEnum(comboFaceMilling.Text);
            if (dim.faceMilling == FaceMilling.MillInvalid)
            {
                return false;
            }
            dim.holeDiameter = validateInch(textHoleDiameter, cea);
            if (cea.Cancel) {
                return false;
            }
            dim.holeWallThickness = validateInch(textHoleWallThickness, cea);
            if (cea.Cancel) {
                return false;
            }
            dim.plateMargin = validateInch(textPlateMargin, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.plateThickness = validateInch(textPlateThickness, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.ridgeHeight = validateInch(textRidgeHeight, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.ridgeMargin = validateInch(textRidgeMargin, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.ridgeThickness = validateInch(textRidgeThickness, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.stockThickness = validateInch(textStockThickness, cea);
            if (cea.Cancel)
            {
                return false;
            }
            dim.toothThickness = validateInch(textToothThickness, cea);
            if (cea.Cancel)
            {
                return false;
            }
            return true;
        }

        internal bool getMillInfo(ref MillInfo mi)
        {
            CancelEventArgs cea = new CancelEventArgs();
            cea.Cancel = false;
            mi.cuttingDepth = validateInch(textMaxCuttingDepth, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.diameter = validateInch(textBoxEndMillDiameter, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.passDepth = validateInch(textPassDepth, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.stepOver = validateInch(textStepOver, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.feed = validateInch(textMillFeed, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.speed = validateInch(textMillSpeed, cea);
            if (cea.Cancel)
            {
                return false;
            }
            return true;
        }

        internal bool getToothMillInfo(ref MillInfo mi)
        {
            CancelEventArgs cea = new CancelEventArgs();
            cea.Cancel = false;
            mi.cuttingDepth = validateInch(textToothCuttingDepth, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.diameter = validateInch(textToothMillDiameter, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.passDepth = validateInch(textToothPassDepth, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.stepOver = validateInch(textToothStepOver, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.feed = validateInch(textToothFeed, cea);
            if (cea.Cancel)
            {
                return false;
            }
            mi.speed = validateInch(textToothSpeed, cea);
            if (cea.Cancel)
            {
                return false;
            }
            return true;
        }

        internal static FaceMilling stringToEnum(string s)
        {
            if (s == doNotMill) return FaceMilling.DoNotMill;
            if (s == millToRidge) return FaceMilling.MillToRidge;
            if (s == millToTeeth) return FaceMilling.MillToTeeth;
            if (s == millToPlate) return FaceMilling.MillToPlate;
            return FaceMilling.MillInvalid;
        }

        internal static string enumToString(FaceMilling fm)
        {
            switch(fm)
            {
                case FaceMilling.DoNotMill: return doNotMill;
                case FaceMilling.MillToRidge: return millToRidge;
                case FaceMilling.MillToTeeth: return millToTeeth;
                case FaceMilling.MillToPlate: return millToPlate;
                default: return doNotMill;
            }
        }

        internal static string doNotMill = "Do not mill";
        internal static string millToRidge = "Mill to ridge";
        internal static string millToTeeth = "Mill to teeth";
        internal static string millToPlate = "Mill to plate";

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadSettingsFromFile(openFileDialog1.FileName);
            }
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GCodeWorkOrder[] orders = makeWorkOrders();
            if (orders == null)
            {
                return;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveSettingsToFile(saveFileDialog1.FileName);
                Microsoft.Win32.Registry.SetValue(
                    Microsoft.Win32.Registry.CurrentUser.Name + @"\Software\Enchanted Age\Gear Generator\Export",
                    @"lastSettingsFile",
                    saveFileDialog1.FileName);
            }
        }

        private void loadControls(IControlLoad xl)
        {
            foreach (Control c in groupBox1.Controls)
            {
                SaveLoad.loadControl(xl, c);
            }
            foreach (Control c in groupBox2.Controls)
            {
                SaveLoad.loadControl(xl, c);
            }
            string sVal = "";
            if (xl.LoadTextBox("labelDirectory", ref sVal))
            {
                labelDirectory.Text = sVal;
            }
        }

        private void loadSettingsFromFile(string path)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(path);
                XmlLoad xl = new XmlLoad(xd);
                loadControls(xl);
                Microsoft.Win32.Registry.SetValue(
                    Microsoft.Win32.Registry.CurrentUser.Name + @"\Software\Enchanted Age\Gear Generator\Export",
                    @"lastSettingsFile",
                    path);
            }
            catch (System.Exception x)
            {
                MessageBox.Show(
                    "Loading from " + path + " failed:\n" + x.Message,
                    "Load Settings Error");
            }
        }

        private void saveSettingsToFile(string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version='1.0'?>");
                sw.WriteLine("<settings version='1.0'>");
                XmlSave xs = new XmlSave(sw);
                SaveControls(xs);
                sw.WriteLine("</settings>");
            }
        }

        private void SaveControls(IControlSave cs)
        {
            foreach (Control c in groupBox1.Controls)
            {
                SaveLoad.saveControl(cs, c);
            }
            foreach (Control c in groupBox2.Controls)
            {
                SaveLoad.saveControl(cs, c);
            }
            cs.SaveTextBox(labelDirectory.Name, labelDirectory.Text);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static void executeWorkOrder(GCodeWorkOrder wo)
        {
            //  finally, the rubber meets the road!
            GCodeGenerator gpin = new GCodeGenerator(wo);
            File.WriteAllText(Path.Combine(wo.fileInfo.directory, wo.fileInfo.fileName),
                gpin.generateMainMill(!wo.useToothMill));
            if (wo.useToothMill)
            {
                File.WriteAllText(Path.Combine(wo.fileInfo.directory, wo.fileInfo.toothFileName),
                    gpin.generateToothMill());
            }
        }


        private void ExportForm_Load(object sender, EventArgs e)
        {
            loadControls(new RegistryLoad(@"\Software\Enchanted Age\Gear Generator\Export"));
        }

        private void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveControls(new RegistrySave(@"\Software\Enchanted Age\Gear Generator\Export"));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pat = textBoxFileNamePattern.Text;
            int i = pat.LastIndexOf('.');
            if (i >= 0)
            {
                pat = pat.Substring(0, i);
            }
            pat = pat + "." + comboFileFormat.Text;
            textBoxFileNamePattern.Text = pat;
        }
    }

    public enum FaceMilling
    {
        DoNotMill,
        MillToRidge,
        MillToTeeth,
        MillToPlate,
        MillInvalid
    }

    public struct Dimensions
    {
        public double stockThickness;
        public double toothThickness;
        public double ridgeHeight;
        public double plateThickness;
        public double plateMargin;
        public double ridgeMargin;
        public double ridgeThickness;
        public double holeWallThickness;
        public double holeDiameter;
        public FaceMilling faceMilling;
    }

    public struct MillInfo
    {
        public double diameter;
        public double cuttingDepth;
        public double passDepth;
        public double stepOver;
        public double feed;
        public double speed;
    }

    public struct WorkFileInfo
    {
        public string directory;
        public string fileName;
        public string toothFileName;
        public FileFormat fileFormat;
    }

    public struct GCodeWorkOrder
    {
        public GearInfo info;
        public GearCalc calc;
        public PointF[] profile;
        public Dimensions dimensions;
        public MillInfo mainMill;
        public MillInfo toothMill;
        public WorkFileInfo fileInfo;
        public bool useToothMill;
        public bool ok;
        public int numTeeth;
    }

    public enum FileFormat
    {
        gCode,
        svg
    }

}

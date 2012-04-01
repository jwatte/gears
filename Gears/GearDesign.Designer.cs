namespace Gears
{
    partial class GearDesign
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textPitch = new System.Windows.Forms.TextBox();
            this.textGearsA = new System.Windows.Forms.TextBox();
            this.textGearsB = new System.Windows.Forms.TextBox();
            this.textPressureAngle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelError = new System.Windows.Forms.Label();
            this.btnDraw = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupParameters = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textProfileShift = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textStubRatio = new System.Windows.Forms.TextBox();
            this.gearDrawing1 = new Gears.GearDrawing();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toothForm1 = new Gears.ToothForm();
            this.toothForm2 = new Gears.ToothForm();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupParameters.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textPitch
            // 
            this.textPitch.Location = new System.Drawing.Point(114, 29);
            this.textPitch.Name = "textPitch";
            this.textPitch.Size = new System.Drawing.Size(100, 22);
            this.textPitch.TabIndex = 0;
            this.textPitch.Text = "24";
            this.textPitch.Leave += new System.EventHandler(this.textPitch_Leave);
            this.textPitch.Validating += new System.ComponentModel.CancelEventHandler(this.textPitch_Validating);
            // 
            // textGearsA
            // 
            this.textGearsA.Location = new System.Drawing.Point(114, 58);
            this.textGearsA.Name = "textGearsA";
            this.textGearsA.Size = new System.Drawing.Size(100, 22);
            this.textGearsA.TabIndex = 1;
            this.textGearsA.Text = "14";
            this.textGearsA.Leave += new System.EventHandler(this.textGearsA_Leave);
            this.textGearsA.Validating += new System.ComponentModel.CancelEventHandler(this.textGearsA_Validating);
            // 
            // textGearsB
            // 
            this.textGearsB.Location = new System.Drawing.Point(114, 87);
            this.textGearsB.Name = "textGearsB";
            this.textGearsB.Size = new System.Drawing.Size(100, 22);
            this.textGearsB.TabIndex = 2;
            this.textGearsB.Text = "56";
            this.textGearsB.Leave += new System.EventHandler(this.textGearsB_Leave);
            this.textGearsB.Validating += new System.ComponentModel.CancelEventHandler(this.textGearsB_Validating);
            // 
            // textPressureAngle
            // 
            this.textPressureAngle.Location = new System.Drawing.Point(114, 116);
            this.textPressureAngle.Name = "textPressureAngle";
            this.textPressureAngle.Size = new System.Drawing.Size(100, 22);
            this.textPressureAngle.TabIndex = 3;
            this.textPressureAngle.Text = "20";
            this.textPressureAngle.Leave += new System.EventHandler(this.textPressureAngle_Leave);
            this.textPressureAngle.Validating += new System.ComponentModel.CancelEventHandler(this.textToothDepth_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Pitch";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Gears A";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Gears B";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Pressure Angle";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(220, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "teeth/d-inch";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(220, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 17);
            this.label6.TabIndex = 9;
            this.label6.Text = "num teeth";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(221, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 17);
            this.label7.TabIndex = 10;
            this.label7.Text = "num teeth";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(220, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 11;
            this.label8.Text = "degrees";
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.ForeColor = System.Drawing.Color.Crimson;
            this.labelError.Location = new System.Drawing.Point(111, 199);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(111, 17);
            this.labelError.TabIndex = 12;
            this.labelError.Text = "Input incomplete";
            // 
            // btnDraw
            // 
            this.btnDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDraw.Location = new System.Drawing.Point(178, 221);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(91, 27);
            this.btnDraw.TabIndex = 14;
            this.btnDraw.Text = "Draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupParameters);
            this.splitContainer1.Panel1.Controls.Add(this.gearDrawing1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1256, 644);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 15;
            // 
            // groupParameters
            // 
            this.groupParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupParameters.Controls.Add(this.btnExport);
            this.groupParameters.Controls.Add(this.label12);
            this.groupParameters.Controls.Add(this.label11);
            this.groupParameters.Controls.Add(this.textProfileShift);
            this.groupParameters.Controls.Add(this.label10);
            this.groupParameters.Controls.Add(this.label9);
            this.groupParameters.Controls.Add(this.textStubRatio);
            this.groupParameters.Controls.Add(this.btnDraw);
            this.groupParameters.Controls.Add(this.textPitch);
            this.groupParameters.Controls.Add(this.textGearsA);
            this.groupParameters.Controls.Add(this.textGearsB);
            this.groupParameters.Controls.Add(this.labelError);
            this.groupParameters.Controls.Add(this.textPressureAngle);
            this.groupParameters.Controls.Add(this.label8);
            this.groupParameters.Controls.Add(this.label1);
            this.groupParameters.Controls.Add(this.label7);
            this.groupParameters.Controls.Add(this.label2);
            this.groupParameters.Controls.Add(this.label6);
            this.groupParameters.Controls.Add(this.label3);
            this.groupParameters.Controls.Add(this.label5);
            this.groupParameters.Controls.Add(this.label4);
            this.groupParameters.Location = new System.Drawing.Point(12, 3);
            this.groupParameters.Name = "groupParameters";
            this.groupParameters.Size = new System.Drawing.Size(276, 254);
            this.groupParameters.TabIndex = 16;
            this.groupParameters.TabStop = false;
            this.groupParameters.Text = "Parameters";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(7, 222);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 27);
            this.btnExport.TabIndex = 21;
            this.btnExport.Text = "Export...";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(221, 177);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(36, 17);
            this.label12.TabIndex = 20;
            this.label12.Text = "ratio";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(30, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 17);
            this.label11.TabIndex = 19;
            this.label11.Text = "Profile shift";
            // 
            // textProfileShift
            // 
            this.textProfileShift.Location = new System.Drawing.Point(114, 174);
            this.textProfileShift.Name = "textProfileShift";
            this.textProfileShift.Size = new System.Drawing.Size(100, 22);
            this.textProfileShift.TabIndex = 18;
            this.textProfileShift.Text = "0";
            this.textProfileShift.Leave += new System.EventHandler(this.textProfileShift_Leave);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(221, 148);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 17);
            this.label10.TabIndex = 17;
            this.label10.Text = "ratio";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(71, 148);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 17);
            this.label9.TabIndex = 16;
            this.label9.Text = "Stub";
            // 
            // textStubRatio
            // 
            this.textStubRatio.Location = new System.Drawing.Point(114, 145);
            this.textStubRatio.Name = "textStubRatio";
            this.textStubRatio.Size = new System.Drawing.Size(100, 22);
            this.textStubRatio.TabIndex = 15;
            this.textStubRatio.Text = "1";
            this.textStubRatio.Leave += new System.EventHandler(this.textStubRatio_Leave);
            // 
            // gearDrawing1
            // 
            this.gearDrawing1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gearDrawing1.BackColor = System.Drawing.SystemColors.Window;
            this.gearDrawing1.Location = new System.Drawing.Point(12, 263);
            this.gearDrawing1.Name = "gearDrawing1";
            this.gearDrawing1.Size = new System.Drawing.Size(276, 369);
            this.gearDrawing1.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.toothForm1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.toothForm2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(955, 638);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toothForm1
            // 
            this.toothForm1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toothForm1.BackColor = System.Drawing.SystemColors.Window;
            this.toothForm1.Location = new System.Drawing.Point(3, 3);
            this.toothForm1.Name = "toothForm1";
            this.toothForm1.Size = new System.Drawing.Size(471, 632);
            this.toothForm1.TabIndex = 0;
            // 
            // toothForm2
            // 
            this.toothForm2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toothForm2.BackColor = System.Drawing.SystemColors.Window;
            this.toothForm2.Location = new System.Drawing.Point(480, 3);
            this.toothForm2.Name = "toothForm2";
            this.toothForm2.Size = new System.Drawing.Size(472, 632);
            this.toothForm2.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1256, 28);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
            this.loadSettingsToolStripMenuItem.Text = "Load Settings...";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
            this.saveSettingsToolStripMenuItem.Text = "Save Settings...";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(227, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "gsx";
            this.openFileDialog1.FileName = "Gear Settings";
            this.openFileDialog1.Filter = "Gear Settings (*.gsx)|*.gxs|All Files (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "gsx";
            this.saveFileDialog1.FileName = "Gear Settings";
            this.saveFileDialog1.Filter = "Gear Settings (*.gsx)|*.gxs|All Files (*.*)|*.*";
            // 
            // GearDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 672);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "GearDesign";
            this.Text = "Involute Gear Maker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GearDesign_FormClosing);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.Form1_Validating);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupParameters.ResumeLayout(false);
            this.groupParameters.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textPitch;
        private System.Windows.Forms.TextBox textGearsA;
        private System.Windows.Forms.TextBox textGearsB;
        private System.Windows.Forms.TextBox textPressureAngle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelError;
        private GearDrawing gearDrawing1;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupParameters;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ToothForm toothForm1;
        private ToothForm toothForm2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textStubRatio;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textProfileShift;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}


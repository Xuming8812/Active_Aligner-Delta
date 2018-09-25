namespace Delta
{
    partial class fFunction
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fFunction));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.sbr = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.pbRunning = new System.Windows.Forms.PictureBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnUnload = new System.Windows.Forms.Button();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.scLeftRight = new System.Windows.Forms.SplitContainer();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabTPS = new System.Windows.Forms.TabPage();
            this.dgvScript = new System.Windows.Forms.DataGridView();
            this.Label9 = new System.Windows.Forms.Label();
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.tabDut = new System.Windows.Forms.TabPage();
            this.dgvDUT = new System.Windows.Forms.DataGridView();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.nudFailIndex = new System.Windows.Forms.NumericUpDown();
            this.chkAutoLoad = new System.Windows.Forms.CheckBox();
            this.nudPassIndex = new System.Windows.Forms.NumericUpDown();
            this.tabLens = new System.Windows.Forms.TabPage();
            this.panelPartTray = new System.Windows.Forms.Panel();
            this.Label8 = new System.Windows.Forms.Label();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.Label6 = new System.Windows.Forms.Label();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.img = new System.Windows.Forms.ImageList(this.components);
            this.sbr.SuspendLayout();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRunning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scLeftRight)).BeginInit();
            this.scLeftRight.Panel1.SuspendLayout();
            this.scLeftRight.Panel2.SuspendLayout();
            this.scLeftRight.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tabTPS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScript)).BeginInit();
            this.tabDut.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDUT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFailIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPassIndex)).BeginInit();
            this.tabLens.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menu.Size = new System.Drawing.Size(1300, 24);
            this.menu.TabIndex = 9;
            this.menu.Text = "MenuStrip1";
            // 
            // sbr
            // 
            this.sbr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.sbr.Location = new System.Drawing.Point(0, 583);
            this.sbr.Name = "sbr";
            this.sbr.Size = new System.Drawing.Size(1300, 22);
            this.sbr.TabIndex = 8;
            this.sbr.Text = "StatusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1285, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Some Notes";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.pbRunning);
            this.Panel1.Controls.Add(this.lblTime);
            this.Panel1.Controls.Add(this.btnAbort);
            this.Panel1.Controls.Add(this.btnPause);
            this.Panel1.Controls.Add(this.btnRun);
            this.Panel1.Controls.Add(this.btnUnload);
            this.Panel1.Controls.Add(this.txtSN);
            this.Panel1.Controls.Add(this.Label1);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel1.Location = new System.Drawing.Point(0, 24);
            this.Panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(1300, 160);
            this.Panel1.TabIndex = 10;
            // 
            // pbRunning
            // 
            this.pbRunning.Image = ((System.Drawing.Image)(resources.GetObject("pbRunning.Image")));
            this.pbRunning.Location = new System.Drawing.Point(602, 29);
            this.pbRunning.Margin = new System.Windows.Forms.Padding(2);
            this.pbRunning.Name = "pbRunning";
            this.pbRunning.Size = new System.Drawing.Size(148, 98);
            this.pbRunning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRunning.TabIndex = 8;
            this.pbRunning.TabStop = false;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Location = new System.Drawing.Point(648, 130);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(64, 17);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "00:00:00";
            // 
            // btnAbort
            // 
            this.btnAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbort.Location = new System.Drawing.Point(434, 83);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(2);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(98, 31);
            this.btnAbort.TabIndex = 5;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(292, 83);
            this.btnPause.Margin = new System.Windows.Forms.Padding(2);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(98, 31);
            this.btnPause.TabIndex = 4;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(434, 36);
            this.btnRun.Margin = new System.Windows.Forms.Padding(2);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(98, 31);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // btnUnload
            // 
            this.btnUnload.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnload.Location = new System.Drawing.Point(292, 36);
            this.btnUnload.Margin = new System.Windows.Forms.Padding(2);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(98, 31);
            this.btnUnload.TabIndex = 2;
            this.btnUnload.Text = "Unload";
            this.btnUnload.UseVisualStyleBackColor = true;
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(13, 56);
            this.txtSN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(180, 22);
            this.txtSN.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(14, 32);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(107, 16);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Serial Number";
            // 
            // scLeftRight
            // 
            this.scLeftRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scLeftRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scLeftRight.Location = new System.Drawing.Point(0, 184);
            this.scLeftRight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.scLeftRight.Name = "scLeftRight";
            // 
            // scLeftRight.Panel1
            // 
            this.scLeftRight.Panel1.Controls.Add(this.TabControl1);
            // 
            // scLeftRight.Panel2
            // 
            this.scLeftRight.Panel2.Controls.Add(this.Label7);
            this.scLeftRight.Panel2.Controls.Add(this.txtData);
            this.scLeftRight.Size = new System.Drawing.Size(1300, 399);
            this.scLeftRight.SplitterDistance = 627;
            this.scLeftRight.TabIndex = 11;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabTPS);
            this.TabControl1.Controls.Add(this.tabGraph);
            this.TabControl1.Controls.Add(this.tabDut);
            this.TabControl1.Controls.Add(this.tabLens);
            this.TabControl1.Controls.Add(this.tabSummary);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(625, 397);
            this.TabControl1.TabIndex = 0;
            // 
            // tabTPS
            // 
            this.tabTPS.Controls.Add(this.dgvScript);
            this.tabTPS.Controls.Add(this.Label9);
            this.tabTPS.Location = new System.Drawing.Point(4, 22);
            this.tabTPS.Margin = new System.Windows.Forms.Padding(2);
            this.tabTPS.Name = "tabTPS";
            this.tabTPS.Padding = new System.Windows.Forms.Padding(2);
            this.tabTPS.Size = new System.Drawing.Size(617, 371);
            this.tabTPS.TabIndex = 0;
            this.tabTPS.Text = "TPS";
            this.tabTPS.UseVisualStyleBackColor = true;
            // 
            // dgvScript
            // 
            this.dgvScript.AllowUserToAddRows = false;
            this.dgvScript.AllowUserToDeleteRows = false;
            this.dgvScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvScript.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvScript.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScript.Location = new System.Drawing.Point(8, 22);
            this.dgvScript.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvScript.Name = "dgvScript";
            this.dgvScript.ReadOnly = true;
            this.dgvScript.Size = new System.Drawing.Size(606, 348);
            this.dgvScript.TabIndex = 0;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(5, 5);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(109, 16);
            this.Label9.TabIndex = 15;
            this.Label9.Text = "Process Steps";
            // 
            // tabGraph
            // 
            this.tabGraph.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.tabGraph.Location = new System.Drawing.Point(4, 22);
            this.tabGraph.Margin = new System.Windows.Forms.Padding(2);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(2);
            this.tabGraph.Size = new System.Drawing.Size(618, 409);
            this.tabGraph.TabIndex = 1;
            this.tabGraph.Text = "Graph";
            // 
            // tabDut
            // 
            this.tabDut.Controls.Add(this.dgvDUT);
            this.tabDut.Controls.Add(this.Label5);
            this.tabDut.Controls.Add(this.Label12);
            this.tabDut.Controls.Add(this.Label10);
            this.tabDut.Controls.Add(this.nudFailIndex);
            this.tabDut.Controls.Add(this.chkAutoLoad);
            this.tabDut.Controls.Add(this.nudPassIndex);
            this.tabDut.Location = new System.Drawing.Point(4, 22);
            this.tabDut.Margin = new System.Windows.Forms.Padding(2);
            this.tabDut.Name = "tabDut";
            this.tabDut.Size = new System.Drawing.Size(618, 409);
            this.tabDut.TabIndex = 2;
            this.tabDut.Text = "DUT";
            this.tabDut.UseVisualStyleBackColor = true;
            // 
            // dgvDUT
            // 
            this.dgvDUT.AllowUserToAddRows = false;
            this.dgvDUT.AllowUserToDeleteRows = false;
            this.dgvDUT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvDUT.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDUT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDUT.Location = new System.Drawing.Point(3, 30);
            this.dgvDUT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvDUT.Name = "dgvDUT";
            this.dgvDUT.ReadOnly = true;
            this.dgvDUT.Size = new System.Drawing.Size(615, 383);
            this.dgvDUT.TabIndex = 24;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(386, 7);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(52, 16);
            this.Label5.TabIndex = 32;
            this.Label5.Text = "Failed";
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(223, 7);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(100, 16);
            this.Label12.TabIndex = 30;
            this.Label12.Text = "Units Passed";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(5, 7);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(106, 16);
            this.Label10.TabIndex = 27;
            this.Label10.Text = "Package Tray";
            // 
            // nudFailIndex
            // 
            this.nudFailIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudFailIndex.Location = new System.Drawing.Point(445, 5);
            this.nudFailIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudFailIndex.Name = "nudFailIndex";
            this.nudFailIndex.Size = new System.Drawing.Size(52, 21);
            this.nudFailIndex.TabIndex = 31;
            // 
            // chkAutoLoad
            // 
            this.chkAutoLoad.AutoSize = true;
            this.chkAutoLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoLoad.Location = new System.Drawing.Point(118, 7);
            this.chkAutoLoad.Name = "chkAutoLoad";
            this.chkAutoLoad.Size = new System.Drawing.Size(97, 20);
            this.chkAutoLoad.TabIndex = 28;
            this.chkAutoLoad.Text = "Auto Load";
            this.chkAutoLoad.UseVisualStyleBackColor = true;
            // 
            // nudPassIndex
            // 
            this.nudPassIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudPassIndex.Location = new System.Drawing.Point(328, 5);
            this.nudPassIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nudPassIndex.Name = "nudPassIndex";
            this.nudPassIndex.Size = new System.Drawing.Size(52, 21);
            this.nudPassIndex.TabIndex = 29;
            // 
            // tabLens
            // 
            this.tabLens.Controls.Add(this.panelPartTray);
            this.tabLens.Controls.Add(this.Label8);
            this.tabLens.Location = new System.Drawing.Point(4, 22);
            this.tabLens.Margin = new System.Windows.Forms.Padding(2);
            this.tabLens.Name = "tabLens";
            this.tabLens.Size = new System.Drawing.Size(618, 409);
            this.tabLens.TabIndex = 3;
            this.tabLens.Text = "Lens";
            this.tabLens.UseVisualStyleBackColor = true;
            // 
            // panelPartTray
            // 
            this.panelPartTray.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelPartTray.BackColor = System.Drawing.SystemColors.Control;
            this.panelPartTray.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelPartTray.Location = new System.Drawing.Point(3, 26);
            this.panelPartTray.Name = "panelPartTray";
            this.panelPartTray.Size = new System.Drawing.Size(615, 387);
            this.panelPartTray.TabIndex = 22;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(3, 4);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(72, 16);
            this.Label8.TabIndex = 23;
            this.Label8.Text = "Part Tray";
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.Label6);
            this.tabSummary.Controls.Add(this.txtSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Margin = new System.Windows.Forms.Padding(2);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Size = new System.Drawing.Size(618, 409);
            this.tabSummary.TabIndex = 4;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(3, 6);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(109, 16);
            this.Label6.TabIndex = 14;
            this.Label6.Text = "Summary Data";
            // 
            // txtSummary
            // 
            this.txtSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSummary.Location = new System.Drawing.Point(3, 28);
            this.txtSummary.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSummary.Size = new System.Drawing.Size(615, 382);
            this.txtSummary.TabIndex = 1;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(5, 2);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(145, 16);
            this.Label7.TabIndex = 15;
            this.Label7.Text = "Process Information";
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.Location = new System.Drawing.Point(5, 26);
            this.txtData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData.Size = new System.Drawing.Size(648, 362);
            this.txtData.TabIndex = 0;
            // 
            // img
            // 
            this.img.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("img.ImageStream")));
            this.img.TransparentColor = System.Drawing.Color.Magenta;
            this.img.Images.SetKeyName(0, "Connect");
            this.img.Images.SetKeyName(1, "Disconn");
            this.img.Images.SetKeyName(2, "Stop");
            this.img.Images.SetKeyName(3, "Run");
            this.img.Images.SetKeyName(4, "Save");
            this.img.Images.SetKeyName(5, "Print");
            this.img.Images.SetKeyName(6, "Table");
            this.img.Images.SetKeyName(7, "Plot");
            this.img.Images.SetKeyName(8, "Message");
            this.img.Images.SetKeyName(9, "Script");
            this.img.Images.SetKeyName(10, "Cal Data");
            this.img.Images.SetKeyName(11, "Save Module Data");
            this.img.Images.SetKeyName(12, "Unload");
            this.img.Images.SetKeyName(13, "View Plot");
            this.img.Images.SetKeyName(14, "Data Folder");
            this.img.Images.SetKeyName(15, "Pause");
            this.img.Images.SetKeyName(16, "New Lot");
            this.img.Images.SetKeyName(17, "Controller");
            this.img.Images.SetKeyName(18, "Home All");
            this.img.Images.SetKeyName(19, "Camera View");
            this.img.Images.SetKeyName(20, "BG Cal");
            // 
            // fFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 605);
            this.Controls.Add(this.scLeftRight);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.sbr);
            this.Name = "fFunction";
            this.Text = "fFunction";
            this.sbr.ResumeLayout(false);
            this.sbr.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRunning)).EndInit();
            this.scLeftRight.Panel1.ResumeLayout(false);
            this.scLeftRight.Panel2.ResumeLayout(false);
            this.scLeftRight.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scLeftRight)).EndInit();
            this.scLeftRight.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.tabTPS.ResumeLayout(false);
            this.tabTPS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScript)).EndInit();
            this.tabDut.ResumeLayout(false);
            this.tabDut.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDUT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFailIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPassIndex)).EndInit();
            this.tabLens.ResumeLayout(false);
            this.tabLens.PerformLayout();
            this.tabSummary.ResumeLayout(false);
            this.tabSummary.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip menu;
        internal System.Windows.Forms.StatusStrip sbr;
        internal System.Windows.Forms.ToolStripStatusLabel lblStatus;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.PictureBox pbRunning;
        internal System.Windows.Forms.Label lblTime;
        internal System.Windows.Forms.Button btnAbort;
        internal System.Windows.Forms.Button btnPause;
        internal System.Windows.Forms.Button btnRun;
        internal System.Windows.Forms.Button btnUnload;
        internal System.Windows.Forms.TextBox txtSN;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.SplitContainer scLeftRight;
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage tabTPS;
        internal System.Windows.Forms.DataGridView dgvScript;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.TabPage tabGraph;
        internal System.Windows.Forms.TabPage tabDut;
        internal System.Windows.Forms.DataGridView dgvDUT;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.NumericUpDown nudFailIndex;
        internal System.Windows.Forms.CheckBox chkAutoLoad;
        internal System.Windows.Forms.NumericUpDown nudPassIndex;
        internal System.Windows.Forms.TabPage tabLens;
        internal System.Windows.Forms.Panel panelPartTray;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.TabPage tabSummary;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.TextBox txtSummary;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox txtData;
        internal System.Windows.Forms.ImageList img;
    }
}
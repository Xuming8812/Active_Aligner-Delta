namespace Delta
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.systemItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.accountItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.motionItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.daqItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.handlingItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.processItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.visionItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.deviceItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.helpItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabMainForm = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabMainForm.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemItems,
            this.accountItems,
            this.motionItems,
            this.daqItems,
            this.handlingItems,
            this.processItems,
            this.visionItems,
            this.deviceItems,
            this.helpItems});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1718, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // systemItems
            // 
            this.systemItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.systemItems.Image = ((System.Drawing.Image)(resources.GetObject("systemItems.Image")));
            this.systemItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.systemItems.Name = "systemItems";
            this.systemItems.Size = new System.Drawing.Size(62, 22);
            this.systemItems.Text = "System";
            // 
            // accountItems
            // 
            this.accountItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.accountItems.Image = ((System.Drawing.Image)(resources.GetObject("accountItems.Image")));
            this.accountItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.accountItems.Name = "accountItems";
            this.accountItems.Size = new System.Drawing.Size(67, 22);
            this.accountItems.Text = "Account";
            // 
            // motionItems
            // 
            this.motionItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.motionItems.Image = ((System.Drawing.Image)(resources.GetObject("motionItems.Image")));
            this.motionItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.motionItems.Name = "motionItems";
            this.motionItems.Size = new System.Drawing.Size(63, 22);
            this.motionItems.Text = "Motion";
            // 
            // daqItems
            // 
            this.daqItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.daqItems.Image = ((System.Drawing.Image)(resources.GetObject("daqItems.Image")));
            this.daqItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.daqItems.Name = "daqItems";
            this.daqItems.Size = new System.Drawing.Size(48, 22);
            this.daqItems.Text = "DAQ";
            // 
            // handlingItems
            // 
            this.handlingItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.handlingItems.Image = ((System.Drawing.Image)(resources.GetObject("handlingItems.Image")));
            this.handlingItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.handlingItems.Name = "handlingItems";
            this.handlingItems.Size = new System.Drawing.Size(73, 22);
            this.handlingItems.Text = "Handling";
            // 
            // processItems
            // 
            this.processItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.processItems.Image = ((System.Drawing.Image)(resources.GetObject("processItems.Image")));
            this.processItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.processItems.Name = "processItems";
            this.processItems.Size = new System.Drawing.Size(66, 22);
            this.processItems.Text = "Process";
            // 
            // visionItems
            // 
            this.visionItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.visionItems.Image = ((System.Drawing.Image)(resources.GetObject("visionItems.Image")));
            this.visionItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.visionItems.Name = "visionItems";
            this.visionItems.Size = new System.Drawing.Size(56, 22);
            this.visionItems.Text = "Vision";
            // 
            // deviceItems
            // 
            this.deviceItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deviceItems.Image = ((System.Drawing.Image)(resources.GetObject("deviceItems.Image")));
            this.deviceItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deviceItems.Name = "deviceItems";
            this.deviceItems.Size = new System.Drawing.Size(59, 22);
            this.deviceItems.Text = "Device";
            // 
            // helpItems
            // 
            this.helpItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpItems.Image = ((System.Drawing.Image)(resources.GetObject("helpItems.Image")));
            this.helpItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpItems.Name = "helpItems";
            this.helpItems.Size = new System.Drawing.Size(48, 22);
            this.helpItems.Text = "Help";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 744);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1718, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(131, 17);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // tabMainForm
            // 
            this.tabMainForm.Controls.Add(this.tabPage1);
            this.tabMainForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMainForm.Location = new System.Drawing.Point(0, 25);
            this.tabMainForm.Name = "tabMainForm";
            this.tabMainForm.SelectedIndex = 0;
            this.tabMainForm.Size = new System.Drawing.Size(1718, 719);
            this.tabMainForm.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1710, 689);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log Editor";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1704, 683);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1718, 766);
            this.Controls.Add(this.tabMainForm);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabMainForm.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton systemItems;
        private System.Windows.Forms.ToolStripDropDownButton accountItems;
        private System.Windows.Forms.ToolStripDropDownButton motionItems;
        private System.Windows.Forms.ToolStripDropDownButton daqItems;
        private System.Windows.Forms.ToolStripDropDownButton handlingItems;
        private System.Windows.Forms.ToolStripDropDownButton visionItems;
        private System.Windows.Forms.ToolStripDropDownButton deviceItems;
        private System.Windows.Forms.ToolStripDropDownButton helpItems;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripDropDownButton processItems;
        private System.Windows.Forms.TabControl tabMainForm;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}
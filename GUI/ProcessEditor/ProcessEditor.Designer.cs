namespace Delta
{
    partial class ProcessEditor
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
            this.panelForm = new System.Windows.Forms.Panel();
            this.menuSelectScript = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panelForm.Location = new System.Drawing.Point(424, 12);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(581, 529);
            this.panelForm.TabIndex = 0;
            // 
            // menuSelectScript
            // 
            this.menuSelectScript.Name = "contextMenuStrip1";
            this.menuSelectScript.Size = new System.Drawing.Size(153, 26);
            // 
            // ProcessEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 625);
            this.Controls.Add(this.panelForm);
            this.Name = "ProcessEditor";
            this.Text = "ProcessEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelForm;
        private System.Windows.Forms.ContextMenuStrip menuSelectScript;
    }
}
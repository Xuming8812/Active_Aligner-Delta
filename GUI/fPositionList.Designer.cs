namespace Delta
{
    partial class PositionList
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
            this.btnClose = new System.Windows.Forms.Button();
            this.treePositionList = new System.Windows.Forms.TreeView();
            this.tbLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Location = new System.Drawing.Point(659, 549);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(73, 68);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // treePositionList
            // 
            this.treePositionList.Location = new System.Drawing.Point(12, 29);
            this.treePositionList.Name = "treePositionList";
            this.treePositionList.Size = new System.Drawing.Size(266, 282);
            this.treePositionList.TabIndex = 2;
            this.treePositionList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treePositionList_NodeMouseClick);
            // 
            // tbLayout
            // 
            this.tbLayout.ColumnCount = 2;
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tbLayout.Location = new System.Drawing.Point(285, 45);
            this.tbLayout.Name = "tbLayout";
            this.tbLayout.RowCount = 2;
            this.tbLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tbLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tbLayout.Size = new System.Drawing.Size(200, 100);
            this.tbLayout.TabIndex = 3;
            // 
            // PositionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 629);
            this.Controls.Add(this.tbLayout);
            this.Controls.Add(this.treePositionList);
            this.Controls.Add(this.btnClose);
            this.Name = "PositionList";
            this.Text = "PositionList";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TreeView treePositionList;
        private System.Windows.Forms.TableLayoutPanel tbLayout;
    }
}
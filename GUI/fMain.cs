using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Delta
{
    public partial class fMain : Form
    {

        private w2.w2IniFile mIniFile;
        private fFunction mFunction;

        public fMain()
        {
            InitializeComponent();
        }

        public bool Initialize(string sIniFile)
        {
            //pass down
            mIniFile = new w2.w2IniFile(sIniFile);

            //GUI
            this.Text = "Delta | Rev " + Application.ProductVersion;
            this.Cursor = Cursors.WaitCursor;
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.SetupGUI();
            this.Show();
            Application.DoEvents();

            //spwan function panel
            this.SpawnFunctionPanel();

            //done
            this.Cursor = Cursors.Default;
            this.Refresh();

            return true;
        }

        private void SpawnFunctionPanel()
        {
            //start function form
            mFunction = new fFunction();

            //put it to the panel
            pFunction.Width = mIniFile.ReadParameter("GUI", "PanelWidth", pFunction.Width);
            mFunction.TopLevel = false;
            mFunction.FormBorderStyle = FormBorderStyle.None;

            int h = mFunction.scLeftRight.SplitterDistance;
            pFunction.Controls.Add(mFunction);
            mFunction.Dock = DockStyle.Fill;
            mFunction.scLeftRight.SplitterDistance = h;
            mFunction.Focus();

            //do this last, this may be slow, but we at least have GUI setup
            mFunction.Initialize(this, ref mIniFile);
        }

        private void SetupGUI()
        {


            pFunction.Width = mIniFile.ReadParameter("GUI", "PanelWidth", pFunction.Width);

            //update screen
            this.Refresh();
            Application.DoEvents();
        }

        #region "callback"

        private void fMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            //check if we can close
            if (mFunction != null)
            {
                if (mFunction.ScriptBusy())
                {
                    MessageBox.Show("Please stop the script before closing the windown", this.Text, MessageBoxButtons.OK);
                    e.Cancel = true;
                }
                mFunction.Close();
            }
        }

        private void fMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:

                    break;
                case Keys.F2:

                    break;
            }
        }

        private void fMain_Resize(object sender, System.EventArgs e)
        {
            int width = 0;

            if (!this.Visible)
                return;

            width = mIniFile.ReadParameter("GUI", "PanelWidth", 0);

            if (this.Width < pFunction.Width)
            {
                pFunction.Width = 4 * this.Width / 5;
            }
            else if (this.Width > pFunction.Width)
            {
                if (width > 0)
                    pFunction.Width = width;
            }
        }

        private void Splitter1_SplitterMoved(System.Object sender, System.Windows.Forms.SplitterEventArgs e)
        {
            pFunction.Width = e.X;
            mIniFile.WriteParameter("GUI", "PanelWidth", pFunction.Width.ToString());
        }

        #endregion
    }
}

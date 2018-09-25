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
    public partial class MainForm : Form
    {
        #region "Declaration"

        private struct ProcessStatus
        {
            public bool Running;
            public bool Stop;
            public bool Sync;
            public bool FailedScript;
        }

        private ProcessStatus mProcess;

        private enum UIPanelEnum
        {
            PartTray,
            MotionControl,
            VisionPanel,
            DAQ,
            PositionList,
            Configuration,
            Database,
        }

        private Form[] mUiPanels = new Form[Enum.GetNames(typeof(UIPanelEnum)).Length];
        private TabPage[] mUiTabPages = new TabPage[Enum.GetNames(typeof(UIPanelEnum)).Length];

        private w2.w2IniFile mIniFile;
        private w2.w2IniFileXML mParaFile;
        private w2.w2TextMessage mMsgInfoHost;
        private w2.w2MessageService mMsgInfo;

        private iStage StageBase;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        public bool Initialize(string sIniFile)
        {
            string s;

            this.SetupGUI();
            this.SetupMenu();

            mIniFile = new w2.w2IniFile(sIniFile);
            s = mIniFile.ReadParameter("Files", "ParameterFile", "");
            s = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,s);

            try
            {
                mParaFile = new w2.w2IniFileXML(s);
            }
            catch
            {
                return false;
            }


            this.Cursor = Cursors.Arrow;

            return true;
        }

        #region "Setup GUI"
        public void SetupGUI()
        {

            // set MainForm properties
            this.Text = "Delta | Rev " + Application.ProductVersion;
            this.Cursor = Cursors.WaitCursor;
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;

            //Set TabControl
            tabMainForm.Dock = DockStyle.Fill;
            tabPage1.Parent = null;

            
        }

        public void SetupMenu()
        {
            systemItems.DropDownItems.Add("Exit",null,Menu_ItemClicked).Name = "Exit";

            motionItems.DropDownItems.Add("Jog Panel", null, Menu_ItemClicked).Name = "Jog Panel";
            motionItems.DropDownItems.Add("PositionList", null, Menu_ItemClicked).Name = "PositionList";

            processItems.DropDownItems.Add("Configuration", null, Menu_ItemClicked).Name = "Configuration";
            processItems.DropDownItems.Add("Log File", null, Menu_ItemClicked).Name = "Log File";

        }
        #endregion

        #region "Setup for UI Panels"

        private void SpawnPanel(UIPanelEnum PanelIndex)
        {
            Form f;
            TabPage p;

            mProcess.Sync = true;

            f = mUiPanels[(int)PanelIndex];
            p = mUiTabPages[(int)PanelIndex];

            if(f!= null)
            {
                tabMainForm.SelectedTab = p;
                return;
            }

            switch (PanelIndex)
            {
                case UIPanelEnum.Configuration:
                    break;
                case UIPanelEnum.DAQ:
                    break;
                case UIPanelEnum.Database:
                    break;
                case UIPanelEnum.MotionControl:
                    break;
                case UIPanelEnum.PartTray:

                    break;

                case UIPanelEnum.PositionList:

                    break;

                case UIPanelEnum.VisionPanel:

                    break;
            }

            p = new TabPage(f.Name);

            f.FormBorderStyle = FormBorderStyle.None;
            f.TopLevel = false;
            f.Parent = p;

            f.FormClosed += UI_FormClosed;

            tabMainForm.TabPages.Add(p);
            tabMainForm.SelectedTab = p;

            mUiPanels[(int)PanelIndex] = f;
            mUiTabPages[(int)PanelIndex] = p;

            f.Left = (p.Width - f.Width) / 2;
            f.Height = (p.Height - f.Height) / 2;
            f.Show();

        }

        #endregion

        #region "Callback"
        private void Menu_ItemClicked(System.Object sender, EventArgs e)
        {
            ToolStripMenuItem menu;
            menu = (ToolStripMenuItem)sender;

            switch (menu.Name)
            {
                case "Exit":
                    this.Close();
                    break;

            }
        }

        private void UI_FormClosed(System.Object sender, EventArgs e)
        {
            Form f;
            f = (Form)sender;

            int index = 0;

            switch (f.Name)
            {
                case "JogPanel":
                    index = (int)UIPanelEnum.MotionControl;
                    break;

            }

            tabMainForm.TabPages.Remove(mUiTabPages[index]);
            mUiTabPages[index] = null;
            mUiPanels[index] = null;

        }

        #endregion



    }
}

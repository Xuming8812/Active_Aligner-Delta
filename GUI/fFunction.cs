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
    public partial class fFunction : Form
    {
        private struct ProcessStatus
        {
            public bool Running;
            public bool Stop;
            public bool Sync;
            public bool AutoLoad;
            public int PartIndex;
            public string FailedScript;
        }
        private ProcessStatus mProcess;


        private fMain mParent;
        private w2.w2IniFile mIniFile;

        private w2.w2IniFileXML mParaFile;

        private w2.w2ScriptEx mScript;
        private w2.w2TextMessage mMsgInfoHost;
        private w2.w2MessageService mMsgInfo;

        private DeltaData mData;
        private DeltaFunction mTool;
        private string mStation;

        public fFunction()
        {
            InitializeComponent();
        }

        public bool Initialize(fMain hParent, ref w2.w2IniFile hIniFile)
        {
            string[] sArgs;
            string s;

            mParent = hParent;
            mIniFile = hIniFile;

            mMsgInfoHost = new w2.w2TextMessage(txtData, lblStatus);
            mMsgInfoHost.Editable = true;
            mMsgInfo = mMsgInfoHost.MessageService;


            return true;

        }

        public void SetScriptNameToTitle(string sScript)
        {
            string s = null;
            int p = 0;

            const string Key = " | Script - ";

            s = mParent.Text;
            p = s.IndexOf(Key);
            if (p > 0)
                s = s.Substring(0, p);

            mParent.Text = s + Key + sScript;
        }

        public bool ScriptBusy()
        {
            return mProcess.Running;
        }

        #region "Callback"
        private void fFunction_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (mTool != null)
            {
                mTool.Instruments.CloseAll();
            }
        }

        private void fFunction_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (mProcess.Running)
            {
                MessageBox.Show("Please stop the script before closing the windown", this.Text, MessageBoxButtons.OK);
                e.Cancel = true;
            }
        }
        #endregion

    }
}

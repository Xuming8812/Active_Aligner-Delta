using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Delta
{
     public partial class DeltaFunction
    {
        private fMain mParent;

        //private fImage mFormImage;
        private w2.w2IniFile mIniFile;

        private w2.w2IniFileXML mParaFile;
        private w2.w2MessageService mMsgInfo;

        private w2.w2MessageService mMsgData;
        private DeltaData mData;
        private DeltaParameters mPara;

        public DeltaParameters Parameter
        {
            get { return mPara; }
        }

        public bool Initialize(fMain hParent, ref w2.w2IniFile hIniFile, ref w2.w2IniFileXML hParaFile, ref DeltaData hDataTool, w2.w2MessageService hMsgInfo, ref w2.w2MessageService hMsgData)
        {
            bool success = false;

            mParent = hParent;

            mData = hDataTool;
            mIniFile = hIniFile;
            mParaFile = hParaFile;

            mMsgInfo = hMsgInfo;
            mMsgData = hMsgData;

            //get parameter - do this before instrument
            mMsgInfo.PostMessage("Read parameters from file ... ");
            mPara = new DeltaParameters(mParaFile);

            //instrument
            mMsgInfo.PostMessage("");
            mMsgInfo.PostMessage("Initialize instrument ... ");
            mInst = new InstrumentList(this);
            success = mInst.InitializeAll();
            if (!success)
                return false;

            //start tool class
            mStage = new StageFunctions(this);
            mUtility = new InstrumentUtility(this);

            return true;
        }

        #region "Stop"
        private bool mStop;
        public void Stop()
        {
            mStop = true;
        }

        public void ClearStop()
        {
            mStop = false;
        }

        protected bool CheckStop()
        {
            Application.DoEvents();
            if (mStop)
            {
                mMsgInfo.PostMessage("   Stopping process ... ");
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public bool WaitForTime(double WaitTimeInSecond, string Message)
        {
            System.DateTime T = default(System.DateTime);
            double seconds = 0;
            string s = null;

            seconds = 0.0;
            T = System.DateTime.Now;
            while (seconds < WaitTimeInSecond)
            {
                System.Threading.Thread.Sleep(100);
                if (this.CheckStop())
                    return false;

                seconds = System.DateTime.Now.Subtract(T).TotalSeconds;

                s = Message + " " + seconds.ToString("0.0") + " of " + WaitTimeInSecond.ToString("0.0") + " sec";
                mMsgInfo.PostMessage(s, w2.w2MessageService.MessageDisplayStyle.NewStatus);
            }

            return true;
        }

        public DialogResult ShowMessageBox(string Text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            DialogResult r = default(DialogResult);
            mInst.XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Fail);
            r = MessageBox.Show(Text, caption, buttons, icon, defaultButton);
            mInst.XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Running);

            return r;
        }
    }
}

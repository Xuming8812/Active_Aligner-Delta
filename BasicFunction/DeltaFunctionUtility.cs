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
        private InstrumentUtility mUtility;
        public InstrumentUtility Utility
        {
            get { return mUtility; }
        }

        public class InstrumentUtility
        {
            private DeltaFunction mTool;
            private DeltaParameters mPara;
            private InstrumentList mInst;
            private w2.w2MessageService mMsgData;
            private w2.w2MessageService mMsgInfo;
            //Add By Ming
            private DeltaData mData;

            private Instrument.iXPS mXPS;
            private Instrument.iXpsIO mIO;
            private StageFunctions mStage;

            public InstrumentUtility(DeltaFunction hTool)
            {
                mTool = hTool;
                mInst = hTool.mInst;
                mPara = hTool.mPara;
                mMsgData = hTool.mMsgData;
                mMsgInfo = hTool.mMsgInfo;

                //   Add By Ming
                mData = hTool.mData;

                mIO = mInst.XpsIO;
                mStage = hTool.mStage;
            }

            #region "probe clamp"
            public bool IsProbeClampOpen()
            {
                return (mInst.ProbeClamp.Position < mPara.ProbeClamp.OpenPosition);
            }

            public bool CloseProbeClamp(bool Close)
            {
                string s = null;
                bool success = false;
                double elapse = 0;
                System.DateTime tStart = default(System.DateTime);

                s = (Close ? "Close" : "Open").ToString();
                s = "    " + s + " clamp for the probe pin ...";
                mMsgInfo.PostMessage(s);

                mInst.ProbeClamp.Enabled = true;
                if (Close)
                {
                    success = mInst.ProbeClamp.CloseGrip(mPara.ProbeClamp.Speed, mPara.ProbeClamp.Force);
                }
                else
                {
                    success = mInst.ProbeClamp.OpenGrip(mPara.ProbeClamp.Speed, mPara.ProbeClamp.Force);
                }

                if (!success)
                {
                    s = "X   Failed to open/close probe clamp. Error Code =  0x" + Convert.ToString(mInst.ProbeClamp.ErrorCode, 16);
                    mMsgInfo.PostMessage(s);
                    return false;
                }

                //wait
                tStart = System.DateTime.Now;
                s = "Wait for clamp to open/close, elapse {1:0.00} second";

                while (mInst.ProbeClamp.ClampBusy)
                {
                    if (mTool.CheckStop())
                    {
                        mInst.ProbeClamp.StopMotion();
                        return false;
                    }

                    //relax and show data
                    System.Threading.Thread.Sleep(100);
                    elapse = System.DateTime.Now.Subtract(tStart).TotalSeconds;
                    mMsgData.PostMessage(string.Format(s, elapse), w2.w2MessageService.MessageDisplayStyle.NewStatus);
                }

                return true;
            }

            public bool MoveProbeClamp(bool IsRelativeMove, double Target)
            {
                string s = null;
                bool success = false;
                double elapse = 0;
                System.DateTime tStart = default(System.DateTime);

                s = (IsRelativeMove ? "Move Clamp by " : "Move Clamp to ").ToString();
                s = "    " + s + Target.ToString("0.00") + "  ...";
                mMsgInfo.PostMessage(s);

                mInst.ProbeClamp.Enabled = true;
                if (IsRelativeMove)
                {
                    if (Target > 0)
                    {
                        success = mInst.ProbeClamp.MoveRelative(Target, mPara.ProbeClamp.Speed);
                    }
                    else
                    {
                        double pos = mInst.ProbeClamp.Position;
                        success = mInst.ProbeClamp.Move(pos + Target, mPara.ProbeClamp.Speed);
                    }

                }
                else
                {
                    success = mInst.ProbeClamp.Move(Target, mPara.ProbeClamp.Speed);
                }

                if (!success)
                {
                    s = "X   Failed to move probe clamp. Error Code =  0x" + Convert.ToString(mInst.ProbeClamp.ErrorCode, 16);
                    mMsgInfo.PostMessage(s);
                    return false;
                }

                //wait
                tStart = System.DateTime.Now;
                s = "Wait for clamp to move, elapse {1:0.00} second";

                while (mInst.ProbeClamp.ClampBusy)
                {
                    if (mTool.CheckStop())
                    {
                        mInst.ProbeClamp.StopMotion();
                        return false;
                    }

                    //relax and show data
                    System.Threading.Thread.Sleep(100);
                    elapse = System.DateTime.Now.Subtract(tStart).TotalSeconds;
                    mMsgData.PostMessage(string.Format(s, elapse), w2.w2MessageService.MessageDisplayStyle.NewStatus);
                }

                return true;
            }

            #endregion

            #region "Force Guage"
            public double GetForceGuageReading(iStage.StageEnum eStage, int Samples)
            {
                double v = 0;
                int i = 0;
                bool flag = false;
                double value = 0;

                Instrument.iOmegaDP40 x = default(Instrument.iOmegaDP40);

                switch (eStage)
                {
                    case iStage.StageEnum.Hexapod:
                        x = mInst.ForceGaugeHexapod;
                        break;
                    case iStage.StageEnum.Main:
                        x = mInst.ForceGaugeMain;
                        break;
                    default:
                        return double.NaN;
                }

                if (x != null)
                {
                    v = 0;

                    flag = true;

                    for (i = 1; i <= Samples; i++)
                    {
                        //While (flag)
                        //    value = x.ReadProcessValue()
                        //    'If value > 100 Then
                        //    '    flag = False
                        //    'End If
                        //    flag = False
                        //End While
                        value = x.ReadProcessValue();
                        v += value;
                        flag = true;
                    }
                    v /= Samples;
                }
                else
                {
                    return double.NaN;
                }

                return v;
            }
            #endregion

            #region "UV Tool"
            public bool RunUvCure()
            {
                bool HaveCtrl = false;
                DialogResult response = default(DialogResult);
                string s = null;
                string fmt = null;

                fmt = "Wait UV exposure to finish in {0:0.00} sec, Elapse {1:0.00} sec";

                mMsgInfo.PostMessage("    Start UV Cure process ...");

                //check if we have UV control
                HaveCtrl = (mInst.UvLamp != null);

                if (HaveCtrl)
                {
                    //check error
                    s = "";
                    if (mInst.UvLamp.AlarmOn)
                        s += "Alarm on, ";
                    if (!mInst.UvLamp.LampOn)
                        s += "Lamp off, ";
                    if (!mInst.UvLamp.LampReady)
                        s += "Lamp not ready, ";
                    if (mInst.UvLamp.ShutterOpen)
                        s += "Shutter open and exposure running, ";
                    //If mInst.UvLamp.NeedCalibration Then s += "Need calibration"

                    if (!string.IsNullOrEmpty(s))
                    {
                        s = "X   UV cure tool is not ready: " + s;
                        mMsgInfo.PostMessage(s);
                        return false;
                    }

                    //start async run
                    mInst.UvLamp.RunExposure(false);
                    mMsgInfo.PostMessage("      Start exposure for " + mInst.UvLamp.ExposureTimeExpected.ToString("0.00") + " sec");

                    //wait

                    //loop
                    while (mInst.UvLamp.ExposureInProgress)
                    {
                        //check stop
                        Application.DoEvents();
                        if (mTool.mStop)
                        {
                            mMsgInfo.PostMessage("      Stopping UV exposure ...");
                            mInst.UvLamp.StopUvExposure();
                        }
                        //wait
                        System.Threading.Thread.Sleep(100);
                        //display
                        s = string.Format(fmt, mInst.UvLamp.ExposureTimeExpected, mInst.UvLamp.ExposureTimePassed);
                        mMsgInfo.PostMessage(s, w2.w2MessageService.MessageDisplayStyle.NewStatus);
                    }

                    //done
                    if (mTool.mStop)
                    {
                        s = " Stopped!";
                    }
                    else
                    {
                        s = " Done!";
                    }

                    mMsgInfo.PostMessage(s, w2.w2MessageService.MessageDisplayStyle.ContinuingStatus);
                    return (!mTool.mStop);

                }
                else
                {
                    mMsgInfo.PostMessage("      No controller found. Proceed the process manually.");

                    s = "Please have the UV cure tool ready.";
                    response = MessageBox.Show(s, "UV Cure", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (response == DialogResult.Cancel)
                        return false;
                    mMsgInfo.PostMessage("      Tool ready confirmed by operator");


                    s = "Please proceed with the UV cure. Clock OK when it is done.";
                    response = MessageBox.Show(s, "UV Cure", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (response == DialogResult.Cancel)
                        return false;
                    mMsgInfo.PostMessage("      Cure finished and confirmed by operator");

                    return true;
                }
            }


            #endregion

        }
    }
}

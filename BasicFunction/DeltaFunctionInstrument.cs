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
        private InstrumentList mInst;
        public InstrumentList Instruments
        {
            get { return mInst; }
        }

        public class InstrumentList
        {
            private const string Text = "Instrument";
            //tool
            private w2.w2IniFile mIniFile;
            private w2.w2IniFileXML mParaFile;

            private w2.w2MessageService mMsg;

            private DeltaParameters mPara;
            //------------------------------------------ -Stage
            public Instrument.iXPS XPS;
            public Instrument.iPiGCS843 PiAngle;
            public Instrument.iPiGCS Hexopod;

            public Instrument.iPiLS65 PiLS;
            public Instrument.iXpsIO XpsIO;

            public iStage StageBase;


            //-------------------------------------------Other

            public Instrument.iTaiyoClamp ProbeClamp;
            //changed by Ming to run different channels

            public Instrument.iUvCure UvLamp;

            public Instrument.iBeamProfiler NanoScan;

            public Instrument.iMultiChannelLDD LDD;

            public Instrument.iOmegaDP40 ForceGaugeMain;

            public Instrument.iOmegaDP40 ForceGaugeHexapod;

            public InstrumentList(DeltaFunction hTool)
            {
                mMsg = hTool.mMsgInfo;
                mPara = hTool.mPara;
                mIniFile = hTool.mIniFile;
                mParaFile = hTool.mParaFile;
            }

            public bool InitializeAll()
            {
                bool success = false;

                success = this.InitializeXPS();
                if (!success)
                    return false;

                success = this.InitializePiAngle();
                if (!success)
                    return false;

                success = this.InitializePiHexopod();
                if (!success)
                    return false;

                success = this.InitializePiLS();
                if (!success)
                    return false;

                //initialize sub classes
                mMsg.PostMessage("");
                mMsg.PostMessage("Configuring the XPS IO interface... ");
                XpsIO = new Instrument.iXpsIO();
                success = XpsIO.Initialize(ref XPS, ref mParaFile);
                if (!success)
                    return false;

                mMsg.PostMessage("");
                mMsg.PostMessage("Configuring the stage info ... ");
                StageBase = new iStage();
                success = StageBase.Initialize(ref mParaFile, ref XPS, ref PiAngle, ref Hexopod, ref PiLS);
                if (!success)
                    return false;

                //other standard instrument
                success = this.InitializeClampForProbe();
                if (!success)
                    return false;

                success = this.InitializeForceGauge();
                if (!success)
                    return false;

                success = this.InitializeLDD();
                if (!success)
                    return false;

                success = this.InitializeNanoScan();
                if (!success)
                    return false;

                success = this.InitializeUvLamp();
                if (!success)
                    return false;

                return success;

            }

            public bool CloseAll()
            {
                int i = 0;

                if (NanoScan != null)
                    NanoScan.Close();
                if (UvLamp != null)
                    UvLamp.Close();
                if (XPS != null)
                    XPS.Close();
                if (LDD != null)
                {
                    for (i = 1; i <= LDD.ChannelCount; i++)
                    {
                        LDD.SetCurrent(i, 0);
                    }
                }
                if (LDD != null)
                    LDD.EnabledProtectionState = true;

                return true;
            }
            #region "individual device"

            private bool InitializeClampForProbe()
            {
                //string s = null;
                string Adrs = null;
                bool success = false;

                Adrs = mIniFile.ReadParameter("Instrument", "AdrsTaiyo", "");
                if (!Adrs.StartsWith("COM"))
                {
                    mMsg.PostMessage("    Configured for not using Taiyo Clamp for the probe pin");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing Taiyo Clamp for the probe pin " + Adrs + " ... ");

                //start
                ProbeClamp = new Instrument.iTaiyoClamp(0);
                success = ProbeClamp.Initialize(Adrs, true);
                if (success)
                {
                    ProbeClamp.Enabled = true;
                }
                else
                {
                    ProbeClamp = null;
                }

                return (ProbeClamp != null);
            }

            private bool InitializeForceGauge()
            {
                string s = null;
                string adrs = null;
                int rate = 0;

                //get baud rate
                rate = mIniFile.ReadParameter("Instrument", "ForceGaugeBaudRate", 9600);

                //force gauge for lens
                adrs = mIniFile.ReadParameter("Instrument", "AdrsForceGaugeMain", "-1");
                if (adrs.StartsWith("COM"))
                {
                    mMsg.PostMessage("    Initializing force gauge for main tool at port " + adrs + " ... ");

                    ForceGaugeMain = new Instrument.iOmegaDP40();
                    if (!ForceGaugeMain.Initialize(adrs, rate, true))
                    {
                        s = "Failed to initialize force gauge controller at COM port " + adrs + " for lens vacuum tip. Application will be terminated.";
                        MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ForceGaugeMain = null;
                        return false;
                    }
                }
                else
                {
                    mMsg.PostMessage("    Configured for not useing use force gauge for lens vacuum tip");
                }

                //force gauage for BS
                adrs = mIniFile.ReadParameter("Instrument", "AdrsForceGaugeHexapod", "-1");
                if (adrs.StartsWith("COM"))
                {
                    mMsg.PostMessage("    Initializing force gauge for beam splitter vacuum tip at port " + adrs + " ... ");

                    ForceGaugeHexapod = new Instrument.iOmegaDP40();
                    if (!ForceGaugeHexapod.Initialize(adrs, rate, true))
                    {
                        s = "Failed to initialize force gauge controller at COM port " + adrs + " for beam splitter vacuum tip. Application will be terminated.";
                        MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ForceGaugeHexapod = null;
                        return false;
                    }
                }
                else
                {
                    mMsg.PostMessage("    Configured for not useing force gauge for beam splitter vacuum tip");
                }

                return true;
            }

            private bool InitializeLDD()
            {

                string Adrs = null;
                bool success = false;

                Adrs = mIniFile.ReadParameter("Instrument", "AdrsLDD", "");
                if (!Adrs.StartsWith("COM"))
                {
                    mMsg.PostMessage("    Configured for not using OC9501 Driver Board");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing OC9501 driver board at " + Adrs + " ... ");

                //start

                LDD = new Instrument.iOC9501LDD(4, mPara.LaserDiode.DefaultCurrent);
                success = LDD.Initialize(Adrs, true);

                if (success)
                {
                }
                else
                {
                    LDD = null;
                }

                return (LDD != null);
            }

            private bool InitializeNanoScan()
            {
                string s = null;
                int adrs = 0;
                bool success = false;

                adrs = mIniFile.ReadParameter("Instrument", "AdrsBeamScan", -1);
                if (adrs < 0)
                {
                    mMsg.PostMessage("    Configured for not using beam scan");
                    return true;
                }

                s = mIniFile.ReadParameter("Instrument", "TypeBeamScan", "");
                switch (s)
                {
                    case "Newport":
                        mMsg.PostMessage("    Initializing NanoScan service. This may take a few minues ... ");

                        NanoScan = new Instrument.iNewportNanoScan();
                        success = NanoScan.Initialize("");
                        if (!success)
                        {
                            s = "Failed to initialize Nano Scan. Application will be closed.";
                            MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        break;
                    case "Thorlabs":
                        mMsg.PostMessage("    Initializing Thorlabs beam scan");

                        NanoScan = new Instrument.iThorlabsBP200BeamScan();
                        success = NanoScan.Initialize(adrs.ToString());
                        if (!success)
                        {
                            s = "Failed to initialize Nano Scan. Application will be closed.";
                            MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        break;
                    default:
                        mMsg.PostMessage("    Beam scan type: " + s + " is not supported!");
                        return false;
                }

                // If Not NanoScan.AutoGain Then NanoScan.Gain = mPara.BeamScan.Gain
                NanoScan.SamplingFrequency = mPara.BeamScan.SampleFrequency;
                NanoScan.SamplingResolution = mPara.BeamScan.SampleResolution;

                //Dim x As Instrument.iBeamProfiler.SimpleData
                //x = NanoScan.AcquireData(5, Instrument.iBeamProfiler.DataAcquisitionMode.PeakWidthEnergy)

                return true;
            }

            private bool InitializeUvLamp()
            {
                string s = null;
                string Adrs = null;
                bool success = false;
                DialogResult r = default(DialogResult);

                Adrs = mIniFile.ReadParameter("Instrument", "AdrsUvLamp", "");
                if (!Adrs.StartsWith("COM"))
                {
                    mMsg.PostMessage("    Configured for not using UV Lamp");
                    return true;
                }

                s = mIniFile.ReadParameter("Instrument", "TypeUvLamp", "");
                switch (s)
                {
                    case "FUWO":
                        //ack
                        mMsg.PostMessage("    Initializing FUWO UV LED tool at port " + Adrs + " ... ");
                        UvLamp = new Instrument.iFUWO(Instrument.iFUWO.ChannelEnum.ChannelAll);

                        break;
                    case "EXFO":
                        //ack
                        mMsg.PostMessage("    Initializing OmniCure S2000 UV Lamp at port " + Adrs + " ... ");
                        UvLamp = new Instrument.iOmniCure();

                        break;

                    case "FUTANSI":
                        //ack
                        mMsg.PostMessage("    Initializing Futansi UV Lamp at port " + Adrs + " ... ");
                        UvLamp = new Instrument.iFUTANSI(Instrument.iFUTANSI.ChannelEnum.Channel1);

                        break;
                    default:
                        mMsg.PostMessage("    UV Lamp type: " + s + " is not supported!");
                        return true;
                }

                //start initialzation
                s = "";
                success = UvLamp.Initialize(Adrs, true);

                if (success)
                {
                    //check UV lamp status
                    if (UvLamp.AlarmOn)
                    {
                        s = "UV lamp alarm is on.";
                        //ElseIf UvLamp.NeedCalibration Then
                        //    s = "UV lamp need calibration."
                    }
                }
                else
                {
                    UvLamp = null;
                }

                if (!string.IsNullOrEmpty(s))
                {
                    s += " Do you want to continue?";
                    XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Fail);
                    r = MessageBox.Show(s, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Running);

                    if (r == DialogResult.No)
                    {
                        UvLamp.Close();
                        UvLamp = null;
                    }

                    return r == DialogResult.Yes;

                }

                if ((UvLamp == null))
                {
                    s = "UV lamp is not connected or available. Do you want to continue any way?";
                    XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Fail);
                    r = MessageBox.Show(s, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    XpsIO.SetPostLight(Instrument.iXpsIO.PostLightStatusEnum.Running);
                    return r == DialogResult.Yes;
                }
                else
                {
                    return true;
                }


            }

            #endregion

            #region "stage"
            private bool InitializeXPS()
            {
                string s = null;
                string Adrs = null;
                bool success = false;

                //address
                Adrs = mIniFile.ReadParameter("Instrument", "AdrsXPS", "-");
                if (Adrs.StartsWith("-"))
                {
                    mMsg.PostMessage("    Configured for not useing Newport XPS motion controller");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing Newport XPS motion controller at IP address " + Adrs + " ... ");

                //initialize
                s = "";
                XPS = new Instrument.iXPS(8);
                success = XPS.Initialize(Adrs, true);

                //fail ack
                if (!success)
                {
                    XPS = null;
                    s = "Failed to initialize Newport XPS motion controller at IP address " + Adrs + ". Application will be closed.";
                    MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return success;

            }

            private bool InitializePiAngle()
            {
                string s = null;
                string Adrs = null;
                bool success = false;

                //address
                Adrs = mIniFile.ReadParameter("Instrument", "AdrsPiAngle", "-");
                if (Adrs.StartsWith("-"))
                {
                    mMsg.PostMessage("    Configured for not useing PI angle stage");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing PI angle stage at " + Adrs + " ... ");

                //initialize
                s = "";
                PiAngle = new Instrument.iPiGCS843(1);
                success = PiAngle.Initialize(Adrs, true);

                //fail ack
                if (!success)
                {
                    PiAngle = null;
                    s = "Failed to initialize PI angle stage " + Adrs + ". Application will be closed.";
                    MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //Home is necessary before moved
                success = PiAngle.HomeAndWait();

                return success;
            }

            private bool InitializePiHexopod()
            {
                string s = null;
                string Adrs = null;
                bool success = false;

                //address
                Adrs = mIniFile.ReadParameter("Instrument", "AdrsPiHexopod", "-");
                if (Adrs.StartsWith("-"))
                {
                    mMsg.PostMessage("    Configured for not useing PI Hexopod stage");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing PI Hexopod stage at " + Adrs + " ... ");

                //initialize
                s = "";
                Hexopod = new Instrument.iPiGCS(6);
                success = Hexopod.Initialize(Adrs, true);

                //fail ack
                if (!success)
                {
                    Hexopod = null;
                    s = "Failed to initialize PI Hexopod stage " + Adrs + ". Application will be closed.";
                    MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //Home is necessary before moved
                Hexopod.Axis = 0;
                // home all axis
                success = Hexopod.HomeAndWait();


                return success;
            }

            private bool InitializePiLS()
            {
                string s = null;
                string Adrs = null;
                bool success = false;

                //address
                Adrs = mIniFile.ReadParameter("Instrument", "AdrsPiLS65", "-");
                if (Adrs.StartsWith("-"))
                {
                    mMsg.PostMessage("    Configured for not useing PI Hexopod stage");
                    return true;
                }

                //ack
                mMsg.PostMessage("    Initializing PI Hexopod stage at " + Adrs + " ... ");

                //initialize
                s = "";
                PiLS = new Instrument.iPiLS65(1);
                success = PiLS.Initialize(Adrs, true);

                //fail ack
                if (!success)
                {
                    PiLS = null;
                    s = "Failed to initialize PI Linear stage " + Adrs + ". Application will be closed.";
                    MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //Home is necessary before moved
                PiLS.Axis = 1;
                // home all axis
                success = PiLS.HomeAndWait();


                return success;
            }

            //private bool InitializeRCX()
            //{
            //    string s = null;
            //    string Adrs = null;
            //    bool success = false;

            //    //address
            //    Adrs = mIniFile.ReadParameter("Instrument", "AdrsRCX", "-");
            //    if (Adrs.StartsWith("-"))
            //    {
            //        mMsg.PostMessage("    Configured for not useing Mitsubishi Robot");
            //        return true;
            //    }

            //    //ack
            //    mMsg.PostMessage("    Initializing Mitsubishi Robot ... ");

            //    //initialize
            //    s = "";
            //    RCX = new Instrument.iRCX();
            //    success = RCX.Initialize(true);

            //    //fail ack
            //    if (!success)
            //    {
            //        Hexopod = null;
            //        s = "Failed to initialize Mitsubishi Robot. Application will be closed.";
            //        MessageBox.Show(s, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //    return success;
            //}
            #endregion
        }

    }
}

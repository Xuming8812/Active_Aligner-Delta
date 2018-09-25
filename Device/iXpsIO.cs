using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Instrument
{
    public class iXpsIO
    {
        public enum PostLightStatusEnum
        {
            Running,
            Fail,
            Standby
        }

        public struct IoLineConfig
        {
            public bool Valid;
            public int Port;

            public int Line;
            public static IoLineConfig Parse(string s)
            {
                IoLineConfig x = default(IoLineConfig);
                string[] data = null;
                data = s.Trim().Split(',');
                try
                {
                    //NOTE: XPS line is 1 based, internal bit is 0 based, we will subtract 1 here to match the line with bit
                    x.Port = Convert.ToInt32(data[0]);
                    x.Line = Convert.ToInt32(data[1]) - 1;
                    x.Valid = true;
                }
                catch (Exception ex)
                {
                    string ss = ex.ToString();
                    x.Valid = false;
                }
                return x;
            }
        }

        private enum XpsIoChannelIndex
        {
            //do not change the name including case here. These names are uased as Key in the configuration file
            DoorInterlock,
            EmergencyStop,

            LampRunning,
            LampFail,
            LampStandby,

            //these 4 are analogy reading of the vacuum or pressure
            VacuumReadingInput,
            VacuumReadingOutputLens,
            VacuumReadingOutputBs,
            CdaReadingInput,

            //vacuum IO
            VacuumMain,
            VacuumMainReadback,

            VacuumHexapod,
            VacuumHexapodReadback,

            VacuumPackage,
            VacuumPackageReadback,

            VacuumOrCda,


            //epoxy 
            CdaEpoxy,
            EpoxyValve,

            //probe
            CdaProbe,

            //spare
            //SpareOutput1   
            SpareOutput2,
            SpareOutput3,
            SpareOutput4,
            SpareOutput5,
            SpareOutput6,

            //SpareInput1
            //SpareInput2
            //SpareInput3
            SpareInput4,
            SpareInput5,
            SpareInput6,
            SpareInput7,
            SpareInput8
        }

        private IoLineConfig[] mPara;

        private Instrument.iXPS mXPS;
        public bool HaveController
        {
            get { return (mXPS != null); }
        }

        public bool Initialize(ref Instrument.iXPS hXPS, ref w2.w2IniFileXML hConfig)
        {
            int i = 0;
            int ii = 0;
            string s = null;
            string key = null;
            Type xType = default(Type);

            const string section = "XpsIoTable";

            mXPS = hXPS;

            //read the parameter
            xType = typeof(XpsIoChannelIndex);
            ii = Enum.GetValues(xType).Length - 1;
            mPara = new IoLineConfig[ii + 1];
            for (i = 0; i <= ii; i++)
            {
                key = Enum.GetName(xType, i);
                s = hConfig.ReadParameter(section, key, "");
                mPara[i] = IoLineConfig.Parse(s);
                if (!mPara[i].Valid)
                {
                    s = "Cannot parse the XPS IO configuration for " + key;
                    MessageBox.Show(s, "IO Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // set GPIO 3 to defautl state "TRUE"
            //if (mXPS != null)
            //{
            //    mXPS.DigitOutput(3, 0) = false;
            //    mXPS.DigitOutput(3, 1) = true;
            //    mXPS.DigitOutput(3, 2) = true;
            //    mXPS.DigitOutput(3, 3) = true;
            //    mXPS.DigitOutput(3, 4) = true;
            //    mXPS.DigitOutput(3, 5) = true;
            //}

            return true;
        }

        public bool DoorClosed
        {
            get { return mXPS.ReadDigitInput(mPara[(int)XpsIoChannelIndex.DoorInterlock].Port, mPara[(int)XpsIoChannelIndex.DoorInterlock].Line); }
        }

        public bool EmergencyStop
        {
            get { return mXPS.ReadDigitInput(mPara[(int)XpsIoChannelIndex.EmergencyStop].Port, mPara[(int)XpsIoChannelIndex.EmergencyStop].Line); }
        }

        public void SetPostLight(PostLightStatusEnum value)
        {
            if (mXPS == null)
                return;
            mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.LampFail].Port, mPara[(int)XpsIoChannelIndex.LampFail].Line,value == PostLightStatusEnum.Fail);
            mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.LampRunning].Port, mPara[(int)XpsIoChannelIndex.LampRunning].Line,value == PostLightStatusEnum.Running);
            mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.LampStandby].Port, mPara[(int)XpsIoChannelIndex.LampStandby].Line,value == PostLightStatusEnum.Standby);
        }

        #region "vacuum IO"
        public enum VacuumLine
        {
            Main = 0,
            Hexapod = 1,
            Package = 2,
            LineInput = 3
        }

        private int GetVacuumLineXpsIoChannelIndex(VacuumLine eLine)
        {
            XpsIoChannelIndex index = default(XpsIoChannelIndex);
            switch (eLine)
            {
                case VacuumLine.Hexapod:
                    index = XpsIoChannelIndex.VacuumHexapod;
                    break;
                case VacuumLine.Main:
                    index = XpsIoChannelIndex.VacuumMain;
                    break;
                case VacuumLine.Package:
                    index = XpsIoChannelIndex.VacuumPackage;
                    break;
                default:
                    return -1;
            }

            return (int)index;
        }

        public bool ReadVacuumEnabled(VacuumLine eLine)
        {
            int index = 0;
            index = this.GetVacuumLineXpsIoChannelIndex(eLine);
            if (index < 0)
                return false;
            return !mXPS.ReadDigitOutput(mPara[index].Port, mPara[index].Line);
        }

        public bool SetVacuumEnabled(VacuumLine eLine,bool value)
        {
            int index = 0;
            index = this.GetVacuumLineXpsIoChannelIndex(eLine);
            if (index < 0)
                return false;
            return mXPS.SetDigitOutput(mPara[index].Port, mPara[index].Line, value);
        }

        public bool ReadVacuumEnabledReadback(VacuumLine eLine)
        {

                XpsIoChannelIndex index = default(XpsIoChannelIndex);
                switch (eLine)
                {
                    case VacuumLine.Hexapod:
                        index = XpsIoChannelIndex.VacuumHexapodReadback;
                        break;
                    case VacuumLine.Main:
                        index = XpsIoChannelIndex.VacuumMainReadback;
                        break;
                    case VacuumLine.Package:
                        //index = XpsIoChannelIndex.VacuumPackageReadback
                        //we do not have feedback line for the package, just use the set value
                        return this.ReadVacuumEnabled(VacuumLine.Package);
                    default:
                        return false;
                }
                return !mXPS.ReadDigitOutput(mPara[(int)index].Port, mPara[(int)index].Line);
            }
        

        public bool VacuumLinePressurized
        {
            get { return mXPS.ReadDigitOutput(mPara[(int)XpsIoChannelIndex.VacuumOrCda].Port, mPara[(int)XpsIoChannelIndex.VacuumOrCda].Line); }
            set { mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.VacuumOrCda].Port, mPara[(int)XpsIoChannelIndex.VacuumOrCda].Line,value); }
        }
        #endregion

        #region "Probe"
        public bool ProbePositionOn
        {
            get { return mXPS.ReadDigitOutput(mPara[(int)XpsIoChannelIndex.CdaProbe].Port, mPara[(int)XpsIoChannelIndex.CdaProbe].Line); }
            set { mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.CdaProbe].Port, mPara[(int)XpsIoChannelIndex.CdaProbe].Line,value); }
        }
        #endregion

        #region "Epoxy/injector"
        public bool EpoxyMoveOut
        {
            get { return mXPS.ReadDigitOutput(mPara[(int)XpsIoChannelIndex.CdaEpoxy].Port, mPara[(int)XpsIoChannelIndex.CdaEpoxy].Line); }
            set { mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.CdaEpoxy].Port, mPara[(int)XpsIoChannelIndex.CdaEpoxy].Line,value); }
        }

        public void EpoxyTriggerOnce()
        {
            //open contact to arm the trigger
            mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.EpoxyValve].Port, mPara[(int)XpsIoChannelIndex.EpoxyValve].Line,true);
            System.Threading.Thread.Sleep(100);
            //trun it on
            mXPS.SetDigitOutput(mPara[(int)XpsIoChannelIndex.EpoxyValve].Port, mPara[(int)XpsIoChannelIndex.EpoxyValve].Line,false);
        }

        #endregion

        #region "Vacuum/Pressure reading"
        public double VacuumLevel(VacuumLine eLine)
        {
                XpsIoChannelIndex index = default(XpsIoChannelIndex);

                switch (eLine)
                {
                    case VacuumLine.Hexapod:
                        index = XpsIoChannelIndex.VacuumReadingOutputBs;
                        break;
                    case VacuumLine.Main:
                        index = XpsIoChannelIndex.VacuumReadingOutputLens;
                        break;
                    case VacuumLine.LineInput:
                        index = XpsIoChannelIndex.VacuumReadingInput;
                        break;
                    case VacuumLine.Package:
                        return double.NaN;
                    default:
                        return double.NaN;
                }

                return this.GetVacuumPressure(mPara[(int)index].Line);
        }

        public double CompressAirPressure
        {
            get { return this.GetVacuumPressure(mPara[(int)XpsIoChannelIndex.CdaReadingInput].Line); }
        }

        private double GetVacuumPressure(int Channel)
        {
            double V = 0;

            const double V1 = 2.8371;
            const double V2 = 3.943;
            const double P1 = -48.8;
            const double P2 = -77.4;

            //convert analogy voltage to pressure in kPa , channel + 1 to accomodate the bit mask
            V = mXPS.ReadAnalogyInput(Channel + 1);

            V -= V1;
            V /= (V2 - V1);
            V *= (P2 - P1);
            V += P1;

            if (Channel == mPara[(int)XpsIoChannelIndex.CdaReadingInput].Line)
            {
                V *= -0.01;
            }

            return V;
        }
        #endregion

    }
}

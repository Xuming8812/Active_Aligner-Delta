using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Instrument
{
    public class iAgilent3440xA : iDigitalMultimeter
    {

        private bool mAutoRange;
        private double mRange;

        private double mResolution;
        /// <summary>
        /// Overrides the base initialization method
        /// </summary>
        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            //intialize through base
            bool success = base.Initialize(AdrsBoard, AdrsInstrument, RaiseError);
            //do system initialization
            if (success)
            {
                this.SendCmd("*RST");
                mAutoRange = true;
                mRange = 10.0;
                mResolution = 0.003;
            }
            //return
            return success;
        }

        public override InputTerminalEnum InputTerminal
        {
            get
            {
                string s = null;
                s = base.QueryString("ROUT:TERM?");
                switch (s)
                {
                    case "FRON":
                        return InputTerminalEnum.Front;
                    case "REAR":
                        return InputTerminalEnum.Real;
                    default:
                        return InputTerminalEnum.Front;
                }
            }
        }

        public override bool AutoRange
        {
            get { return mAutoRange; }
            set { mAutoRange = value; }
        }

        public override double Range
        {
            get { return mRange; }
            set { mRange = value; }
        }

        public override double Resolution
        {
            get { return mResolution; }
            set { mResolution = value; }
        }

        public override double ACI
        {
            get
            {
                string s = null;
                s = "MEAS:CURR:AC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double ACV
        {
            get
            {
                string s = null;
                s = "MEAS:VOLT:AC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double DCI
        {
            get
            {
                string s = null;
                s = "MEAS:CURR:DC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double DCV
        {
            get
            {
                string s = null;
                s = "MEAS:VOLT:DC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double Resistance
        {
            get
            {
                string s = null;
                s = "MEAS:RES?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        protected string GetRangeString()
        {
            dynamic s = null;

            if (mAutoRange)
            {
                s = " AUTO";
            }
            else if (mRange > 0)
            {
                s = " " + mRange;
                if (mResolution > 0)
                {
                    s = s + "," + mResolution;
                }
            }
            else
            {
                s = "";
            }

            return s;
        }

    }

    public class iAgilent34970A_DMM : iAgilent3440xA
    {

        #region "34970 specific"
        private int mMaxChannel = -1;

        private string mChannel = "(@101)";
        //system, read card information
        public string GertModuleType(int Slot)
        {
            string s = null;

            s = "SYST:CTYP? " + Slot + "00";
            s = base.QueryString(s);
            //get max channel
            if (s.Contains("34901A") || s.Contains("34903A"))
            {
                mMaxChannel = 20;
            }
            else if (s.Contains("34902A"))
            {
                mMaxChannel = 16;
            }
            else if (s.Contains("349044"))
            {
                mMaxChannel = 48;
            }
            else if (s.Contains("34905A") || s.Contains("34906A"))
            {
                mMaxChannel = 24;
            }
            else if (s.Contains("34907A"))
            {
                mMaxChannel = -1;
            }
            else if (s.Contains("34908A"))
            {
                mMaxChannel = 40;
            }
          
            return s;
        }

        public bool SetChannel(int Slot, int Channel)
        {
            if (Slot < 1)
                return false;
            if (Slot > 3)
                return false;

            if (mMaxChannel == -1)
                this.GertModuleType(Slot);

            if (Channel < 1)
                return false;
            if (Channel > mMaxChannel)
                return false;

            mChannel = "(@" + Slot.ToString("0") + Channel.ToString("00") + ")";
            return true;
        }

        //build scan list
        private string BuildScanList(int slot, params int[] channels)
        {
            int i = 0;
            int ii = 0;
            string s = null;

            if (slot < 1)
                return "";
            if (slot > 3)
                return "";

            if (mMaxChannel == -1)
                this.GertModuleType(slot);

            s = "(@";
            ii = channels.Length - 1;
            for (i = 0; i <= ii; i++)
            {
                if (channels[i] < 1)
                    return "";
                if (channels[i] > mMaxChannel)
                    return "";

                s += slot.ToString("0") + channels[i].ToString("00");
                if (i != ii)
                    s += ",";
            }

            s += ")";

            return s;
        }

        public bool ConfigScanResistance(int slot, params int[] channels)
        {
            string s = null;

            s = this.BuildScanList(slot, channels);
            if (string.IsNullOrEmpty(s))
                return false;
            s = "CONF:RES " + GetRangeString(s);
            base.SendCmd(s);

            return true;
        }

        public double[] ReadScanResults()
        {
            string s = null;
            NationalInstruments.NI4882.TimeoutValue old = default(NationalInstruments.NI4882.TimeoutValue);

            old = mGPIB.IOTimeout;
            mGPIB.IOTimeout = NationalInstruments.NI4882.TimeoutValue.T10s;
            s = base.QueryString("READ?");
            mGPIB.IOTimeout = old;

            return ParseStringToArray(s);
        }

        private double[] ParseStringToArray(string sRead)
        {
            int i = 0;
            int ii = 0;
            string[] data = null;
            double[] v = null;

            data = sRead.Split(',');
            ii = data.Length;
            v = new double[ii];

            for (i = 0; i <= ii - 1; i++)
            {
                v[i] = Conversion.Val(data[i]);
            }

            return v;
        }
        #endregion

        #region "inherits overloads"
        public override InputTerminalEnum InputTerminal
        {
            get { return InputTerminalEnum.Internal; }
        }

        public override double ACI
        {
            get
            {
                string s = null;
                s = "MEAS:CURR:AC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double ACV
        {
            get
            {
                string s = null;
                s = "MEAS:VOLT:AC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double DCI
        {
            get
            {
                string s = null;
                s = "MEAS:CURR:DC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double DCV
        {
            get
            {
                string s = null;
                s = "MEAS:VOLT:DC?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        public override double Resistance
        {
            get
            {
                string s = null;
                s = "MEAS:RES?" + GetRangeString();
                return base.QueryValue(s);
            }
        }

        private new string GetRangeString()
        {
            return GetRangeString(mChannel);
        }

        private  string GetRangeString(string ScanList)
        {
            string s = null;
            s = base.GetRangeString();
            if (s.EndsWith("?"))
            {
                s += " " + ScanList;
            }
            else
            {
                s += ", " + ScanList;
            }
            return s;
        }
        #endregion
    }

    public class iKeithely2000 : iDigitalMultimeter
    {

        //Private Enum ResolutionDigitEnum
        //    ThreeAndHalfDigits = 4
        //    FourAndHalfDigits = 5
        //    FiveAndHalfDigits = 6
        //    SixAndHalfDigits = 7
        //End Enum

        private bool mAutoRange;
        private double mRange;

        private double mResolution;
        /// <summary>
        /// Overrides the base initialization method
        /// </summary>
        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            //intialize through base
            bool success = base.Initialize(AdrsBoard, AdrsInstrument, RaiseError);
            //do system initialization
            if (success)
            {
                this.SendCmd("*RST");
                mAutoRange = true;
                mRange = 10.0;
                mResolution = 0.03;

            }
            //return
            return success;
        }

        public override InputTerminalEnum InputTerminal
        {
            get
            {
                string s = null;
                s = base.QueryString(":SYST:FRSW?");
                switch (s)
                {
                    case "1":
                        return InputTerminalEnum.Front;
                    case "0":
                        return InputTerminalEnum.Real;
                    default:
                        return InputTerminalEnum.Front;
                }
            }
        }

        public override bool AutoRange
        {
            get { return mAutoRange; }
            set { mAutoRange = value; }
        }

        public override double Range
        {
            get { return mRange; }
            set { mRange = value; }
        }

        public override double Resolution
        {
            get { return mResolution; }
            set { mResolution = value; }
        }

        public override double ACI
        {
            get
            {
                string s = null;

                if (mAutoRange)
                {
                    s = ":SENS:CURR:AC:RANG:AUTO 1";
                }
                else
                {
                    s = ":SENS:CURR:AC:RANG: " + mRange.ToString("0.0");
                }
                base.SendCmd(s);

                return base.QueryValue("MEAS:CURR:AC?");
            }
        }

        public override double ACV
        {
            get
            {
                string s = null;

                if (mAutoRange)
                {
                    s = ":SENS:VOLT:AC:RANG:AUTO 1";
                }
                else
                {
                    s = ":SENS:VOLT:AC:RANG: " + mRange.ToString("0");
                }
                base.SendCmd(s);

                return base.QueryValue("MEAS:VOLT:AC?");

            }
        }

        public override double DCI
        {
            get
            {
                string s = null;

                if (mAutoRange)
                {
                    s = ":SENS:CURR:DC:RANG:AUTO 1";
                }
                else
                {
                    s = ":SENS:CURR:DC:RANG: " + mRange.ToString("0.0");
                }
                base.SendCmd(s);

                return base.QueryValue("MEAS:CURR:DC?");

            }
        }

        public override double DCV
        {
            get
            {
                string s = null;

                if (mAutoRange)
                {
                    s = ":SENS:VOLT:DC:RANG:AUTO 1";
                }
                else
                {
                    s = ":SENS:VOLT:DC:RANG: " + mRange.ToString("0");
                }
                base.SendCmd(s);

                return base.QueryValue("MEAS:VOLT:DC?");

            }
        }

        public override double Resistance
        {
            get
            {
                string s = null;

                if (mAutoRange)
                {
                    s = ":SENS:RES:RANG:AUTO 1";
                }
                else
                {
                    s = ":SENS:RES:RANG: " + mRange.ToString("0");
                }
                base.SendCmd(s);

                return base.QueryValue("MEAS:RES?");
            }
        }

    }

    public class iKeithley6485 : iDigitalMultimeter
    {

        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            //intialize through base
            bool success = base.Initialize(AdrsBoard, AdrsInstrument, RaiseError);
            //do system initialization so that we are in known state
            if (success)
            {
                this.SendCmd("*RST");
            }
            //return
            return success;
        }

        #region "null property "
        public override double ACI
        {

            get { return 0.0; }
        }

        public override double ACV
        {

            get { return 0.0; }
        }

        public override double DCV
        {

            get { return 0.0; }
        }

        public override iDigitalMultimeter.InputTerminalEnum InputTerminal
        {

            get { return 0.0; }
        }

        public override double Resistance
        {

            get { return 0.0; }
        }

        public override double Resolution
        {

            get { return 0.0; }

            set { }
        }
        #endregion

        public override bool AutoRange
        {
            get
            {
                string s = null;
                s = this.QueryString("SENS:CURR:RANG:AUTO?");
                return (s == "1");
            }
            set
            {
                string s = null;
                s = "SENS:CURR:RANG:AUTO ";
                s += (value ? "1" : "0").ToString();
                this.SendCmd(s);
            }
        }

        public override double DCI
        {
            get
            {
                string s = null;
                string[] data = null;
                s = this.QueryStringWait("MEAS?", 300);
                data = s.Split(',');
                return Conversion.Val(data[0]);
            }
        }

        public override double Range
        {
            get { return this.QueryValue("SENS:CURR:RANG?"); }
            set
            {
                string s = null;
                s = "SENS:CURR:RANG ";
                s += value.ToString("0E0");
                this.SendCmd(s);
            }
        }

        public bool ZeroCheckEnabled
        {
            get
            {
                string s = null;
                s = this.QueryString("SYST:ZCH?");
                return s == "1";
            }
            set
            {
                string s = null;
                s = "SYST:ZCH " + (value ? "1" : "0").ToString();
                this.SendCmd(s);
            }
        }

        public bool ZeroCorrectionEnabled
        {
            get
            {
                string s = null;
                s = this.QueryString("SYST:ZCOR?");
                return s == "1";
            }
            set
            {
                string s = null;
                s = "SYST:ZCOR " + (value ? "1" : "0").ToString();
                this.SendCmd(s);
            }
        }

        public void ZeroMeter()
        {
            this.ZeroCheckEnabled = true;
            this.SendCmd("SYST:ZCOR:ACQ");
            this.ZeroCorrectionEnabled = true;
        }
    }

    public abstract class iDigitalMultimeter : iGPIB
    {

        public enum InputTerminalEnum
        {
            Front,
            Real,
            Internal
        }

        #region "instrument auto detection"
        public static iDigitalMultimeter AutoInitialize(int AdrsInstrument, bool RaiseError)
        {
            return AutoInitialize(0, AdrsInstrument, RaiseError);
        }

        public static iDigitalMultimeter AutoInitialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            iDigitalMultimeter DMM = null;

            //connect to GPIB device
            Instrument.iGPIB gpib = new Instrument.iGPIB();
            if (gpib.Initialize(AdrsBoard, AdrsInstrument, RaiseError))
            {
                if (gpib.ModelName.Contains("MODEL 2000"))
                {
                    DMM = new Instrument.iKeithely2000();
                }
                else if (gpib.ModelName.Contains("344") && gpib.ModelName.Contains("A"))
                {
                    DMM = new Instrument.iAgilent3440xA();
                }
                else if (gpib.ModelName.Contains("6485"))
                {
                    DMM = new Instrument.iKeithley6485();
                }

            }
            if (!DMM.Initialize(AdrsBoard, AdrsInstrument, RaiseError))
                DMM = null;

            return DMM;
        }
        #endregion

        public abstract double Range { get; set; }
        public abstract bool AutoRange { get; set; }
        public abstract double Resolution { get; set; }

        public abstract double DCV { get; }
        public abstract double DCI { get; }

        public abstract double ACV { get; }
        public abstract double ACI { get; }

        public abstract double Resistance { get; }

        public abstract InputTerminalEnum InputTerminal { get; }

        public double ReadDCV(int Samples)
        {
            int i = 0;
            double v = 0;

            v = 0.0;
            for (i = 1; i <= Samples; i++)
            {
                v += this.DCV;
            }
            return v / Samples;
        }

    }
}

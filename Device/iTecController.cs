using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.IO.Ports;
using System.Windows.Forms;

namespace Instrument
{
    public class iTecController : iTemperatureController
    {
        public enum LaserDiodeDriverType
        {
            LDD,
            TEC,
            Combo,
            Empty
        }

        private LaserDiodeDriverType[] mModuleType;

        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            if (base.Initialize(AdrsBoard, AdrsInstrument, RaiseError))
            {
                this.ConfigChannels();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ConfigChannels()
        {
            if (base.mManufacturer.Contains("Newport"))
            {
                //this is special for Newport
                //get all existin modules and check their type
                int i = 0;
                int ii = 0;
                string[] s = null;

                s = this.GetChannelList();
                ii = s.Length - 1;

                if (ii >= 0)
                {
                    mModuleType = new LaserDiodeDriverType[ii + 1];
                    for (i = 0; i <= ii; i++)
                    {
                        if (s[i].Contains("LDD") && s[i].Contains("TEC"))
                        {
                            mModuleType[i] = LaserDiodeDriverType.Combo;
                        }
                        else if (s[i].Contains("LDD"))
                        {
                            mModuleType[i] = LaserDiodeDriverType.LDD;
                        }
                        else if (s[i].Contains("TEC"))
                        {
                            mModuleType[i] = LaserDiodeDriverType.TEC;
                        }
                        else
                        {
                            mModuleType[i] = LaserDiodeDriverType.Empty;
                        }                        
                    }
                }

            }
            else
            {
                //otherwise, only one channel for TEC
                mModuleType = new LaserDiodeDriverType[2];
                mModuleType[0] = LaserDiodeDriverType.Empty;
                mModuleType[1] = LaserDiodeDriverType.TEC;
            }
        }

        #region "RS232 for GPIB"

        private  SerialPort mPort;
        public bool Initialize(string sPort, bool RaiseError)
        {
            string s = null;
            string[] sData = null;

            //set port
            mPort = new  SerialPort(sPort, 38400,  Parity.None, 8,  StopBits.One);
            mPort.ReadTimeout = 1000;

            //open port
            try
            {
                mPort.Open();

                //set port properties
                mPort.NewLine = "\r";

                s = this.QueryString("*IDN?");
                if (s.Contains(","))
                {
                    //this is for standard GPIB
                    sData = s.Split(',');
                }
                else
                {
                    //this is Arroyo special
                    sData = s.Split(' ');
                }

                mManufacturer = sData[0].Trim();

                if (sData.Length > 1)
                    mModel = sData[1].Trim();
                if (mModel == "0")
                    mModel = mManufacturer;
                //some old instrument mix this up

                if (sData.Length > 2)
                    mSerialNumber = sData[2].Trim();
                if (sData.Length > 3)
                    mFirmwareVersion = sData[3].Trim();

                this.ConfigChannels();

                return true;
            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, this.Name);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;
            }

        }

        public void Dispose()
        {
            this.Enabled = false;
            mPort.Close();
        }

        public override string QueryString(string sCmd)
        {
            string s = null;

            //this make sure GPIB interface will also work
            if (mPort == null)
                return base.QueryString(sCmd);

            //this is for COM port
            mPort.DiscardInBuffer();
            mPort.ReadExisting();
            this.SendCmd(sCmd);

            s = mPort.ReadLine();
            s = s.Replace("\r", "");
            return s;
        }


        public override void SendCmd(string sCmd)
        {
            //this make sure GPIB interface will also work
            if (mPort == null)
            {
                base.SendCmd(sCmd);
                return;
            }

            mPort.DiscardOutBuffer();
            sCmd += ControlChars.Cr;
            mPort.Write(sCmd);
            System.Threading.Thread.Sleep(50);
            //50 ms is tested to be the "minimum" required before being too fast causing time out problem 
        }
        #endregion

        #region "multi channel system from Newport"
        public int ChannelCount
        {
            //first channel, 0, is the main frame
            get { return (mModuleType.Length - 1); }
        }

        public string[] GetChannelList()
        {
            string s = null;
            s = this.QueryString("EQUIP?");
            return s.Split(',');
        }

        public bool IsTEC(int Channel)
        {
            if (Channel < 0)
                return false;
            if (Channel >= mModuleType.Length)
                return false;
            return mModuleType[Channel] == LaserDiodeDriverType.TEC || mModuleType[Channel] == LaserDiodeDriverType.Combo;
        }

        public int Channel
        {
            get
            {
                string s = null;
                s = this.QueryString("TEC:CHAN?");
                return int.Parse(s.Split(',')[0]);
            }
            set { this.SendCmd("TEC:CHAN " + value); }
        }
        #endregion

        #region "Unused properties"
        public override double CycleTime
        {
            get { return 0.0; }
            //do nothing
            set { }
        }

        public override double DAC
        {
            get { return 0.0; }
            //do nothing
            set { }
        }

        public override double PID_D
        {

            get { return 0.0; }

            set { }
        }

        public override double PID_I
        {

            get { return 0.0; }

            set { }
        }

        public override double PID_P
        {

            get { return 0.0; }

            set { }
        }
        #endregion

        public double TemperatureMax
        {
            get { return this.QueryValue("TEC:LIM:THI?"); }
            set { this.SendCmd("TEC:LIM:THI " + value); }
        }

        public double TemperatureMin
        {
            get { return this.QueryValue("TEC:LIM:TLO?"); }
            set { this.SendCmd("TEC:LIM:TLO " + value); }
        }

        #region "overrided properties and methods"

        public override iTemperatureController.ControlModeEnum ControlMode
        {
            get
            {
                string s = null;

                s = this.QueryString("TEC:MODE?");
                switch (s)
                {
                    case "ITE":
                        return ControlModeEnum.ConstCurrent;
                    case "R":
                        return ControlModeEnum.ConstResistance;
                    case "T":
                        return ControlModeEnum.ConstTemperature;
                default:
                    return ControlModeEnum.ConstTemperature;
            }
            }
            set
            {
                string s = null;

                switch (value)
                {
                    case ControlModeEnum.ConstCurrent:
                        s = "ITE";
                        break;
                    case ControlModeEnum.ConstResistance:
                        s = "R";
                        break;
                    case ControlModeEnum.ConstTemperature:
                        s = "T";
                        break;
                    default:
                        return;
                }

                s = "TEC:MODE:" + s;
                this.SendCmd(s);
            }
        }

        public override bool Enabled
        {
            get { return (this.QueryString("TEC:OUT?") == "1"); }
            set
            {
                string s = null;
                s = (value ? "1" : "0").ToString();
                s = "TEC:OUT " + s;
                this.SendCmd(s);
            }
        }

        public override iTemperatureController.SensorTypeEnum SensorType
        {
            get
            {
                string s = null;
                s = this.QueryString("TEC:SEN?");
                switch (s)
                {
                    case "1":
                        return SensorTypeEnum.ThermistorHighCurrent;
                    case "2":
                        return SensorTypeEnum.ThermistorLowCurrent;
                    case "3":
                        return SensorTypeEnum.LM335;
                    case "4":
                        return SensorTypeEnum.AD590;
                    case "5":
                        return SensorTypeEnum.RTD;
                    case "6":
                        return SensorTypeEnum.RTD4Wire;
                    case "7":
                        return SensorTypeEnum.Thermistor1mA;
                    default:
                        return SensorTypeEnum.Other;
                }
            }
            set
            {
                string s = null;

                switch (value)
                {
                    case SensorTypeEnum.ThermistorHighCurrent:
                        s = "1";
                        break;
                    case SensorTypeEnum.ThermistorLowCurrent:
                        s = "2";
                        break;
                    case SensorTypeEnum.LM335:
                        s = "3";
                        break;
                    case SensorTypeEnum.AD590:
                        s = "4";
                        break;
                    case SensorTypeEnum.RTD:
                        s = "5";
                        break;
                    case SensorTypeEnum.RTD4Wire:
                        s = "6";
                        break;
                    case SensorTypeEnum.Thermistor1mA:
                        s = "7";
                        break;
                    default:
                        return;
                }

                s = "TEC:SEN " + s;
                this.SendCmd(s);
            }
        }

        public override double Setpoint
        {
            get { return this.QueryValue("TEC:SET:T?"); }
            set { this.SendCmd("TEC:T " + value); }
        }

        public override double Temperature
        {
            get { return this.QueryValue("TEC:T?"); }
        }

        public override TemperatureUnitEnum TemperatureUnit
        {
            get { return TemperatureUnitEnum.DegC; }
            //do nothing
            set { }
        }
        #endregion
    }

    public abstract class iTemperatureController : iGPIB
    {

        #region "enum"
        public enum SensorTypeEnum
        {
            None = 0,
            Thermistor = 1,
            ThermistorHighCurrent = 1,
            ThermistorLowCurrent = 2,
            LM335 = 3,
            AD590 = 4,

            RTD = 5,
            RTDJIS = 6,
            RTDDIN = 7,
            RTD4Wire = 8,

            TC_TypeJ = 10,
            TC_TypeK = 11,
            TC_TypeT = 12,
            TC_TypeE = 13,
            TC_TypeN = 14,
            TC_TypeC = 15,
            TC_TypeD = 16,
            TC_TypePT2 = 17,
            TC_TypeR = 18,
            TC_TypeS = 19,
            TC_TypeB = 20,
            TC_TypeL = 21,

            Thermistor1mA = 50,

            Other = 100
        }

        public enum ControlModeEnum
        {
            ConstCurrent = 0,
            ConstResistance = 1,
            ConstTemperature = 2,
            ConstVoltage = 3
        }

        public enum AlarmTypeEnum
        {
            Off = 0,
            LowLimit = 1,
            HighLimit = 2,
            LowAndHigh = 3
        }

        #endregion

        public abstract bool Enabled { get; set; }

        public abstract TemperatureUnitEnum TemperatureUnit { get; set; }

        public abstract double Setpoint { get; set; }
        public abstract double Temperature { get; }

        public abstract ControlModeEnum ControlMode { get; set; }
        public abstract SensorTypeEnum SensorType { get; set; }

        public abstract double PID_P { get; set; }
        public abstract double PID_I { get; set; }
        public abstract double PID_D { get; set; }
        public abstract double CycleTime { get; set; }
        public abstract double DAC { get; set; }

        //extra interface for reading temperature
        public double ReadTemperature()
        {
            return this.Temperature;
        }
    }
}


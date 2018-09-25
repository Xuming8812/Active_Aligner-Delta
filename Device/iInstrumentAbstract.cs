using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NationalInstruments;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.IO.Ports;

namespace Instrument
{

    #region "enum"
    //common enumation
    public enum PowerUnitEnum
    {
        Unknown = -1,
        W = 0,
        uW = 1,
        mW = 2,
        dBm = 3,
        dB = 4,
        Relative = 5
    }

    public enum MediumEnum
    {
        Air = 1,
        Vacuum = 2
    }

    public enum TriggleModeEnum
    {
        Single,
        Continuous
    }

    public enum TemperatureUnitEnum
    {
        None = -1,

        DegC = 0,
        DegF = 1,
        Kelvin = 2
    }
    #endregion

    /// <summary>
    /// All physics are in MKS unit
    /// </summary>
    /// <remarks></remarks>
    public class Physics
    {
        public static double Cvacuum = 299792458.0;

        public static double RefractiveIndexAir = 1.00026407;
        public static double GHzTonm(double Frequency_GHz)
        {
            if (Frequency_GHz > 0)
            {
                return (Cvacuum / RefractiveIndexAir / Frequency_GHz);
            }
            else
            {
                return 0.0;
            }
        }

        public static double nmToGHz(double Wavelength_nm)
        {
            if (Wavelength_nm > 0)
            {
                return (Cvacuum / RefractiveIndexAir / Wavelength_nm);
            }
            else
            {
                return 0.0;
            }
        }

        public static double mWTodBm(double Power_mW)
        {
            return 10.0 * Math.Log10(Power_mW);
        }

        public static double dBmTomW(double Power_dBm)
        {
            return Math.Pow(10.0, (0.1 * Power_dBm));
        }

        public static double Thermistor_TemperatureToResistance(double Temperature_DegC)
        {
            double v = 0;
            double alpha = 0;
            double beta = 0;

            const double A = 0.001129148;
            const double B = 0.000234125;
            const double C = 8.76741E-08;

            v = 273.15 + Temperature_DegC;
            alpha = 0.5 * (A - 1.0 / v) / C;

            beta = Math.Pow((B / 3.0 / C), 3);
            beta += Math.Pow(alpha, 2);
            beta = Math.Sqrt(beta);

            v = Math.Pow((beta - alpha), (1 / 3));
            v -= Math.Pow((beta + alpha), (1 / 3));

            return Math.Exp(v);
        }


    }

    /// <summary>
    /// Abstract instrument class for all instrument hardwares
    /// </summary>
    /// <remarks></remarks>
    public abstract class iInstrumentGeneric
    {

        /// <summary>
        /// This is the class entry point beside the constructer to initialize the hardware
        /// </summary>
        /// <param name="InstrumentAddress">GPIB board address, or COM port, USB ID</param>
        /// <param name="AdditionalInfo">GPIB instrument address, COM baud rate, and etc</param>
        /// <param name="RaiseError">if error message shall be shown when error encountered</param>
        /// <returns>True if successful</returns>
        /// <remarks></remarks>
        public abstract bool Initialize(int InstrumentAddress, int AdditionalInfo, bool RaiseError);

        //overloaded initialization short cuts
        public virtual bool Initialize(int AdrsInstrument)
        {
            return this.Initialize(0, AdrsInstrument, false);
        }

        public virtual bool Initialize(int AdrsInstrument, bool RaiseError)
        {
            return this.Initialize(0, AdrsInstrument, RaiseError);
        }

        public abstract string Name { get; }

        public abstract bool Connected { get; set; }

        //duplicated method for Connected Set property, 
        //this is mainly to emulate the .NET serial port method
        public void Open()
        {
            this.Connected = true;
        }
        public void Close()
        {
            this.Connected = false;
        }

        //these are only used for inherated classes
        public abstract double QueryValue(string sCmd);
        public abstract string QueryString(string sCmd);
        public abstract void SendCmd(string sCmd);
    }

    public class iGPIB : iInstrumentGeneric
    {


        protected NationalInstruments.NI4882.Device mGPIB;
        protected string mModel = "";
        protected string mManufacturer = "";
        protected string mSerialNumber = "";

        protected string mFirmwareVersion = "";
        protected int mAdrsBoard;

        protected int mAdrsDevice;
        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            string s = null;
            string[] sData = null;

            //save data
            mAdrsBoard = AdrsBoard;
            mAdrsDevice = AdrsInstrument;

            //start device
            mGPIB = new NationalInstruments.NI4882.Device(AdrsBoard, Convert.ToByte(AdrsInstrument));
            var _with1 = mGPIB;
            //set a default timeout value
            _with1.IOTimeout = NationalInstruments.NI4882.TimeoutValue.T1s;

            //get instrument model number
            try
            {
                int Wait = 500;
                s = QueryString("*IDN?", Wait);
                sData = s.Split(',');

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

                //return for success
                return true;

            }
            catch (NationalInstruments.NI4882.GpibException ex)
            {
                if (ex.ErrorCode == NationalInstruments.NI4882.GpibError.IOOperationAborted)
                {
                    //time out error is ok as some old instrument does not implement *IDN? command
                    return true;
                }
                else
                {
                    if (RaiseError)
                        MessageBox.Show(ex.ErrorMessage, this.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

            }
            catch (Exception ex)
            {
                if (RaiseError)
                    MessageBox.Show(ex.Message, this.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }


        }

        public override string Name
        {
            get { return "GPIB basic interface"; }
        }

        public int AddressBoard
        {
            get { return mAdrsBoard; }
        }

        public int AddressDevice
        {
            get { return mAdrsDevice; }
        }

        public NationalInstruments.NI4882.Device GPIBDevice
        {
            get { return mGPIB; }
        }

        public virtual string ModelName
        {
            get { return mModel; }
            set { mModel = value; }
        }

        public virtual string SerialNumber
        {
            get { return mSerialNumber; }
        }

        public virtual string FirmwareVersion
        {
            get { return mFirmwareVersion; }
        }

        public virtual string Manufacturer
        {
            get { return mManufacturer; }
        }

        public override bool Connected
        {
            get { return (mGPIB != null); }
            //do not need to 
            set { }
        }

        public virtual void InitializeSetting(string sSetting)
        {
            string[] cmdBuff = sSetting.Split(';');
            foreach (string s in cmdBuff)
            {
                SendCmd(s);
            }
        }

        #region "protected read/write"
        public override void SendCmd(string sCmd)
        {
            mGPIB.Write(sCmd);
        }

        public override string QueryString(string sCmd)
        {
            return QueryString(sCmd, 0);
        }

        protected string QueryString(string sCmd, int BufferSize)
        {
            mGPIB.Write(sCmd);
            return ReadData(Convert.ToInt32(BufferSize));
        }

        protected string QueryStringWait(string sCmd, int WaitTime_ms)
        {
            mGPIB.Write(sCmd);
            if (WaitTime_ms >= 0)
                System.Threading.Thread.Sleep(WaitTime_ms);
            return ReadData(0);
        }

        protected string QueryStringWait(string sCmd, byte WaitBit, int TimeOut_ms)
        {
            //write command
            mGPIB.Write(sCmd);
            //wait until data is available
            this.WaitStatusBit(WaitBit, TimeOut_ms);
            //read data
            return ReadData(0);
        }

        public override double QueryValue(string sCmd)
        {
            string s = QueryString(sCmd);
            if (Information.IsNumeric(s))
            {
                return Convert.ToDouble(s);
            }
            else
            {
                return double.NaN;
            }
        }

        private string ReadData(int BufferSize)
        {
            string s = null;

            //read
            if (BufferSize == 0)
            {
                s = mGPIB.ReadString();
            }
            else
            {
                s = mGPIB.ReadString(BufferSize);
            }

            //remove EOL character
            s = s.Replace(Constants.vbLf, "");
            s = s.Replace(Constants.vbCr, "");

            //strip quto
            s = s.Replace("\"", "");

            //return
            return s;

        }

        protected bool WaitStatusBit(byte Flag, int Timeout_ms)
        {
            NationalInstruments.NI4882.SerialPollFlags bStatus = default(NationalInstruments.NI4882.SerialPollFlags);
            DateTime tStart = default(DateTime);
            double tSpan = 0;

            //wait until data is available
            tStart = DateTime.Now;
            while (((Convert.ToByte(bStatus) & Flag) != Flag) & (tSpan < Timeout_ms))
            {
                //take a nap
                System.Threading.Thread.Sleep(100);
                //check status
                bStatus = mGPIB.SerialPoll();
                //check time
                tSpan = DateTime.Now.Subtract(tStart).TotalMilliseconds;
            }

            return ((Convert.ToByte(bStatus) & Flag) == Flag);
        }

        protected bool ReadBinaryData(string sCmd, ref byte[] Data)
        {
            //setup binary read
            if (!this.SetupBinaryRead(sCmd))
                return false;
            //read data to file
            Data = mGPIB.ReadByteArray();
            return true;
        }

        protected bool ReadBinaryToFile(string sCmd, string sFile)
        {
            //setup binary read
            if (!SetupBinaryRead(sCmd))
                return false;
            //read data to file
            mGPIB.ReadToFile(sFile);
            return true;
        }

        private bool SetupBinaryRead(string sCmd)
        {
            int i = 0;
            string s = null;

            //send cmd
            mGPIB.Write(sCmd);

            //parse data using IEEE 488.2 binary format
            //read leading "#"
            s = mGPIB.ReadString(1);
            if (s != "#")
                return false;
            //read the first digit, which is the length of the following size string
            s = mGPIB.ReadString(1);
            if (!int.TryParse(s, out i))
                return false;
            //read the size string, this is the size (number of bytes) of the binary data followed
            s = mGPIB.ReadString(i);
            if (!int.TryParse(s, out i))
                return false;
            //set the buffer size
            mGPIB.DefaultBufferSize = i;
            //return
            return true;
        }
        #endregion

    }
    /// <summary>
    /// RS232 interface
    /// </summary>
    /// <remarks></remarks>
    public abstract class iRS232Generic : iInstrumentGeneric
    {


        protected System.IO.Ports.SerialPort mPort;
        public abstract bool Initialize(string sPort, int BaudRate, bool RaiseError);

        public bool Initialize(string sPort, int BuadRate)
        {
            return this.Initialize(sPort, BuadRate, false);
        }

        public override bool Initialize(int COMAdrs, int BaudRate, bool RaiseError)
        {
            return this.Initialize("COM" + COMAdrs, BaudRate, RaiseError);
        }


        public override abstract string Name { get; }

        public override abstract string QueryString(string sCmd);
        public override abstract double QueryValue(string sCmd);
        public override abstract void SendCmd(string sCmd);

        public override bool Connected
        {
            get
            {
                if (mPort == null)
                {
                    return false;
                }
                else
                {
                    return mPort.IsOpen;
                }
            }
            set
            {
                if (mPort == null)
                    return;
                if (value)
                {
                    if (!mPort.IsOpen)
                        mPort.Open();
                }
                else
                {
                    mPort.Close();
                }
            }
        }

        #region "shared functions"

        private static System.Comparison<string> mSorter = ComPortSorter;
        private static int ComPortSorter(string x, string y)
        {
            System.Collections.CaseInsensitiveComparer cpr = new System.Collections.CaseInsensitiveComparer();

            if (x.StartsWith("COM") & x.StartsWith("COM"))
            {
                double Vx = Convert.ToDouble(x.Substring(3));
                double Vy = Convert.ToDouble(y.Substring(3));
                return cpr.Compare(Vx, Vy);
            }
            else
            {
                return cpr.Compare(x, y);
            }
        }

        public static string[] PortNamesAll
        {
            get
            {

                string[] sData = SerialPort.GetPortNames();

                Array.Resize(ref sData, sData.Count());
                //Microsoft.VisualBasic.Devices.Computer.Ports.SerialPortNames.CopyTo(sData, 0);
                    
                Array.Sort(sData, mSorter);
                return sData;
            }
        }

        public static string[] PortNamesAvailable
        {
            get
            {
                List<string> sList = new List<string>();
                System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort();

                foreach (string s in PortNamesAll)
                {
                    port.PortName = s;
                    try
                    {
                        port.Open();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    sList.Add(s);
                    port.Close();
                }

                string[] sData = new string[sList.Count];

                sList.CopyTo(sData, 0);
                return sData;
            }
        }

        #endregion
    }

}

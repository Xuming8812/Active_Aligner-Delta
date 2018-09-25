using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace Instrument
{
    public class iOC9501LDD : iMultiChannelLDD
    {
        private SerialPort mPort;
        public iOC9501LDD(int ChannelCount, double DefaultCurrent) : base(ChannelCount, DefaultCurrent)
        {
        }

        public override bool Initialize(string sPort, bool RaiseError)
        {
            //double v = 0;
            bool b = false;
            int i = 0;

            //set serial port
            mPort = new SerialPort(sPort, 9600, Parity.None, 8, StopBits.One);

            //open port
            try
            {
                mPort.Open();
                mPort.Handshake = Handshake.None;
                mPort.WriteTimeout = 1000;
                mPort.ReadTimeout = 5000;

                //get protection state
                b = this.EnabledProtectionState;
                if (b)
                    this.EnabledProtectionState = false;

            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, "OC9501 LDD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;

            }

            //test
            //Me.EnabledProtectionState = True
            //b = Me.EnabledProtectionState()

            //enable TEC
            this.TECEnabled = true;

            System.Threading.Thread.Sleep(2000);

            //try to read temperature
            //v = Me.TemperatureReading

            for (i = 0; i <= mChannelCount - 1; i++)
            {
                mCurrent[i]= this.ReadCurrent(i + 1);
            }

            return true;
        }

        #region "serial communication"
        private byte[] QueryData(byte CmdGroup, byte CmdID, byte[] bData)
        {
            byte[] data = null;
            byte[] NewData = null;
            int len = 0;

            try
            {
                this.SendCmd(CmdGroup, CmdID, bData);

                data = new byte[4];
                mPort.Read(data, 0, 4);

                len = data[2];
                len = len << 8;
                len += data[3];
                len += 2;
                //2 is for CRC

                data = new byte[len];
                mPort.Read(data, 0, len);

                NewData = new byte[len - 14];
                Array.Copy(data, 12, NewData, 0, NewData.Length);

                return NewData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void SendCmd(byte CmdGroup, byte CmdID, byte[] bData)
        {
            System.Text.ASCIIEncoding e = new System.Text.ASCIIEncoding();
            int i = 0;
            int ii = 0;
            int length = 0;
            byte[] Data = null;
            Delta.CheckSum x = new Delta.CheckSum();

            //build command, "data" length does not include Pre-Code, lengh, and CRC
            ii = bData.Length;
            length = 18 + ii;
            Data = new byte[length];

            //pre code - RS232
            Data[0] = 0xf6;
            Data[1] = 0x28;
            //length
            Data[2] = Convert.ToByte((length & 0xff00) >> 8);
            Data[3] = Convert.ToByte(length & 0xff);
            //cmd type
            Data[4] = CmdGroup;
            //cmd ID
            Data[5] = CmdID;
            //client ID
            Data[6] = 0x0;
            //status
            Data[7] = 0x0;
            //serial ID
            Data[8] = 0x12;
            Data[9] = 0x34;
            Data[10] = 0x56;
            Data[11] = 0x78;
            //dummy
            Data[12] = 0x0;
            Data[13] = 0x0;
            Data[14] = 0x0;
            Data[15] = 0x0;
            //data
            for (i = 0; i <= ii - 1; i++)
            {
                Data[16 + i] = bData[i];
            }

            //CRC, null them first, so that it will not affect the CRC
            Data[16 + ii] = 0x0;
            Data[17 + ii] = x.CRC8Ex(Data, Data.Length - 2);

            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();
            mPort.Write(Data, 0, Data.Length);

            //delay
            System.Threading.Thread.Sleep(50);
        }

        private int QueryData(byte CmdID, int Channel)
        {
            byte[] data = null;
            int i = 0;

            data = this.QueryData(0x4, CmdID, new byte[] { Convert.ToByte(Channel) });
            if (data.Length == 1)
            {
                return data[0];
            }
            else
            {
                //for this command, the first byte is channel, the 2 following byte is data
                if (Channel != data[1])
                    MessageBox.Show("Data Error");
                i = data[1];
                i = (i << 8);
                i = i + data[2];
                return i;
            }

        }

        private int QueryData(byte CmdID)
        {
            byte[] data = null;
            int i = 0;

            data = this.QueryData(0x4, CmdID, new byte[999]);
            if (data.Length == 1)
            {
                return data[0];
            }
            else
            {
                i = data[0];
                i = (i << 8);
                i = i + data[i];
                return i;
            }
        }

        private void SendCmd(byte CmdID, int Channel, int Data)
        {
            byte[] bData = new byte[3];

            bData[0] = Convert.ToByte(Channel);
            bData[1] = Convert.ToByte((Data & 0xff00) >> 8);
            bData[2] = Convert.ToByte(Data & 0xff);

            this.SendCmd(0x4, CmdID, bData);
        }

        private void SendCmd(byte CmdID, int Data)
        {
            byte[] bData = new byte[2];

            bData[0] = Convert.ToByte((Data & 0xff00) >> 8);
            bData[1] = Convert.ToByte(Data & 0xff);

            this.SendCmd(0x4, CmdID, bData);
        }
        #endregion

        public override bool EnabledProtectionState
        {
            get
            {
                byte[] data = null;
                data = this.QueryData(0x2, 0x8, new byte[] { });
                return data[0] == 0;
            }

            set
            {
                byte[] data = new byte[1];
                if (value)
                {
                    data[0] = 0;
                }
                else
                {
                    data[0] = 1;
                }
                this.SendCmd(0x2, 0x7, data);

                System.Threading.Thread.Sleep(2000);
            }
        }

        public override double ReadCurrent(int channel)
        {
            int data = 0;
            data = this.QueryData(0xd, channel);
            return 0.01 * data;
        }

        public override bool SetCurrent(int channel, double value)
        {
            int data = 0;
            data = Convert.ToInt32(100.0 * value);
            this.SendCmd(0xe, channel, data);
            return true;
        }

        public override double TECCurrent
        {
            get
            {
                int data = 0;
                data = this.QueryData(0x14);
                return 0.1 * data;
            }
        }

        private bool mEnabled;

        public override bool TECEnabled
        {
            get { return mEnabled; }
            set
            {
                byte b = 0;
                mEnabled = value;
                b = 0;
                if (value)
                    b = 1;
                this.SendCmd(0x4, 0x1, new byte[] { b });
            }
        }

        public override double TECVoltage
        {
            get
            {
                int data = 0;
                data = this.QueryData(0x15);
                return 0.001 * data;
            }
        }

        public override double TemperatureReading
        {
            get
            {
                int data = 0;
                data = this.QueryData(0x4);

                return 0.01 * data;
            }
        }

        public override double TemperatureSetpoint
        {
            get
            {
                int data = 0;
                data = this.QueryData(0x2);
                return 0.01 * data;
            }
            set
            {
                int data = 0;
                data = Convert.ToInt32(100.0 * value);
                this.SendCmd(0x3, data);
            }
        }

        public override double ReadVcrossing(int channel)
        {
            int data = 0;
            data = this.QueryData(0x9, channel);
            return 0.001 * data;
        }

        public override bool SetVcrossing(int channel, double value)
        {
            int data = 0;
            data = Convert.ToInt32(1000.0 * value);
            this.SendCmd(0xa, channel, data);
            return true;
        }

        public override double  ReadVoltage(int channel)
        {
                int data = 0;
                data = this.QueryData(0xf, channel);
                return 0.001 * data;
        }

        public override bool SetVoltage(int channel, double value)
        {
            int data = 0;
            data = Convert.ToInt32(1000.0 * value);
            this.SendCmd(0xa, channel, data);
            return true;
        }
    }

    public abstract class iMultiChannelLDD
    {
        protected int mChannelCount;
        protected double[] mCurrent;

        internal int Address;
        public iMultiChannelLDD(int ChannelCount, double DefaultCurrent)
        {
            int i = 0;

            mChannelCount = ChannelCount;

            mCurrent = new double[mChannelCount];
            for (i = 0; i <= mChannelCount - 1; i++)
            {
                mCurrent[i] = DefaultCurrent;
            }
        }

        public int ChannelCount
        {
            get { return mChannelCount; }
        }

        public abstract bool Initialize(string sPort, bool RaiseError);

        public abstract bool EnabledProtectionState { get; set; }

        public abstract double ReadCurrent(int channel);
        public abstract bool SetCurrent(int channel, double value);

        public abstract double ReadVoltage(int channel);
        public abstract bool SetVoltage(int channel,double value);

        public abstract double ReadVcrossing(int channel);
        public abstract bool SetVcrossing(int channel,double value);

        public abstract bool TECEnabled { get; set; }
        public abstract double TECCurrent { get; }
        public abstract double TECVoltage { get; }

        public abstract double TemperatureSetpoint { get; set; }
        public abstract double TemperatureReading { get; }


        public void SetSingleChannelCurrent(int Channel, double Current)
        {
            bool b = false;
            b = this.EnabledProtectionState;
            if (b)
            {
                this.EnabledProtectionState = false;
                System.Threading.Thread.Sleep(1000);
            }


            if (Channel == 0)
                Channel = 2;
            mCurrent[Channel - 1] = Current;
            this.TurnSingleChannelOn(Channel);
        }

        public void TurnSingleChannelOn(int Channel)
        {

            bool b = false;
            b = this.EnabledProtectionState;
            if (b)
            {
                this.EnabledProtectionState = false;
                System.Threading.Thread.Sleep(1000);
            }

            mCurrent[Channel - 1] = 85;

            int i = 0;
            //set current, do this first, the zeroing of other channel acts as a delay fot this set channel
            this.SetCurrent(Channel, mCurrent[Channel - 1] ) ;
            //zero other channels
            for (i = 1; i <= mChannelCount; i++)
            {
                if (i != Channel)
                {
                    //zero setting
                    //this.Current[i] = 0.0;
                    this.SetCurrent(Channel, 0.0);
                }
                //read back
                //v(i - 1) = Me.Current(i)
            }

        }

    }
}

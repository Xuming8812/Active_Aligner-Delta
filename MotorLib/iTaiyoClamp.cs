using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Instrument
{
    public class iTaiyoClamp
    {
        private  SerialPort mPort;
        private byte mSubAdrs;
        int mStatus;

        public enum ErrorMask
        {
            InOperation = 0x1,
            PositionOutOfRange = 0x2,
            ServoOff = 0x4,
            CommandError = 0x20,
            Alarm = 0x40,
            CommunicationError = 0x80
        }

        public enum StatusMask
        {
            Ready = 0x1,
            Busy = 0x2,
            Alarm = 0x4,
            InPosition = 0x8,
            Hold = 0x10,
            RunLED = 0x20,
            ReadyLED = 0x40,
            AlarmLED = 0x80
        }


        public iTaiyoClamp(int AdrsDevice)
        {
            mSubAdrs = Convert.ToByte(AdrsDevice);
        }

        public bool Initialize(string sPort, bool RaiseError)
        {

            //set serial port
            mPort = new  SerialPort(sPort, 9600,  Parity.Even, 8,  StopBits.One);
            //byte[] DataOut = new byte[99];
            //open port
            try
            {
                mPort.Open();
                mPort.Handshake =  Handshake.None;
                mPort.WriteTimeout = 300;
                mPort.ReadTimeout = 1000;

                byte[] DataOut = null;
                //test NOP
                this.SendCmd(0x30, new byte[] { }, ref DataOut);

                if (mStatus == 0)
                {
                    return true;
                }
                else
                {
                    if (RaiseError)
                        MessageBox.Show("Cannot estabilish communication with Taiyo controller.", "Taiyo Controller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mPort.IsOpen)
                        mPort.Close();
                    return false;
                }

            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, "Taiyo Controller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;
            }

        }

        public bool Enabled
        {
            get
            {
                byte[] data = new byte[1] { 0x0};
                this.SendCmd(0x46, new byte[] { }, ref data);
                return (data[0] == 1);
            }
            set
            {
                byte[] data = new byte[1] { 0x0 };
                byte[] DataOut = null;
                data[0] = 0;
                if (value)
                    data[0] = 1;
                this.SendCmd(0x31, data, ref DataOut);
            }
        }

        public double Position
        {
            get
            {
                byte[] data = new byte[1] { 0x0 };

                int i = 0;
                this.SendCmd(0x41, new byte[] { }, ref data);
                i = BitConverter.ToInt32(data, 0);
                return 0.01 * i;
            }
        }

        public bool StopMotion()
        {
            byte[] DataOut = null;
            return this.SendCmd(0x10, new byte[] { }, ref DataOut);
        }

        public bool ORG()
        {
            byte[] DataOut = null;
            return this.SendCmd(0x11, new byte[] { }, ref DataOut);
        }

        public bool Move(double Target, double Speed)
        {
            byte[] Data = new byte[5] { 0x0, 0x0, 0x0, 0x0, 0x0 };
            byte[] DataOut = null;
            if (Target < 0)
                Target = 0;
            Int32 intTarget = Convert.ToInt32(100 * Target);
            Data[3] = Convert.ToByte(intTarget >> 24);
            Data[2] = Convert.ToByte((intTarget & 0xff0000) >> 16);
            Data[1] = Convert.ToByte((intTarget & 0xff00) >> 8);
            Data[0] = Convert.ToByte(intTarget & 0xff);
            Data[4] = Convert.ToByte(Speed);

            return this.SendCmd(0x17, Data, ref DataOut);
        }

        public bool MoveRelative(double Target, double Speed)
        {
            byte[] Data = new byte[5] { 0x0, 0x0, 0x0, 0x0, 0x0 };
            byte[] DataOut = null;
            Int32 intTarget = Convert.ToInt32(100 * Math.Abs(Target));

            Data[0] = Convert.ToByte(intTarget & 0xff);
            Data[1] = Convert.ToByte((intTarget & 0xff00) >> 8);
            Data[2] = Convert.ToByte((intTarget & 0xff0000) >> 16);
            Data[3] = Convert.ToByte(intTarget >> 24);
            Data[4] = Convert.ToByte(Speed);
            return this.SendCmd(0x16, Data, ref DataOut);

        }

        public bool MoveWithForce(double Target, double Speed, double Force)
        {
            byte[] Data = new byte[6] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            if (Target < 0)
                Target = 0;
            Int32 intTarget = Convert.ToInt32(100 * Target);
            Data[3] = Convert.ToByte(intTarget >> 24);
            Data[2] = Convert.ToByte((intTarget & 0xff0000) >> 16);
            Data[1] = Convert.ToByte((intTarget & 0xff00) >> 8);
            Data[0] = Convert.ToByte(intTarget & 0xff);
            Data[4] = Convert.ToByte(Convert.ToInt16(Speed * 100));
            Data[5] = Convert.ToByte(Convert.ToInt16(Force * 100));
            byte[] DataOut = null;
            return this.SendCmd(0x23, Data, ref DataOut);
        }

        public bool MoveRelativeWithForce(double Target, double Speed, double Force)
        {
            byte[] Data = new byte[6] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            Int32 intTarget = Convert.ToInt32(100 * Target);
            Data[3] = Convert.ToByte(intTarget >> 24);
            Data[2] = Convert.ToByte((intTarget & 0xff0000) >> 16);
            Data[1] = Convert.ToByte((intTarget & 0xff00) >> 8);
            Data[0] = Convert.ToByte(intTarget & 0xff);
            Data[4] = Convert.ToByte(Convert.ToInt16(Speed * 100));
            Data[5] = Convert.ToByte(Convert.ToInt16(Force * 100));
            byte[] DataOut = null;
            return this.SendCmd(0x22, Data, ref DataOut);
        }

        public bool OpenGrip(double Speed, double Force)
        {
            byte[] Data = new byte[2];
            byte[] DataOut = null;
            Data[0] = Convert.ToByte(Speed);
            Data[1] = Convert.ToByte(Force);
            return this.SendCmd(0x20, Data, ref DataOut);
        }

        public bool CloseGrip(double Speed, double Force)
        {
            byte[] Data = new byte[2];
            byte[] DataOut = null;
            Data[0] = Convert.ToByte(Speed);
            Data[1] = Convert.ToByte(Force);
            return this.SendCmd(0x21, Data, ref DataOut);
        }

        public bool ClampBusy
        {
            get
            {
                byte[] data = new byte[3] { 0x0,0x0,0x0};
                this.SendCmd(0x52, new byte[] { }, ref data);
                return (data[2] & Convert.ToByte(StatusMask.Busy)) == Convert.ToByte(StatusMask.Busy);
            }
        }

        #region "serial communication"
        public int ErrorCode
        {
            get { return mStatus; }
        }

        private bool SendCmd(byte register, byte[] DataOut, ref byte[] DataIn)
        {
            int i = 0;
            int length = 0;
            int checksum = 0;
            byte[] Cmd = null;
            if (DataOut == null)
            {
                length = 4;
            }
            else
            {
                length = DataOut.Length + 4;
            }



            Cmd = new byte[length];

            //build command
            Cmd[0] = Convert.ToByte(length);
            Cmd[1] = mSubAdrs;
            Cmd[2] = register;

            checksum = Cmd[0] + Cmd[1] + Cmd[2];
            for (i = 0; i <= DataOut.Length - 1; i++)
            {
                Cmd[i + 3] = DataOut[i];
                checksum += DataOut[i];
            }
            Cmd[length - 1] = Convert.ToByte(checksum & 0xff);

            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();

            mPort.Write(Cmd, 0, length);

            //delay
            for (i = 0; i <= 99; i++)
            {
                System.Threading.Thread.Sleep(50);
                if (mPort.BytesToRead >= 4)
                    break; // TODO: might not be correct. Was : Exit For
            }

            //read the rest of the data
            length = mPort.BytesToRead;
            //Cmd = new byte[length];

            System.Array.Resize<byte>(ref DataIn, length);
            mPort.Read(Cmd, 0, length);

            //get data out
            mStatus = Cmd[2];
            if (length > 4)
            {
                DataIn = new byte[length - 4];
                for (i = 0; i <= length - 5; i++)
                {
                    DataIn[i] = Cmd[i + 3];
                }
            }

            return (mStatus & ((int)ErrorMask.CommandError + (int)ErrorMask.CommunicationError)) == 0;
        }

        #endregion
    }
}

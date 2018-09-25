using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.IO.Ports;
using System.Windows.Forms;
using Delta;


    namespace Instrument
    {
        

        public class iFUWO : iUvCure
        {

            public enum ChannelEnum
            {
                Channel1 = 1,
                Channel2 = 2,
                Channel3 = 3,
                Channel4 = 4,
                ChannelAll = 0xff
            }


            private SerialPort mPort;
            public ChannelEnum ActiveChannel { get; set; }

            public iFUWO(ChannelEnum iActiveChannel)
            {
                this.ActiveChannel = iActiveChannel;
            }

            public override bool Initialize(string sPort, bool RaiseError)
            {
                bool success = false;
                byte[] dataSent = null;
                byte[] DataReceived = null;

                //set serial port
                mPort = new SerialPort(sPort, 9600, Parity.None, 8, StopBits.One);

                //open port
                try
                {
                    mPort.Open();
                    mPort.Handshake = Handshake.None;
                    mPort.WriteTimeout = 300;
                    mPort.ReadTimeout = 1000;

                    //establish communication and set exposure to auto program 
                    dataSent = new byte[] {
                    0x20,
                    0x4
                };
                    //DataReceived = new byte[];
                    success = this.SendCmd(dataSent, ref DataReceived);
                    if (success)
                        success = (DataReceived[0] == 0x4);

                    //pick program A
                    dataSent = new byte[] {
                    0x8,
                    0x1
                };
                    //DataReceived = new byte[];
                    success = this.SendCmd(dataSent, ref DataReceived);
                    if (success)
                        success = (DataReceived[0] == 0x1);

                    if (success)
                    {
                        return true;

                    }
                    else
                    {
                        if (RaiseError)
                            MessageBox.Show("Cannot estabilish communication with FUWO.", "FUWO UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (mPort.IsOpen)
                            mPort.Close();
                        return false;
                    }

                }
                catch (Exception e)
                {
                    if (RaiseError)
                        MessageBox.Show(e.Message, "FUWO UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mPort.IsOpen)
                        mPort.Close();
                    return false;

                }

            }

            public override void Close()
            {
                mPort.Close();
            }

            protected override void StartExposure()
            {
                this.ShutterOpen = true;
            }

            protected override bool ExposureRunning
            {
                get { return this.ShutterOpen; }
            }

            public override bool ShutterOpen
            {
                get
                {
                    byte[] data = null;
                    this.SendCmd(new byte[] {0x0, 0xff}, ref data);
                    switch (this.ActiveChannel)
                    {
                        case ChannelEnum.Channel1:
                        case ChannelEnum.ChannelAll:
                            return data[0] == 1;
                        case ChannelEnum.Channel2:
                            return data[1] == 1;
                        case ChannelEnum.Channel3:
                            return data[2] == 1;
                        case ChannelEnum.Channel4:
                            return data[3] == 1;
                        default:
                            return false;
                    }
                }
                set
                {
                    byte[] data = null;
                    if (value)
                    {
                        this.SendCmdWithChannelInfo(0x2, ref data);
                    }
                    else
                    {
                        this.SendCmd(new byte[] {
                        0x6,
                        0xff
                    }, ref data);
                    }
                }
            }

            #region "serial communication"
            private bool SendCmdWithChannelInfo(byte cmd, ref byte[] DataIn)
            {
                byte[] data = new byte[2];
                data[0] = cmd;
                data[1] = Convert.ToByte(this.ActiveChannel);
                return this.SendCmd(data, ref DataIn);
            }

            private bool SendCmd(byte[] DataOut, ref byte[] DataIn)
            {
                int i = 0;
                int length = 0;
                int checksum = 0;
                byte[] Cmd = null;

                length = DataOut.Length + 4;
                Cmd = new byte[length];

                //build command
                Cmd[0] = 0x3c;
                Cmd[1] = Convert.ToByte(DataOut.Length + 1);
                //includes the check sum

                checksum = 0;
                for (i = 0; i <= DataOut.Length - 1; i++)
                {
                    Cmd[i+2] = DataOut[i];
                    checksum += DataOut[i];
                }
                Cmd[length - 2] = Convert.ToByte(checksum & 0xff);
                Cmd[length - 1] = 0xd;

                //send data
                mPort.DiscardInBuffer();
                mPort.DiscardOutBuffer();

                mPort.Write(Cmd, 0, length);

                //delay
                System.Threading.Thread.Sleep(20);

                //read data back
                mPort.Read(Cmd, 0, 2);
                //cmd[1] contains the length of the returnd data
                length = Cmd[1];
                mPort.Read(Cmd, 0, length);

                //cmd now contains the checksum and ending code, we will ignore them, no checksum check
                //but we will check the return command data, it should be want is sent + 1
                if (Cmd[0] == DataOut[0] + 1)
                {
                    length = length - 3;
                    //no cmd, no checksum, no ending type
                    DataIn = new byte[length];
                    for (i = 0; i <= length - 1; i++)
                    {
                        DataIn[i] = Cmd[i + 1];
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

            public override bool AlarmOn
            {
                get { return false; }
            }

            protected override bool CheckAlarm(bool CheckCalibration)
            {
                return true;
            }

            public override bool ExposureFailed
            {
                get { return false; }
            }

            public override double ExposureTime
            {
                get
                {
                    byte[] data = null;
                    this.SendCmdWithChannelInfo(0xe, ref data);
                    return 0.1 * BitConverter.ToInt32(data, 0);
                }
                set
                {
                    byte[] data = null;
                    byte[] NewData = null;
                    int x = 0;

                    this.SendCmdWithChannelInfo(0xe, ref data);
                    x = Convert.ToInt32(10 * value);

                    NewData = new byte[7];
                    NewData[0] = 0xc;
                    NewData[1] = Convert.ToByte(x & 0xff);
                    NewData[2] = Convert.ToByte((x >> 8) & 0xff);
                    NewData[3] = Convert.ToByte((x >> 16) & 0xff);
                    NewData[4] = Convert.ToByte((x >> 24) & 0xff);
                    NewData[5] = data[4];
                    NewData[6] = data[5];

                    this.SendCmd(NewData, ref data);
                }
            }

            public override int ActiveSingleChannel
            {
                get { return (int)ActiveChannel; }
                set { ActiveChannel = (ChannelEnum)value; }
            }

            public override double PowerLevel
            {
                get
                {
                    byte[] data = null;
                    this.SendCmdWithChannelInfo(0xe, ref data);
                    return data[4];
                }
                set
                {
                    byte[] data = null;
                    byte[] NewData = null;
                    byte x = 0;

                    this.SendCmdWithChannelInfo(0xe, ref data);
                    x = Convert.ToByte(value);

                    NewData = new byte[7];
                    NewData[0] = 0xc;
                    NewData[1] = data[0];
                    NewData[2] = data[1];
                    NewData[3] = data[2];
                    NewData[4] = data[3];
                    NewData[5] = x;
                    NewData[6] = data[5];

                    this.SendCmd(NewData, ref data);
                }
            }

            public override bool LampOn
            {
                get { return true; }
                //do nothing
                set { }
            }

            public override bool LampReady
            {
                //FUWO does not have RS232 command for this, it has a digit output
                get { return true; }
            }

            public override bool NeedCalibration
            {
                get { return false; }
            }
        }

        public class iFUTANSI : iUvCure
        {


            private SerialPort mPort;
            public enum ChannelEnum
            {
                Channel1 = 1,
                Channel2 = 2,
                Channel3 = 3,
                Channel4 = 4,
                ChannelAll = 0xff
            }

            public ChannelEnum ActiveChannel { get; set; }

            public override int ActiveSingleChannel
            {
                get { return (int)ActiveChannel; }
                set { ActiveChannel = (ChannelEnum)value; }
            }

            public iFUTANSI(ChannelEnum iActiveChannel)
            {
                this.ActiveChannel = iActiveChannel;
            }

            public string forestring;

            public string backstring;

            public string controlString;
            public override bool Initialize(string sPort, bool RaiseError)
            {
                //set serial port
                mPort = new SerialPort(sPort, 9600,Parity.None, 8, StopBits.One);

                //open port
                try
                {
                    mPort.Open();
                    mPort.Handshake = Handshake.None;
                    mPort.WriteTimeout = 300;
                    mPort.ReadTimeout = 1000;

                    //establish communication and set exposure to auto program 
                    string s = null;
                    int i = 0;
                    for (i = 0; i <= 10; i++)
                    {
                        s = this.QueryString("FUTANSI");
                        if (s.Length > 0)
                        {
                            this.SendCmd(15);
                            return true;
                        }
                        else
                        {
                            byte cmd = 0;
                            cmd = 1;
                            s = this.QueryString(cmd);
                            if (s.Length > 0)
                            {
                                this.SendCmd(15);
                                return true;
                            }
                        }
                    }

                    if (RaiseError)
                        MessageBox.Show("Cannot estabilish communication with Futansi.", "Futansi UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mPort.IsOpen)
                        mPort.Close();
                    return false;

                }
                catch (Exception e)
                {
                    if (RaiseError)
                        MessageBox.Show(e.Message, "Futansi UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mPort.IsOpen)
                        mPort.Close();
                    return false;

                }
            }

            public override bool AlarmOn
            {

                get { return false; }
            }

            protected override bool CheckAlarm(bool CheckCalibration)
            {
                return false;
            }

            public override void Close()
            {
                mPort.Close();
            }

            public override bool ExposureFailed
            {

                get { return false; }
            }

            protected override bool ExposureRunning
            {

                get { return false; }
            }

            public override double ExposureTime
            {

                get { return 0.0; }
                set
                {
                    if (value >= 100)
                    {
                        backstring = "A " + Convert.ToString(value) + ".0S!";
                    }
                    else if (value > 9)
                    {
                        backstring = "A 0" + Convert.ToString(value) + ".0S!";
                    }
                    else
                    {
                        backstring = "A 00" + Convert.ToString(value) + ".0S!";
                    }

                }
            }
            public override bool LampOn { get; set; }

            public override bool LampReady
            {

                get { return true; }
            }

            public override bool NeedCalibration
            {

                get { return false; }
            }

            public override double PowerLevel
            {

                get { return 0.0; }
                set
                {
                    if (value >= 100)
                    {
                        forestring = Convert.ToString(value) + "%";
                    }
                    else if (value > 9)
                    {
                        forestring = "0" + Convert.ToString(value) + "%";
                    }
                    else
                    {
                        forestring = "00" + Convert.ToString(value) + "%";
                    }

                }
            }
            public override bool ShutterOpen
            {

                get { return false; }
                set
                {
                    if (value)
                    {
                        switch (ActiveChannel)
                        {
                            case ChannelEnum.Channel1:
                                controlString = "CH1 " + forestring + backstring;

                                break;
                            case ChannelEnum.Channel2:
                                controlString = "CH2 " + forestring + backstring;
                                break;
                            case ChannelEnum.Channel3:
                                controlString = "CH3 " + forestring + backstring;
                                break;
                            case ChannelEnum.Channel4:
                                controlString = "CH4 " + forestring + backstring;
                                break;
                        }
                        string s = null;
                        s = this.QueryString(controlString);
                        StopExposure();
                        StartExposure();
                    }
                    else
                    {
                        StopExposure();
                    }
                }
            }

            protected override void StartExposure()
            {
                byte cmd = 0;
                switch (ActiveChannel)
                {
                    case ChannelEnum.Channel1:
                        cmd = Convert.ToByte(Math.Pow(2, 4) + Math.Pow(2, 0));
                        break;
                    case ChannelEnum.Channel2:
                        cmd = Convert.ToByte(Math.Pow(2, 4) + Math.Pow(2, 1));
                        break;
                    case ChannelEnum.Channel3:
                        cmd = Convert.ToByte(Math.Pow(2, 4) + Math.Pow(2, 2));
                        break;
                    case ChannelEnum.Channel4:
                        cmd = Convert.ToByte(Math.Pow(2, 4) + Math.Pow(2, 3));
                        break;
                    case ChannelEnum.ChannelAll:
                        cmd = 31;
                        break;
                }
                System.Threading.Thread.Sleep(500);
                this.SendCmd(cmd);
            }

            protected void StopExposure()
            {
                byte cmd = 0;
                switch (ActiveChannel)
                {
                    case ChannelEnum.Channel1:
                        cmd = Convert.ToByte(Math.Pow(2, 0));
                        break;
                    case ChannelEnum.Channel2:
                        cmd = Convert.ToByte(Math.Pow(2, 1));
                        break;
                    case ChannelEnum.Channel3:
                        cmd = Convert.ToByte(Math.Pow(2, 2));
                        break;
                    case ChannelEnum.Channel4:
                        cmd = Convert.ToByte(Math.Pow(2, 3));
                        break;
                    case ChannelEnum.ChannelAll:
                        cmd = 15;
                        break;
                }

                this.SendCmd(cmd);
            }

            #region "serial communication"
            private bool SendCmd(byte DataOut)
            {
                byte[] Cmd = new byte[1];
                Cmd[0] = DataOut;
                //send data
                mPort.DiscardInBuffer();
                mPort.DiscardOutBuffer();
                mPort.Write(Cmd, 0, 1);

                Cmd[0] = 33;
                mPort.Write(Cmd, 0, 1);

                System.Threading.Thread.Sleep(20);

                return true;
            }

            private string QueryString(string sCmd)
            {
                string s = null;
                //clear buffer
                mPort.DiscardInBuffer();
                mPort.ReadExisting();
                System.Threading.Thread.Sleep(100);
                //send command
                this.SendCmd(sCmd);
                System.Threading.Thread.Sleep(300);
                //read data
                s = mPort.ReadExisting();
                //remove known characters
                s = s.Replace(sCmd, "");
                s = s.Replace(ControlChars.Cr, Convert.ToChar(""));
                return s;
            }

            private string QueryString(byte sCmd)
            {
                string s = null;
                //clear buffer
                mPort.DiscardInBuffer();
                mPort.ReadExisting();
                System.Threading.Thread.Sleep(100);
                //send command
                this.SendCmd(sCmd);
                System.Threading.Thread.Sleep(300);
                //read data
                s = mPort.ReadExisting();
                //remove known characters
                s = s.Replace(Convert.ToString(sCmd), "");
                s = s.Replace(ControlChars.Cr, Convert.ToChar(""));
                return s;
            }

            private void SendCmd(string sCmd)
            {
                string s = null;

                mPort.DiscardOutBuffer();
                s = "*" + sCmd + ControlChars.Cr;
                mPort.Write(s);

                System.Threading.Thread.Sleep(100);
            }
            #endregion
        }

        //public class iLamplic : iUvCure
        //{

        //    public enum ChannelEnum
        //    {
        //        Channel1 = 0,
        //        Channel2 = 1,
        //        Channel3 = 2,
        //        Channel4 = 3,
        //        ChannelAll = 0xff
        //    }

        //    public enum ECmd
        //    {
        //        SetParameters = 1,
        //        GetParameters,
        //        EnableBeep,
        //        EnablePedalMode,
        //        EnableKeypadMode,
        //        GetWorkMode,
        //        GetCureTime,
        //        EnableAllChannelsCure,
        //        OnOffSingleChannelCure,
        //        EnableChannel,
        //        SetAddress
        //    }

        //    public enum ECureType
        //    {
        //        ConstantPower,
        //        StepPower
        //    }

        //    public struct UvParameterStructure
        //    {
        //        public ECureType CureType;
        //        public int StepCount;
        //        public double[] CureTime;
        //        public int[] CurePower;
        //    }

        //    private SerialPort mPort;

        //    private int mAddress;
        //    public ChannelEnum ActiveChannel { get; set; }

        //    public iLamplic(ChannelEnum iActiveChannel)
        //    {
        //        this.ActiveChannel = iActiveChannel;
        //    }

        //    public override bool Initialize(string sPort, bool RaiseError)
        //    {
        //        //Dim success As Boolean

        //        //set serial port
        //        mPort = new SerialPort(sPort, 9600,  Parity.None, 8,  StopBits.One);

        //        //open port
        //        try
        //        {
        //            mPort.Open();
        //            mPort.Handshake =  Handshake.None;
        //            mPort.WriteTimeout = 300;
        //            mPort.ReadTimeout = 1000;

        //            this.Address = 1;
        //            System.Threading.Thread.Sleep(200);
        //            UvParameterStructure para = default(UvParameterStructure);
        //            para = UVParameter[this.ActiveChannel];
        //            System.Threading.Thread.Sleep(200);
        //            para.StepCount = 1;
        //            para.CureTime[0] = 10;
        //            para.CurePower[0] = 40;
        //            this.UVParameter[this.ActiveChannel] = para;

        //            para = UVParameter[1];
        //            System.Threading.Thread.Sleep(200);
        //            para.StepCount = 1;
        //            para.CureTime[0] = 10;
        //            para.CurePower[0] = 40;
        //            this.UVParameter[1] = para;




        //            System.Threading.Thread.Sleep(200);
        //            para = this.UVParameter[this.ActiveChannel];
        //            System.Threading.Thread.Sleep(200);
        //            for (int i = 1; i <= 4; i++)
        //            {
        //                this.EnableChannel[this.ActiveChannel] = false;
        //                System.Threading.Thread.Sleep(200);
        //            }

        //            if (para.StepCount == 1)
        //            {
        //                return true;

        //            }
        //            else
        //            {
        //                if (RaiseError)
        //                    MessageBox.Show("Cannot estabilish communication with Lamplic.", "Lamplic UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                if (mPort.IsOpen)
        //                    mPort.Close();
        //                return false;
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            if (RaiseError)
        //                MessageBox.Show(e.Message, "Lamplic UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            if (mPort.IsOpen)
        //                mPort.Close();
        //            return false;

        //        }
        //    }

        //    public override void Close()
        //    {
        //        for (int i = 1; i <= 4; i++)
        //        {
        //            this.SetChannel(ActiveChannel) = false;
        //            this.EnableChannel(i) = false;
        //        }
        //        mPort.Close();
        //    }

        //    protected override void StartExposure()
        //    {
        //        this.ShutterOpen = true;
        //    }

        //    protected override bool ExposureRunning
        //    {
        //        get { return this.ShutterOpen; }
        //    }

        //    public override bool ShutterOpen
        //    {
        //        get { return DateTime.Now.Subtract(mStartTime).TotalSeconds < mExpectedTime; }
        //        set
        //        {
        //            byte[] data = null;
        //            if (value)
        //            {
        //                switch (ActiveChannel)
        //                {
        //                    case ChannelEnum.Channel1:
        //                        this.EnableChannel[0] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[0] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel2:
        //                        this.EnableChannel[1] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[1] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel3:
        //                        this.EnableChannel[2] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[2] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel4:
        //                        this.EnableChannel[3] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[3] = true;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                }



        //                //Me.EnableChannel(ActiveChannel) = True
        //                //System.Threading.Thread.Sleep(200)
        //                //Me.SetChannel(ActiveChannel) = True
        //                //System.Threading.Thread.Sleep(200)
        //                //Me.EnableChannel(ChannelEnum.Channel2) = True
        //                //System.Threading.Thread.Sleep(200)
        //                //Me.SetChannel(ChannelEnum.Channel2) = True
        //            }
        //            else
        //            {
        //                switch (ActiveChannel)
        //                {
        //                    case ChannelEnum.Channel1:
        //                        this.EnableChannel[0] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[0] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel2:
        //                        this.EnableChannel[1] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[1] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel3:
        //                        this.EnableChannel[2] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[2] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                    case ChannelEnum.Channel4:
        //                        this.EnableChannel[3] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        this.SetChannel[3] = false;
        //                        System.Threading.Thread.Sleep(200);
        //                        break;
        //                }


        //                //Me.EnableChannel(ActiveChannel) = False
        //                //System.Threading.Thread.Sleep(200)
        //                //Me.SetChannel(ActiveChannel) = False
        //            }
        //        }
        //    }

        //    #region "serial communication"
        //    private bool SendCmdWithChannelInfo(byte cmd, ref byte[] DataIn)
        //    {
        //        byte[] data = new byte[2];
        //        data[0] = cmd;
        //        data[1] = Convert.ToByte(this.ActiveChannel);
        //        return this.SendCmd(data, ref DataIn);
        //    }

        //    private bool SendCmd(byte[] DataOut, ref byte[] DataIn)
        //    {
        //        int i = 0;
        //        int length = 0;
        //        int checksum = 0;
        //        byte[] Cmd = null;

        //        length = DataOut.Length + 4;
        //        Cmd = new byte[length];

        //        //build command
        //        Cmd[0] = 0x3c;
        //        Cmd[1] = Convert.ToByte(DataOut.Length + 1);
        //        //includes the check sum

        //        checksum = 0;
        //        for (i = 0; i <= DataOut.Length - 1; i++)
        //        {
        //            Cmd[i+2] = DataOut[i];
        //            checksum += DataOut[i];
        //        }
        //        Cmd[length - 2] = Convert.ToByte(checksum & 0xff);
        //        Cmd[length - 1] = 0xd;

        //        //send data
        //        mPort.DiscardInBuffer();
        //        mPort.DiscardOutBuffer();

        //        mPort.Write(Cmd, 0, length);

        //        //delay
        //        System.Threading.Thread.Sleep(20);

        //        //read data back
        //        mPort.Read(Cmd, 0, 2);
        //        //cmd[1] contains the length of the returnd data
        //        length = Cmd[1];
        //        mPort.Read(Cmd, 0, length);

        //        //cmd now contains the checksum and ending code, we will ignore them, no checksum check
        //        //but we will check the return command data, it should be want is sent + 1
        //        if (Cmd[0] == DataOut[0] + 1)
        //        {
        //            length = length - 3;
        //            //no cmd, no checksum, no ending type
        //            DataIn = new byte[length];
        //            for (i = 0; i <= length - 1; i++)
        //            {
        //                DataIn[i] = Cmd[i + 1];
        //            }
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    private void SendCmd(int CmdID, byte[] bData)
        //    {
        //        System.Text.ASCIIEncoding e = new System.Text.ASCIIEncoding();
        //        int i = 0;
        //        int ii = 0;
        //        int length = 0;
        //        int CRC = 0;
        //        byte[] Data = null;

        //        //build command, "data" length does not include Pre-Code, lengh, and CRC
        //        if (bData != null)
        //        {
        //            ii = bData.Length;
        //            length = 6 + ii;
        //            Data = new byte[length];
        //        }
        //        else
        //        {
        //            Data = new byte[7];
        //            length = Data.Length;
        //            ii = 1;
        //        }

        //        //pre code - RS232
        //        Data[0] = 0x55;
        //        Data[1] = 0xaa;
        //        //length
        //        Data[2] = Convert.ToByte(4 + ii - 1);
        //        Data[3] = Convert.ToByte(mAddress);
        //        Data[4] = Convert.ToByte(CmdID);
        //        //Data[5] = Convert.ToByte(CmdChannel)

        //        //data
        //        if (bData != null)
        //        {
        //            for (i = 0; i <= ii - 1; i++)
        //            {
        //                Data[5 + i] = bData[i];
        //            }
        //        }

        //        //calculate CRC
        //        CRC = 0;
        //        for (i = 0; i <= length - 2; i++)
        //        {
        //            CRC += Data[i];
        //        }
        //        //CRC
        //        Data[length - 1] = Convert.ToByte(CRC & 0xff);

        //        //send data
        //        mPort.DiscardInBuffer();
        //        mPort.DiscardOutBuffer();
        //        mPort.Write(Data, 0, Data.Length);

        //        //delay
        //        System.Threading.Thread.Sleep(20);
        //    }

        //    private byte[] QueryData(int CmdID, byte[] bData)
        //    {
        //        byte[] data = new byte[61];
        //        byte[] NewData = null;
        //        int len = 0;

        //        this.SendCmd(Convert.ToByte(CmdID), bData);
        //        System.Threading.Thread.Sleep(1000);
        //        mPort.Read(data, 0, mPort.BytesToRead);

        //        len = data[2];
        //        len += 3;

        //        NewData = new byte[len];

        //        Array.Copy(data, NewData, NewData.Length);

        //        return NewData;
        //    }

        //    private byte[] QueryData(int CmdID, byte[] bData, int BytesToRead)
        //    {
        //        byte[] data = new byte[61];
        //        //Dim NewData() As Byte
        //        int n = 0;

        //        this.SendCmd(Convert.ToByte(CmdID), bData);
        //        while (mPort.BytesToRead < BytesToRead & n < 15)
        //        {
        //            System.Threading.Thread.Sleep(200);
        //            n += 1;
        //        }

        //        if (mPort.BytesToRead < 20 | mPort.BytesToRead > 60)
        //        {
        //            System.Threading.Thread.Sleep(200);
        //            n = 0;
        //            this.SendCmd(Convert.ToByte(CmdID), bData);
        //            while (mPort.BytesToRead < BytesToRead & n < 15)
        //            {
        //                System.Threading.Thread.Sleep(200);
        //                n += 1;
        //            }
        //        }

        //        mPort.Read(data, 0, mPort.BytesToRead);

        //        //len = data[2]
        //        //len += 3

        //        //ReDim NewData(len - 1)

        //        //Array.Copy(data, NewData, NewData.Length)

        //        return data;
        //    }

        //    #endregion

        //    public override bool AlarmOn
        //    {
        //        get { return false; }
        //    }

        //    protected override bool CheckAlarm(bool CheckCalibration)
        //    {
        //        return true;
        //    }

        //    public override bool ExposureFailed
        //    {
        //        get { return false; }
        //    }

        //    public override double ExposureTime
        //    {
        //        get
        //        {
        //            UvParameterStructure para = default(UvParameterStructure);
        //            switch (ActiveChannel)
        //            {
        //                case ChannelEnum.Channel1:
        //                    para = this.UVParameter[0];
        //                    break;
        //                case ChannelEnum.Channel2:
        //                    para = this.UVParameter[1];
        //                    break;
        //                case ChannelEnum.Channel3:
        //                    para = this.UVParameter[2];
        //                    break;
        //                case ChannelEnum.Channel4:
        //                    para = this.UVParameter[3];
        //                    break;
        //            }


        //            //para = Me.UVParameter(ActiveChannel)
        //            return para.CureTime[0];
        //        }
        //        set
        //        {
        //            UvParameterStructure para = default(UvParameterStructure);
        //            System.Threading.Thread.Sleep(200);
        //            switch (ActiveChannel)
        //            {
        //                case ChannelEnum.Channel1:
        //                    para = this.UVParameter[0];
        //                    para.CureTime[0] = value;
        //                    System.Threading.Thread.Sleep(200);
        //                    this.UVParameter[0] = para;
        //                    break;
        //                case ChannelEnum.Channel2:
        //                    para = this.UVParameter[1];
        //                    para.CureTime[0] = value;
        //                    System.Threading.Thread.Sleep(200);
        //                    this.UVParameter[1] = para;
        //                    break;
        //                case ChannelEnum.Channel3:
        //                    para = this.UVParameter[2];
        //                    para.CureTime[0] = value;
        //                    System.Threading.Thread.Sleep(200);
        //                    this.UVParameter[2] = para;
        //                    break;
        //                case ChannelEnum.Channel4:
        //                    para = this.UVParameter[3];
        //                    para.CureTime[0] = value;
        //                    System.Threading.Thread.Sleep(200);
        //                    this.UVParameter[3] = para;
        //                    break;
        //            }



        //            //para = Me.UVParameter[0]
        //            //para.CureTime[0] = value
        //            //System.Threading.Thread.Sleep(200)
        //            //Me.UVParameter[0] = para
        //            //For i = 0 To 1
        //            //    System.Threading.Thread.Sleep(200)
        //            //    para = Me.UVParameter[i]
        //            //    para.CureTime[0] = value
        //            //    System.Threading.Thread.Sleep(200)
        //            //    Me.UVParameter[i] = para
        //            //Next
        //        }
        //    }

        //    public override int ActiveSingleChannel
        //    {
        //        get { return ActiveChannel; }
        //        set { ActiveChannel = (ChannelEnum)value; }
        //    }

        //    public override double PowerLevel
        //    {
        //        get
        //        {
        //            UvParameterStructure para = default(UvParameterStructure);
        //            switch (ActiveChannel)
        //            {
        //                case ChannelEnum.Channel1:
        //                    para = this.UVParameter[0];
        //                    break;
        //                case ChannelEnum.Channel2:
        //                    para = this.UVParameter[1];
        //                    break;
        //                case ChannelEnum.Channel3:
        //                    para = this.UVParameter[2];
        //                    break;
        //                case ChannelEnum.Channel4:
        //                    para = this.UVParameter[3];
        //                    break;
        //            }
        //            //para = Me.UVParameter(ActiveChannel)
        //            return para.CurePower[0];
        //        }
        //        set
        //        {
        //            if (value < 0 | value > 100)
        //            {
        //                MessageBox.Show("Make Sure Power is from 0 to 100%!", "Lamplic UV Lamp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //            else
        //            {
        //                UvParameterStructure para = default(UvParameterStructure);
        //                System.Threading.Thread.Sleep(100);

        //                switch (ActiveChannel)
        //                {
        //                    case ChannelEnum.Channel1:
        //                        para = this.UVParameter[0];
        //                        para.CurePower[0] = Convert.ToInt32(value);
        //                        System.Threading.Thread.Sleep(100);
        //                        this.UVParameter[0] = para;
        //                        break;
        //                    case ChannelEnum.Channel2:
        //                        para = this.UVParameter[1];
        //                        para.CurePower[0] = Convert.ToInt32(value);
        //                        System.Threading.Thread.Sleep(100);
        //                        this.UVParameter[1] = para;
        //                        break;
        //                    case ChannelEnum.Channel3:
        //                        para = this.UVParameter[2];
        //                        para.CurePower[0] = Convert.ToInt32(value);
        //                        System.Threading.Thread.Sleep(100);
        //                        this.UVParameter[2] = para;
        //                        break;
        //                    case ChannelEnum.Channel4:
        //                        para = this.UVParameter[3];
        //                        para.CurePower[0] = Convert.ToInt32(value);
        //                        System.Threading.Thread.Sleep(100);
        //                        this.UVParameter[3] = para;
        //                        break;
        //                }



        //                //para = Me.UVParameter[0]
        //                //para.CurePower[0] = CInt(value)
        //                //System.Threading.Thread.Sleep(100)
        //                //Me.UVParameter[0] = para
        //                //For i = 0 To 1
        //                //    System.Threading.Thread.Sleep(100)
        //                //    para = Me.UVParameter[i]
        //                //    para.CurePower[0] = CInt(value)
        //                //    System.Threading.Thread.Sleep(100)
        //                //    Me.UVParameter[i] = para
        //                //Next
        //            }
        //        }
        //    }

        //    public override bool LampOn
        //    {
        //        get { return true; }
        //        //do nothing
        //        set { }
        //    }

        //    public override bool LampReady
        //    {
        //        //FUWO does not have RS232 command for this, it has a digit output
        //        get { return true; }
        //    }

        //    public override bool NeedCalibration
        //    {
        //        get { return false; }
        //    }

        //    public int Address
        //    {
        //        get { return mAddress; }
        //        set
        //        {
        //            byte[] data = new byte[1];
        //            data[0] = Convert.ToByte(value);
        //            SendCmd(ECmd.SetAddress, ref data);
        //            mAddress = value;
        //        }
        //    }

        //    public UvParameterStructure UVParameter
        //    {
        //        get
        //        {
        //            byte[] data = new byte[1];
        //            data[0] = Convert.ToByte(channel);
        //            data = QueryData(ECmd.GetParameters, data, 54);
        //            UvParameterStructure para = new UvParameterStructure();
        //            para.CureType = (ECureType)Enum.Parse(typeof(ECureType), data[6].ToString());
        //            para.StepCount = data(7);
        //            para.CureTime = new double[para.StepCount + 1];
        //            para.CurePower = new int[para.StepCount + 1];
        //            for (int i = 0; i <= para.StepCount; i++)
        //            {
        //                para.CureTime[i] = (255 * data(9 + 2 * i) + data(8 + 2 * i)) / 10;
        //                para.CurePower[i] = data(40 + i);
        //            }

        //            return para;
        //        }
        //        set
        //        {
        //            byte[] data = new byte[51];
        //            data[0] = Convert.ToByte(channel);
        //            data[1] = Convert.ToByte(value.CureType);
        //            data[2] = Convert.ToByte[1];
        //            for (int i = 0; i <= value.StepCount - 1; i++)
        //            {
        //                data(3 + 2 * i) = Convert.ToByte(10 * value.CureTime[i] % 255);
        //                data(4 + 2 * i) = Convert.ToByte(Convert.ToInt32(10 * value.CureTime[i]) / 255);
        //                data(35 + i) = Convert.ToByte(value.CurePower[i]);
        //            }
        //            SendCmd(ECmd.SetParameters, ref data);
        //        }
        //    }

        //    public bool EnableChannel
        //    {

        //        get { }
        //        set
        //        {
        //            byte[] data = new byte[4];
        //            for (int i = 0; i <= 3; i++)
        //            {
        //                data[i] = 1;
        //            }
        //            if (value)
        //            {
        //                data(channel) = 1;
        //            }
        //            else
        //            {
        //                data(channel) = 0;
        //            }
        //            SendCmd(ECmd.EnableChannel, ref data);
        //        }
        //    }

        //    public bool SetChannel
        //    {

        //        get { }
        //        set
        //        {
        //            byte[] data = new byte[1];
        //            data[0] = Convert.ToByte(49 + channel);
        //            SendCmd(ECmd.OnOffSingleChannelCure, ref data);
        //        }
        //    }

        //}

        public class iOmniCure : iUvCure
        {
            private  SerialPort mPort;

            private int ActiveChannel;
            public override bool Initialize(string sPort, bool RaiseError)
            {

                //set serial port
                mPort = new  SerialPort(sPort, 19200,  Parity.None, 8,  StopBits.One);

                //open port
                try
                {
                    mPort.Open();
                    mPort.NewLine = Convert.ToString(ControlChars.Cr);
                    mPort.Handshake =  Handshake.None;
                    mPort.WriteTimeout = 300;
                    mPort.ReadTimeout = 1000;

                    //establish communication
                    if ("READY" != this.QueryString("CONN"))
                    {
                        if (RaiseError)
                            MessageBox.Show("Cannot estabilish communication with OmniCur.", "OmniCure UV Curer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (mPort.IsOpen)
                            mPort.Close();
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                catch (Exception e)
                {
                    if (RaiseError)
                        MessageBox.Show(e.Message, "OmniCure UV Curer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mPort.IsOpen)
                        mPort.Close();
                    return false;

                }

            }

            public override void Close()
            {
                this.SendCmd("DCON");
                mPort.Close();
            }

            protected override void StartExposure()
            {
                this.SendCmd("RUN");
            }

            protected override bool ExposureRunning
            {
                get
                {
                    double tPassed = 0;
                    tPassed = System.DateTime.Now.Subtract(mStartTime).TotalSeconds;
                    return (tPassed < mExpectedTime);
                }
            }

            public override bool ShutterOpen
            {
                get { return (this.GetStatus() & (int)StatusMask.ShutterOpen) == (int)StatusMask.ShutterOpen; }
                set
                {
                    string s = null;
                    s = (value ? "OPN" : "CLS").ToString();
                    this.SendCmd(s);
                }
            }

            public override bool LampOn
            {
                get
                {
                    int status = this.GetStatus();
                    return (status & (int)StatusMask.LampOn) == (int)StatusMask.LampOn;
                }
                set
                {
                    string s = null;
                    s = (value ? "TON" : "TOF").ToString();
                    this.SendCmd(s);
                }
            }

            public override int ActiveSingleChannel
            {
                get { return ActiveChannel; }
                set { ActiveChannel = value; }
            }

            #region "lamp proeprty"
            public string SerialNumber
            {
                get { return this.QueryString("GSN"); }
            }

            public struct LampConfiguration
            {
                public enum LampType
                {
                    SurfaceCuring = 0,
                    Standard = 1
                }
                public bool Abused;
                public LampType Type;
                public int Hours;
            }

            public LampConfiguration Lamp
            {
                get
                {
                    int k = 0;
                    LampConfiguration x = default(LampConfiguration);

                    k = this.QueryInteger("GLH");

                    x.Abused = (k & 0x8000) == 0x8000;

                    if ((k & 0x4000) == 0x4000)
                    {
                        x.Type = LampConfiguration.LampType.Standard;
                    }
                    else
                    {
                        x.Type = LampConfiguration.LampType.SurfaceCuring;
                    }

                    x.Hours = (k & 0x3fff);

                    return x;
                }
            }

            public int CalibratedLampHours
            {
                get { return this.QueryInteger("CLH"); }
            }
            #endregion

            #region "alarm, status"
            private enum StatusMask
            {
                AlarmOn = 0x1,
                LampOn = 0x2,
                ShutterOpen = 0x4,
                HomeFaulty = 0x8,
                LampReady = 0x10,
                FrontLockout = 0x20,
                InCalibratio = 0x40,
                ExposureFault = 0x80
            }

            private int GetStatus()
            {
                int status = this.QueryInteger("GUS");
                return status;
            }

            protected override bool CheckAlarm(bool CheckCalibration)
            {
                int k = 0;
                string s = null;

                //check if we can run
                k = this.GetStatus();
                s = "";
                if ((k & (int)StatusMask.AlarmOn) == (int)StatusMask.AlarmOn)
                    s += "Alarm On" + ControlChars.CrLf;
                if ((k & (int)StatusMask.LampOn) != (int)StatusMask.LampOn)
                    s += "Lamp is not on" + ControlChars.CrLf;
                if ((k & (int)StatusMask.LampReady) != (int)StatusMask.LampReady)
                    s += "Lamp is not ready" + ControlChars.CrLf;
                if ((k & (int)StatusMask.ShutterOpen) == (int)StatusMask.ShutterOpen)
                    s += "Shutter open, exposure running" + ControlChars.CrLf;

                if (!string.IsNullOrEmpty(s))
                {
                    MessageBox.Show(s, "UV Cure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //warning
                //If CheckCalibration And (k And StatusMask.InCalibratio) <> StatusMask.InCalibratio Then
                //    s = "Lamp is not calibrated. Do you want to continue?"
                //    If Windows.Forms.DialogResult.Yes <> MessageBox.Show(s, "UV Cure", MessageBoxButtons.YesNo, MessageBoxIcon.Question) Then
                //        Return False
                //    End If
                //End If

                //done
                return true;
            }

            public bool FrontPanelLockout
            {
                get { return (this.GetStatus() & (int)StatusMask.FrontLockout) == (int)StatusMask.FrontLockout; }
                set
                {
                    string s = null;
                    s = (value ? "LOC" : "ULOC").ToString();
                    this.SendCmd(s);
                }
            }

            public override bool AlarmOn
            {
                get { return (this.GetStatus() & (int)StatusMask.AlarmOn) == (int)StatusMask.AlarmOn; }
            }

            public void ClearAlarm()
            {
                this.SendCmd("CLR");
            }

            public override bool NeedCalibration
            {
                get { return (this.GetStatus() & (int)StatusMask.InCalibratio) != (int)StatusMask.InCalibratio; }
            }

            public void ClearUnitCalibration()
            {
                this.SendCmd("CLC");
            }

            public override bool LampReady
            {
                get { return (this.GetStatus() & (int)StatusMask.LampReady) == (int)StatusMask.LampReady; }
            }

            public override bool ExposureFailed
            {
                get { return (this.GetStatus() & (int)StatusMask.HomeFaulty) == (int)StatusMask.HomeFaulty; }
            }
            #endregion

            #region "setting"
            public override double ExposureTime
            {
                get
                {
                    double time = this.QueryInteger("GTM");
                    return 0.1 * time;
                }
                set
                {
                    value *= 10;
                    if (value < 2)
                    {
                        value = 2;
                    }
                    else if (value > 9999)
                    {
                        value = 9999;
                    }
                    this.SendCmd("STM" + value.ToString("0"));
                }
            }

            public override double PowerLevel
            {
                get { return this.QueryDouble("GPW"); }
                set { this.SendCmd("SPW" + value.ToString("0.00")); }
            }

            public double Irradiance
            {
                get { return this.QueryDouble("GIR"); }
                set
                {
                    if (value < 0)
                        value = 0.1;
                    this.SendCmd("SIR" + value.ToString("0.00"));
                }
            }

            public double IrradianceActual
            {
                get { return this.QueryDouble("GIM"); }
            }

            public double IrradianceMaximum
            {
                get { return this.QueryDouble("GMP"); }
            }

            public double IrisLevel
            {
                get { return 0.01 * this.QueryInteger("GIL"); }
                set
                {
                    value *= 100.0;
                    if (value < 1)
                    {
                        value = 1;
                    }
                    else if (value > 100)
                    {
                        value = 100;
                    }
                    this.SendCmd("SIL" + value.ToString("0"));
                }
            }

            #endregion

            #region "serial communication"
            private string QueryString(string sCmd)
            {
                string s = null;
                this.SendCmd(sCmd);
                s = mPort.ReadLine();
                //remove the CRC from the end
                s = s.Substring(0, s.Length - 2);
                return s;
            }

            private int QueryInteger(string sCmd)
            {
                string s = null;
                int k = 0;
                s = this.QueryString(sCmd);
                if (int.TryParse(s, out k))
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }

            private double QueryDouble(string sCmd)
            {
                string s = null;
                double v = 0;
                s = this.QueryString(sCmd);
                if (double.TryParse(s, out v))
                {
                    return v;
                }
                else
                {
                    return double.NaN;
                }
            }

            private void SendCmd(string sCmd)
            {
                System.Text.ASCIIEncoding e = new System.Text.ASCIIEncoding();
                byte CRC = 0;
                byte[] Data = null;

                //build command
                Data = e.GetBytes(sCmd);

                CRC = Delta.CheckSum.CRC8(Data);

                sCmd += Convert.ToString(CRC, 16).ToUpper();
                sCmd += ControlChars.Cr;

                //send data
                mPort.DiscardInBuffer();
                mPort.DiscardOutBuffer();
                mPort.Write(sCmd);

                //delay
                System.Threading.Thread.Sleep(20);
            }


            #endregion

        }

        public abstract class iUvCure
    {

        private System.ComponentModel.BackgroundWorker mWorker;
        public iUvCure()
        {
            mWorker = new System.ComponentModel.BackgroundWorker();
            mWorker.WorkerSupportsCancellation = true;
            mWorker.DoWork += mWorker_DoWork;
            mWorker.RunWorkerCompleted += mWorker_RunWorkerCompleted;
        }

        public abstract bool Initialize(string sPort, bool RaiseError);
        public abstract void Close();

        public abstract bool ShutterOpen { get; set; }
        public abstract bool LampOn { get; set; }

        public abstract double ExposureTime { get; set; }
        public abstract double PowerLevel { get; set; }

        public abstract bool AlarmOn { get; }
        public abstract bool LampReady { get; }
        public abstract bool NeedCalibration { get; }
        public abstract bool ExposureFailed { get; }

        public abstract int ActiveSingleChannel { get; set; }


        //following are used for async run
        protected abstract bool CheckAlarm(bool CheckCalibration);
        protected abstract bool ExposureRunning { get; }
        protected abstract void StartExposure();

        #region "asycn run"
        protected System.DateTime mStartTime;

        protected double mExpectedTime;
        public bool ExposureInProgress
        {
            get { return mWorker.IsBusy; }
        }

        public double ExposureTimePassed
        {
            get { return System.DateTime.Now.Subtract(mStartTime).TotalSeconds; }
        }

        public double ExposureTimeExpected
        {
            get { return mExpectedTime; }
        }

        public void StopUvExposure()
        {
            this.ShutterOpen = false;
            mWorker.CancelAsync();
        }

        public bool RunExposure(bool CheckCalibration)
        {
            //check alarm
            //If Not Me.CheckAlarm(CheckCalibration) Then Return False
            System.Threading.Thread.Sleep(200);
            mExpectedTime = this.ExposureTime;

            //do actual run
            this.StartExposure();
            DateTime
            mStartTime = DateTime.Now;
            //start wait  
            mWorker.RunWorkerAsync();

            //return success
            return true;
        }

        protected void mWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            mStartTime = System.DateTime.Now;
            while (this.ExposureRunning)
            {
                System.Threading.Thread.Sleep(100);
                if (mWorker.CancellationPending)
                    break; // TODO: might not be correct. Was : Exit While
            }
        }

        protected void mWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //do nothing
        }
        #endregion

    }
    }

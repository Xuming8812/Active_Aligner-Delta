using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Net;
using System.IO.Ports;
using Delta;

namespace Instrument
{
    public class iMitsubishiRobot_6_Axis : iRobotController
    {
        Socket cliSocket;
        private double mTimeOut;
        private bool mComplete;
        private string mAxisString;

        public enum AxisEnum
        {
            All = -1,
            X,
            Y,
            Z,
            Rx,
            Ry,
            Rz
        }

        #region "Initialize and close"
        public override bool Initialize(string sConnection, bool RaiseError)
        {
            string reply = null;
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(sConnection), 10008);
            cliSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                cliSocket.Connect(remoteEP);
                SendCmd("OPEN=");
                System.Threading.Thread.Sleep(500);
                reply = ReadString();
                reply = QueryString("CNTLON");
            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, "Mistsubishi robot", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            mComplete = true;
            return true;
        }
        public override void Close()
        {
            SendCmd("CLOSE");
            cliSocket.Close();
        }
        #endregion

        #region "system error and info"
        public void EmergencyReset()
        {
            this.SendCmd("ERRLOGCLR");
        }

        public override int Axis
        {
            get { return mAxis; }
            set
            {
                if (value < 1)
                    return;
                if (value > mAxisCount)
                    return;
                mAxis = value;
                switch (mAxis)
                {
                    case 0:
                        mAxisString = "X";
                        break;
                    case 1:
                        mAxisString = "Y";
                        break;
                    case 2:
                        mAxisString = "Z";
                        break;
                    case 3:
                        mAxisString = "";
                        break;
                    case 4:
                        mAxisString = "";
                        break;
                    case 5:
                        mAxisString = "Rz";
                        break;
                }
            }
        }

        public override string AxisName
        {
            get { return mAxisString; }
        }

        #endregion

        #region "Status"

        public override double CurrentPosition
        {
            get
            {
                string s = null;
                string[] data = null;
                s = this.QueryString("PPOSF");
                s = this.QueryString("PPOSF");
                data = s.Split(Convert.ToChar(";"));
                return Convert.ToDouble(data[mAxis * 2 + 1]);
            }

        }

        public override iRobot.Position6D Current6DPosition
        {
            get
            {
                iRobot.Position6D p = default(iRobot.Position6D);
                string s = null;
                string[] data = null;

                s = this.QueryString("PPOSF");
                s = this.QueryString("PPOSF");
                data = s.Split(Convert.ToChar(";"));
                p.X = Convert.ToDouble(data[1]);
                p.Y = Convert.ToDouble(data[3]);
                p.Z = Convert.ToDouble(data[5]);
                p.Rx = Convert.ToDouble(data[7]);
                p.Ry = Convert.ToDouble(data[9]);
                p.Rz = Convert.ToDouble(data[11]);
                return p;
            }
        }

        public override ServoStatusMethodEnum ServoStatus
        {
            get { return ServoStatusMethodEnum.ServoOn; }
            set
            {
                if (value == ServoStatusMethodEnum.ServoOn)
                {
                    this.SendCmd("SRVON");
                }
                else
                {
                    this.SendCmd("SRVOFF");
                }
            }
        }

        public override double Velocity
        {
            get
            {
                return 0.0;
            }
            set
            {
                this.SendCmd("OVRD=" + value.ToString("0.00"));
            }

        }

        public override double Acceleration
        {
            get
            {
                return 0.0;
            }
            set
            {

            }
        }

        public override bool DriveEnabled
        { 
            set
            { }
        }
        #endregion

        #region "Motion"
        public override bool HaltMotion()
        {
            return this.SendCmd("STOP");
        }

        public override bool InitializeMotion()
        {
            throw new NotImplementedException();
        }

        public override bool KillMotion()
        {
            return this.SendCmd("STOP");
        }

        public override bool StageMoving
        {
            get
            {
                return false;
            }
        }

        protected override bool StartHome()
        {
            SendCmd("MOVSP");
            return true;
        }

        protected override bool StartMove(MoveToTargetMethodEnum Method, double Target)
        {
            iRobot.Position6D Target6DPosition = new iRobot.Position6D (0,0,0,0,0,0);

            switch (mAxis)
            {
                case 1:
                    Target6DPosition.X += Target;
                    break;
                case 2:
                    Target6DPosition.Y += Target;
                    break;
                case 3:
                    Target6DPosition.Z += Target;
                    break;
                case 4:
                    Target6DPosition.X += Target;
                    break;
                case 5:
                    Target6DPosition.Y += Target;
                    break;
                case 6:
                    Target6DPosition.Z += Target;
                    break;
            }

            return this.StartMove(Method, Target6DPosition);
        }

        protected override bool StartMove(MoveToTargetMethodEnum Method, iRobot.Position6D Target)
        {
            iRobot.Position6D Current6DPosition = default(iRobot.Position6D);

            Current6DPosition = this.Current6DPosition;

            if (Method == MoveToTargetMethodEnum.Relative)
            {
                Target += Current6DPosition;
            }

            string s = null;
            string fmt = null;
            fmt = "{0,9}{1,7},{2,7},{3,7},{4,7},{5,7},{6,7}";
            s = string.Format(fmt, "EXECMOV (", Target.X.ToString("0.00"), Target.Y.ToString("0.00"), Target.Z.ToString("0.00"), 
                Target.Rx.ToString("0.00"), Target.Ry.ToString("0.00"), Target.Rz.ToString("0.00")) + ")(7,0)";
            s = QueryString(s);
            if (s.Contains("oK"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region "socket communication"

        public bool SendCmd(string sCmd)
        {
            string sRobot_ID = "1;1;";
            byte[] msg = Encoding.UTF8.GetBytes(sRobot_ID + sCmd);
            byte[] bytes = new byte[1025];
            string data = string.Empty;

            cliSocket.Send(msg);
            //Dim s As String
            //s = ReadString()

            //Return s.Contains("oK")
            //mComplete = True
            return true;
        }

        public string QueryString(string sCmd)
        {
            string sRobot_ID = "1;1;";
            byte[] msg = Encoding.UTF8.GetBytes(sRobot_ID + sCmd);
            cliSocket.Send(msg);

            string s = null;
            s = ReadString();

            return s;
        }

        public string ReadString()
        {
            dynamic timeOut = 200000;
            System.DateTime Tstart = DateTime.Now;
            int bytesRec = 0;
            byte[] bytes = new byte[1025];
            string data = string.Empty;

            while (bytesRec <= 0 & DateTime.Now.Subtract(Tstart).TotalMilliseconds < timeOut)
            {
                bytesRec = cliSocket.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }

            return data;
        }
        #endregion

    }

    public class iYamahaRobot_4_Axis : iRobotController
    {

        private SerialPort mPort;
        private double mTimeOut;
        private bool mComplete;
        private string mAxisString;


        public enum AxisEnum
        {         
            X,
            Y,
            Z,
            Rx,
            Ry,
            Rz
        }

        public enum EMode
        {
            AUTO,
            PROGRAM,
            MANUAL,
            SYSTEM,
            NONE
        }

        public enum EArmType
        {
            Left,
            Right
        }

        #region "Initialize and close"
        public override bool Initialize(string sConnection, bool RaiseError)
        {
            mPort = new SerialPort(sConnection, 9600, Parity.Odd, 8, StopBits.One);

            //open port
            try
            {
                mPort.Open();
                mPort.NewLine = ControlChars.CrLf;
                mPort.Handshake = Handshake.RequestToSendXOnXOff;
                mPort.WriteTimeout = 300;
                mPort.ReadTimeout = 500;

                //establish communication
            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, "Yamaha Robot Controller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;
            }

            mComplete = true;

            return true;
        }

        public override void Close()
        {
            this.SendCmd("DCON");
            mPort.Close();
        }
        #endregion

        #region "system error and info"
        public string ControllerVersion
        {
            get { return this.QueryString("@ ?VER"); }
        }

        public string LastError
        {
            get
            {
                int p = 0;
                string s = null;
                s = this.QueryString("@ ?MSG");
                return s;
            }
        }

        public void EmergencyReset()
        {
            this.SendCmd("@ EMGRST");
        }

        public double TimeOut
        {
            get { return mTimeOut; }
            set
            {
                mTimeOut = value;
                for (int i = 1; i <= Enum.GetValues(typeof(AxisEnum)).Length; i++)
                {
                    this.SendCmd("@ TRQTIME(" + i.ToString() + ")=" + value.ToString());
                }
            }
        }

        public override int Axis
        {
            get { return mAxis; }
            set
            {
                if (value < 1)
                    return;
                if (value > mAxisCount)
                    return;
                mAxis = value;
                switch (mAxis)
                {
                    case 0:
                        mAxisString = "X";
                        break;
                    case 1:
                        mAxisString = "Y";
                        break;
                    case 2:
                        mAxisString = "Z";
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        mAxisString = "Rz";
                        break;
                }
            }
        }

        public override string AxisName
        {
            get { return mAxisString; }
        }
        #endregion



        #region "Status"
        public EMode Mode
        {
            get
            {
                string s = null;
                s = this.QueryString("@ ?MOD");
                switch (s)
                {
                    case "AUTO":
                        return EMode.AUTO;
                    case "PROGRAM":
                        return EMode.PROGRAM;
                    case "MANUAL":
                        return EMode.MANUAL;
                    case "SYSTEM":
                        return EMode.SYSTEM;
                }
                return EMode.NONE;
            }
            set
            {
                switch (value)
                {
                    case EMode.AUTO:
                        this.SendCmd("@ AUTO");
                        break;
                    case EMode.PROGRAM:
                        this.SendCmd("@ PROGRAM");
                        break;
                    case EMode.SYSTEM:
                        this.SendCmd("@ SYSTEM");
                        break;
                    case EMode.MANUAL:
                        this.SendCmd("@ MANUAL");
                        break;
                }
            }
        }

        public override ServoStatusMethodEnum ServoStatus
        {
            get
            {
                string s = null;
                s = this.QueryString("@ ?SERVO");
                if (s.Contains("ON"))
                {
                    return ServoStatusMethodEnum.ServoOn;
                }
                else
                {
                    return ServoStatusMethodEnum.ServoOff;
                }
            }
            set
            {
                if (value == ServoStatusMethodEnum.ServoOn)
                {
                    this.SendCmd("@ SERVO ON");
                }
                else
                {
                    this.SendCmd("@ SERVO OFF");
                }
            }
        }

        public void SetServoMode(AxisEnum axis, ServoStatusMethodEnum status)
        {
            switch (status)
            {
                case ServoStatusMethodEnum.ServoOn:
                    this.SendCmd("@ SERVO ON (" + Convert.ToInt16(axis).ToString() + ")");
                    break;
                case ServoStatusMethodEnum.ServoOff:
                    this.SendCmd("@ SERVO OFF (" + Convert.ToInt16(axis).ToString() + ")");
                    break;
            }
        }

        public EArmType ArmType
        {
            get
            {
                string s = null;
                s = this.QueryString("@ ?ARM");
                if (s.Contains("RIGHT"))
                {
                    return EArmType.Right;
                }
                else
                {
                    return EArmType.Left;
                }
            }
            set
            {
                if (value == EArmType.Left)
                {
                    this.SendCmd("@ LEFTY");
                }
                else
                {
                    this.SendCmd("@ RIGHTY");
                }
            }
        }

        public override double CurrentPosition
        {
            get
            {
                iRobot.Position6D p = default(iRobot.Position6D);
                p = this.Current6DPosition;
                switch (mAxis)
                {
                    case 0:
                        return p.X;
                    case 1:
                        return p.Y;
                    case 2:
                        return p.Z;
                    case 5:
                        return p.Rz;
                    default:
                        return double.NaN;

                }

            }
        }

        public override iRobot.Position6D Current6DPosition
        {
            get
            {
                iRobot.Position6D p = default(iRobot.Position6D);
                string s = null;
                string[] data = new string[6];
                s = this.QueryString("@ ?WHRXY");
                for (int i = 1; i <= 3; i++)
                {
                    if (!s.Contains("POS"))
                    {
                        s = this.QueryString("@ ?WHRXY");
                    }
                }
                for (int i = 0; i <= 5; i++)
                {
                    data[i] = s.Substring(5 + i * 8, 8);
                }

                p.X = Convert.ToDouble(data[0]);
                p.Y = Convert.ToDouble(data[1]);
                p.Z = Convert.ToDouble(data[2]);
                p.Rx = 0;
                p.Ry = 0;
                p.Rz = Convert.ToDouble(data[3]);
                return p;
            }
        }

        #endregion

        #region "Config"
        public override double Acceleration
        {
            get
            {
                string s = null;
                s = this.QueryString("@ ?ACCEL");
                return Convert.ToDouble(s);
            }
            set {
                this.SendCmd("@ ACCEL " + value.ToString());
            }
        }

        public override double Velocity
        {
            get
            {
                string s = null;
                string[] data = null;
                s = this.QueryString("@ ?SPEED");
                data = s.Split(',');
                return Convert.ToDouble(data[1]);
            }
            set { this.SendCmd("@ MSPEED " + value.ToString()); }
        }

        public override bool DriveEnabled
        {
            set
            { }
        }



        #endregion

        #region "motion"
        protected override bool StartHome()
        {
            return this.SendCmdWithoutWait("@ ORGRTN");
        }

        protected override bool StartMove(MoveToTargetMethodEnum Method, double Target)
        {
            throw new NotImplementedException();
        }

        protected override bool StartMove(MoveToTargetMethodEnum Method, iRobot.Position6D Target)
        {
            string s = null;
            string fmt = null;
            fmt = "{0,9}, {1,7} {2,7} {3,7} {4,7} {5,7} {6,7}";
            s = string.Format(fmt, "@ MOVE P ", Target.X.ToString("0.00"), Target.Y.ToString("0.00"), Target.Z.ToString("0.00"), Target.Rz.ToString("0.00"), Target.Rx.ToString("0.00"), Target.Ry.ToString("0.00"));
            return this.SendCmdWithoutWait(s);        
        }

        public override bool HaltMotion()
        {
            byte[] command = new byte[2];
            command[0] = 3;
            mPort.Write(command, 0, 1);
            return true;
        }

        public override bool KillMotion()
        {
            return this.HaltMotion();
        }

        public override bool InitializeMotion()
        {
            return this.SendCmd("@ INIT HND");
        }

        public override bool StageMoving
        {
            get
            {
                if (!mComplete)
                {
                    System.Threading.Thread.Sleep(100);
                    mComplete = mPort.BytesToRead > 1;
                }

                return mComplete;
            }

        }
        #endregion

        #region "serial communication"
        private string QueryString(string sCmd)
        {
            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();
            //sCmd += vbCrLf
            mPort.WriteLine(sCmd);

            //delay
            while (mPort.BytesToRead < 2)
            {
                System.Threading.Thread.Sleep(100);
            }

            string s = null;
            s = mPort.ReadLine();
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

        private bool SendCmdWithoutWait(string sCmd)
        {

            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();
            //sCmd += vbCrLf
            mPort.WriteLine(sCmd);
            mComplete = false;
            return true;
        }

        private bool SendCmd(string sCmd)
        {

            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();
            //sCmd += vbCrLf
            mPort.WriteLine(sCmd);

            //delay
            while (mPort.BytesToRead < 2)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }

            string s = null;
            s = mPort.ReadLine();
            mComplete = true;
            return s.Contains("OK");
            
        }


        #endregion
    }
}

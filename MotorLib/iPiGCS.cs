using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.IO.Ports;
namespace Instrument
{
    public class iPiGCS : iMotionController
    {
        #region "additional DLL"
        [DllImport("PI_GCS2_DLL.dlll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PI_qSPI(int iID, string Axis,ref double[] Values);
        [DllImport("PI_GCS2_DLL.dlll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PI_SPI(int iID, string Axis, ref double[] Values);
        #endregion

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

        private int mID;
        private int mError;

        private string mAxisString;

        public iPiGCS(int AxisCount) : base(AxisCount)
        {
        }

        public override bool Initialize(string sConnection, bool RaiseError)
        {
            string s = null;
            int iPort = 0;

            if (sConnection.StartsWith("COM"))
            {
                s = sConnection.Substring(3);
                iPort = Convert.ToInt32(s);
                mID = GCS2.ConnectRS232(iPort, 115200);
            }
            else if (sConnection.Contains("."))
            {
                mID = GCS2.ConnectTCPIP(sConnection, 50000);
            }
            else
            {
                MessageBox.Show("Unrecognized connection string: " + sConnection, "PI CGS");
                return false;
            }
            if ((mID == -1) & RaiseError)
            {
                MessageBox.Show("Error connecting to PI controller at " + sConnection, "PI CGS");
                return false;
            }
            else
            {
                //we are fine
            }

            //following were used for PIVOT debug, 
            //Me.StartHome()
            //Dim v1, v2, v3 As Double
            //'Me.SetPivot(0, 0, 0)
            //Me.ReadPivot(v1, v2, v3)

            double r0 = 0;
            double s0 = 0;
            double t0 = 0;
            this.ReadPivot(ref r0, ref s0, ref t0);
            this.SetPivot(1.8, 1.5, 1.7);

            return (GCS2.IsConnected(mID) == 1);
        }

        public override void Close()
        {
            GCS2.CloseConnection(mID);
        }


        #region "Pivot Point"
        public bool ReadPivot(ref double R, ref double S, ref double T)
        {
            System.Text.StringBuilder reply = new System.Text.StringBuilder();
            int i = 0;

            mError = GCS2.GcsCommandset(mID, "SPI?");
            System.Threading.Thread.Sleep(100);
            mError = GCS2.GcsGetAnswerSize(mID, ref i);
            for (int k = 0; k <= 2; k++)
            {
                mError = GCS2.GcsGetAnswer(mID, reply, i);
                dynamic s1 = reply.ToString();
                if (s1.Contains("R"))
                {
                    s1 = s1.Replace("R=", "");
                    R = Conversion.Val(s1);
                }

                if (s1.Contains("S"))
                {
                    s1 = s1.Replace("S=", "");
                    S = Conversion.Val(s1);
                }

                if (s1.Contains("T"))
                {
                    s1 = s1.Replace("T=", "");
                    T = Conversion.Val(s1);
                }

            }

            return (mError == 1);
        }

        public bool SetPivot(double R, double S, double T)
        {
            //mError = PI_SPI(mID, "R", New Double() {R})
            //If (mError = 1) Then mError = PI_SPI(mID, "S", New Double() {S})
            //If (mError = 1) Then mError = PI_SPI(mID, "T", New Double() {T})
            //Return (mError = 1)
            string head = null;
            string command = null;
            head = "SPI ";
            command = head + "R " + R;
            mError = GCS2.GcsCommandset(mID, command);

            command = head + "S " + S;
            mError = GCS2.GcsCommandset(mID, command);

            command = head + "T " + T;
            mError = GCS2.GcsCommandset(mID, command);

            return (mError == 1);
        }
        #endregion
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
                        mAxisString = "";
                        // for all connected axis
                        break;
                    case 1:
                        mAxisString = "X";
                        break;
                    case 2:
                        mAxisString = "Y";
                        break;
                    case 3:
                        mAxisString = "Z";
                        break;
                    case 4:
                        mAxisString = "U";
                        break;
                    case 5:
                        mAxisString = "V";
                        break;
                    case 6:
                        mAxisString = "W";
                        break;
                }
            }
        }

        public override string AxisName
        {
            get { return mAxisString; }
        }

        public override double CurrentPosition
        {
            get
            {
                double[] v = new double[1];
                mError = GCS2.qPOS(mID, mAxisString, v);
                if (mError == 1)
                {
                    return v[0];
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        #region "Move, home, halt"

        public override bool StageReady
        {
            get
            {
                int[] data = new int[1];
                mError = GCS2.IsControllerReady(mID, ref data[0]);
                if ((mError == 1) & (data[0] == 1))
                {
                    mError = GCS843.qFRF(mID, mAxisString, data);
                    //Return (mError = 1) And (data[0] = 1)
                    return data[0] == 1;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool StageMoving
        {
            get
            {
                int[] value = new int[1];
                mError = GCS2.IsMoving(mID, mAxisString, value);
                if ((mError == 1) & (value[0] == 1))
                    return true;

                //homing will cause not ready 
                if (!this.StageReady)
                    return true;

                return false;
            }
        }

        public override bool InitializeMotion()
        {
            mError = GCS2.INI(mID, mAxisString);
            return (mError == 1);
        }

        protected override bool StartHome()
        {
            mError = GCS2.FRF(mID, mAxisString);
            return (mError == 1);
        }

        protected override bool StartMove(iMotionController.MoveToTargetMethodEnum Method, double Target)
        {
            //check ready
            if (this.StageMoving)
                return false;

            //move
            switch (Method)
            {
                case MoveToTargetMethodEnum.Absolute:
                    mError = GCS2.MOV(mID, mAxisString, new double[] { Target });
                    break;
                case MoveToTargetMethodEnum.Relative:
                    mError = GCS2.MVR(mID, mAxisString, new double[] { Target });
                    break;
            }

            //return
            return (mError == 1);
        }

        public override bool DriveEnabled
        {
            get
            {
                int[] data =null;
                mError = GCS2.qSVO(mID, mAxisString, data);
                return (mError == 1) & (data[0] == 1);
            }
            set
            {
                int[] data = new int[1];
                data[0] = 1;
                GCS2.SVO(mID, mAxisString, data);
            }
        }

        public override bool HaltMotion()
        {
            mError = GCS2.HLT(mID, mAxisString);
            return mError == 1;
        }

        public override bool KillMotion()
        {
            return this.KillMotionAllAxis();
        }

        public override bool KillMotionAllAxis()
        {
            mError = GCS2.STP(mID);
            return mError == 1;
        }
        #endregion

        #region "system error and info"

        public override string ControllerVersion
        {
            get
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder(256);

                mError = GCS2.qIDN(mID, s, s.Capacity);
                if (mError == 1)
                {
                    return s.ToString();
                }
                else
                {
                    mError = GCS2.GetError(mID);
                    return this.LastError;
                }
            }
        }

        public int LastErrorCode
        {
            get { return mError; }
        }

        public override string LastError
        {
            get
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder(1024);
                GCS2.TranslateError(mError, s, s.Capacity);
                return s.ToString();
            }
        }
        #endregion

        #region "velocity acceleration"
        public override double Acceleration
        {

            get { return 0; }

            set { }
        }

        public override double AccelerationMaximum
        {

            get { return 0; }

            set { }
        }

        public override double Deceleration
        {

            get { return 0; }

            set { }
        }

        public override double Velocity
        {

            get { return 0; }

            set { }
        }

        public override double VelocityMaximum
        {

            get { return 0; }

            set { }
        }
        #endregion
    }

    public class iPiGCS843 : iMotionController
    {

        #region "additional DLL"
        [DllImport("C843_GCS_DLL.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int C843_OpenUserStagesEditDialog(int iID);
        #endregion

        private int mID;
        private int mError;

        private string mAxisString;
        public iPiGCS843(int AxisCount) : base(AxisCount)
        {
        }

        public override bool Initialize(string sConnection, bool RaiseError)
        {
            int iBoard = 0;

            if (int.TryParse(sConnection, out iBoard))
            {
                mID = GCS843.Connect(iBoard);
            }
            else
            {
                MessageBox.Show("Invalid board number " + sConnection, "PI CGS for C843 Controller");
                return false;
            }

            if ((mID == -1) & RaiseError)
            {
                MessageBox.Show("Error connecting to PI controller at " + sConnection, "PI CGS");
                return false;
            }
            else
            {
                mError = GCS843.CST(mID, "1", "M-116.DGH");
                if (mError != 1)
                    return false;

                System.Text.StringBuilder names = new System.Text.StringBuilder();
                mError = GCS843.qCST(mID, mAxisString, names, 1024);
                if (mError != 1)
                    return false;

                mError = GCS843.INI(mID, "");
                if (mError != 1)
                    return false;

            }

            return (GCS843.IsConnected(mID) == 1);
        }

        public override void Close()
        {
            GCS843.CloseConnection(mID);
        }

        public void OpenEditDialog()
        {
            C843_OpenUserStagesEditDialog(mID);
        }

        public override bool StageReady
        {
            get
            {
                int[] data = new int[1];
                mError = GCS843.qREF(mID, mAxisString, data);
                return (mError == 1) & (data[0] == 1);
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
                mAxisString = mAxis.ToString();
            }
        }

        public override string AxisName
        {
            get { return mAxisString; }
        }

        public override double CurrentPosition
        {
            get
            {
                double[] v = new double[1];
                mError = GCS843.qPOS(mID, mAxisString, v);
                if (mError == 1)
                {
                    return v[0];
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        #region "move, home, halt"
        public override bool StageMoving
        {
            get
            {
                int[] value = new int[1];
                mError = GCS843.IsMoving(mID, mAxisString, value);

                if ((mError == 1) & (value[0] == 1))
                {
                    return true;
                }
                else
                {
                    mError = GCS843.IsReferencing(mID, mAxisString, value);
                    return (mError == 1) & (value[0] == 1);
                }
            }
        }

        public override bool InitializeMotion()
        {
            mError = GCS843.INI(mID, mAxisString);
            return (mError == 1);
        }

        protected override bool StartHome()
        {
            mError = GCS843.REF(mID, mAxisString);
            return mError == 1;
        }

        protected override bool StartMove(iMotionController.MoveToTargetMethodEnum Method, double Target)
        {
            //check ready
            if (this.StageMoving)
                return false;

            double[] value = new double[1];
            value[0] = Target;

            //move
            switch (Method)
            {
                case MoveToTargetMethodEnum.Absolute:
                    mError = GCS843.MOV(mID, mAxisString, value);
                    break;
                case MoveToTargetMethodEnum.Relative:
                    mError = GCS843.MVR(mID, mAxisString, value);
                    break;
            }

            //return
            return (mError == 1);
        }

        public override bool DriveEnabled
        {
            get
            {
                int[] data = new int[1];
                mError = GCS843.qSVO(mID, mAxisString, data);
                return (mError == 1) & (data[0] == 1);
            }
            set
            {
                int[] data = new int[1];
                data[0] = 1;
                GCS843.SVO(mID, mAxisString, data);
            }
        }

        public override bool HaltMotion()
        {
            mError = GCS843.HLT(mID, mAxisString);
            return mError == 1;
        }

        public override bool KillMotion()
        {
            return this.KillMotionAllAxis();
        }

        public override bool KillMotionAllAxis()
        {
            mError = GCS843.STP(mID);
            return mError == 1;
        }
        #endregion

        #region "system error and info"
        public override string ControllerVersion
        {
            get
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder(256);

                mError = GCS843.qIDN(mID, s, s.Capacity);
                if (mError == 1)
                {
                    return s.ToString();
                }
                else
                {
                    mError = GCS843.GetError(mID);
                    return this.LastError;
                }
            }
        }
        public int LastErrorCode
        {
            get { return mError; }
        }

        public override string LastError
        {
            get
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder(1024);
                GCS843.TranslateError(mError, s, s.Capacity);
                return s.ToString();
            }
        }
        #endregion

        #region "velocity acceleration"
        public override double Acceleration
        {

            get { return 0; }

            set { }
        }

        public override double AccelerationMaximum
        {

            get { return 0; }

            set { }
        }

        public override double Deceleration
        {

            get { return 0; }

            set { }
        }

        public override double Velocity
        {

            get { return 0; }

            set { }
        }

        public override double VelocityMaximum
        {

            get { return 0; }

            set { }
        }
        #endregion
    }

    public class iPiLS65:iMotionController
    {
        private SerialPort mPort;
        public iPiLS65(int AxisCount) : base(AxisCount)
{
        }

        public override bool Initialize(string sPort, bool RaiseError)
        {
            //set serial port
            mPort = new SerialPort(sPort, 19200, Parity.None, 8, StopBits.One);

            //open port
            try
            {
                mPort.Open();
                mPort.Handshake = Handshake.None;
                mPort.WriteTimeout = 1000;
                mPort.ReadTimeout = 5000;

            }
            catch (Exception e)
            {
                if (RaiseError)
                    MessageBox.Show(e.Message, "PI LS65", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (mPort.IsOpen)
                    mPort.Close();
                return false;
            }

            return true;
        }

        public override void Close()
        {
            mPort.Close();
        }

        public override bool StageReady
        {
            get
            {
                string reply = null;
                byte statusCode = 0;
                reply = this.QueryData("nstatus");
                statusCode = Convert.ToByte(reply);
                return (statusCode & 133) != 1;
            }
        }

        public override int Axis
        {
            get { return mAxis; }
            set { mAxis = value; }
        }

        public override string AxisName
        {
            get { return mAxis.ToString(); }
        }

        public override double CurrentPosition
        {
            get
            {
                string reply = null;
                reply = this.QueryData("npos");
                return Convert.ToDouble(reply);
            }
        }

        #region "Move, home, halt"
        public override bool StageMoving
        {
            get
            {
                string reply = null;
                byte statusCode = 0;
                reply = this.QueryData("nstatus");
                statusCode = Convert.ToByte(reply);
                return (statusCode & 1) == 1;
            }
        }

        public override bool InitializeMotion()
        {
            DriveEnabled = true;
            return true;
        }

        protected override bool StartHome()
        {
            this.SendCmd("ncal");
            return true;
        }

        protected override bool StartMove(iMotionController.MoveToTargetMethodEnum Method, double Target)
        {
            //check ready
            if (this.StageMoving)
                return false;

            double[] value = new double[1];
            value[0] = Target;

            //move
            switch (Method)
            {
                case MoveToTargetMethodEnum.Absolute:
                    this.SendCmd(Target, "nmove");
                    break;
                case MoveToTargetMethodEnum.Relative:
                    this.SendCmd(Target, "nrmove");
                    break;
            }

            //return
            return true;
        }

        public override bool DriveEnabled
        {
            get
            {
                string reply = null;
                reply = this.QueryData("getaxis");
                return Convert.ToInt32(reply) != 0;
            }
            set { this.SendCmd(Convert.ToInt32((value ? 1 : 0)), "setaxis"); }
        }

        public override bool HaltMotion()
        {
            this.SendCmd("nabort");
            return true;
        }

        public override bool KillMotion()
        {
            this.SendCmd(double.NaN, "nabort");
            return true;
        }

        public override bool KillMotionAllAxis()
        {
            this.SendCmd(double.NaN, "nabort");
            return true;
        }

        #endregion

        #region "system error and info"
        public override string ControllerVersion
        {
            get
            {
                string s = null;
                s = this.QueryData("nversion");
                return s;
            }
        }

        public int LastErrorCode
        {
            get
            {
                string s = this.QueryData("getnerror");
                return Convert.ToInt32(s);
            }
        }

        public override string LastError
        {
            get
            {
                string s = this.QueryData("getnerror");
                return s;
            }
        }
        #endregion

        #region "velocity acceleration"
        public override double Acceleration
        {
            get
            {
                string reply = null;
                reply = this.QueryData("getnaccel");
                return Convert.ToDouble(reply);
            }
            set { this.SendCmd(value, "setnaccel"); }
        }

        public override double AccelerationMaximum
        {

            get { return 0; }

            set { }
        }

        public override double Deceleration
        {

            get { return 0; }

            set { }
        }

        public override double Velocity
        {
            get
            {
                string reply = null;
                reply = this.QueryData("getnvel");
                return Convert.ToDouble(reply);
            }
            set { this.SendCmd(value, "setnvel"); }
        }

        public override double VelocityMaximum
        {

            get { return 0; }

            set { }
        }
        #endregion

        #region "serial communication"
        private void SendCmd(string cmd)
        {
            this.SendCmd(double.NaN, cmd);
        }

        private void SendCmd(int parameter, string cmd)
        {
            this.SendCmd(Convert.ToDouble(parameter), cmd);
        }

        private void SendCmd(double parameter, string cmd)
        {
            string s = null;
            if (double.IsNaN(parameter))
            {
                s = mAxis + " " + cmd;
            }
            else
            {
                s = parameter + " " + mAxis + " " + cmd;
            }

            //send data
            mPort.DiscardInBuffer();
            mPort.DiscardOutBuffer();
            mPort.WriteLine(s);

            //delay
            System.Threading.Thread.Sleep(50);
        }

        private string QueryData(double parameter, string cmd)
        {
            this.SendCmd(parameter, cmd);

            string reply = null;
            reply = mPort.ReadLine();
            return reply;
        }

        private string QueryData(string cmd)
        {
            this.SendCmd(double.NaN, cmd);

            string reply = null;
            reply = mPort.ReadLine();
            return reply;
        }
        #endregion
    }

}

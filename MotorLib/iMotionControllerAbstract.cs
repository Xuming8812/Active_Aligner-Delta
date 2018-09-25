using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delta;

namespace Instrument
{
    public abstract class iMotionController
    {
        public enum MoveToTargetMethodEnum
        {
            Absolute,
            Relative
        }

        protected int mAxisCount;
        protected int mAxis;

        public iMotionController() : this(1)
        {
        }

        public iMotionController(int AxisCount)
        {
            mAxisCount = AxisCount;
            mAxis = 1;
        }

        public abstract int Axis { get; set; }
        public abstract string AxisName { get; }
        public abstract bool Initialize(string sConnection, bool RaiseError);
        public abstract void Close();
        public abstract string ControllerVersion { get; }
        public abstract bool StageReady { get; }
        public abstract string LastError { get; }

        #region "configuration"
        public abstract bool DriveEnabled { get; set; }
        public abstract double CurrentPosition { get; }

        public abstract double Velocity { get; set; }
        public abstract double VelocityMaximum { get; set; }

        public abstract double Acceleration { get; set; }
        public abstract double Deceleration { get; set; }
        public abstract double AccelerationMaximum { get; set; }

        #endregion

        #region "motion"
        public abstract bool InitializeMotion();
        protected abstract bool StartHome();
        protected abstract bool StartMove(MoveToTargetMethodEnum Method, double Target);

        public abstract bool KillMotionAllAxis();
        public abstract bool KillMotion();
        public abstract bool HaltMotion();

        public abstract bool StageMoving { get; }

        public bool HomeNoWait()
        {
            if (this.StageMoving)
                return false;
            return this.StartHome();
        }

        public bool MoveNoWait(MoveToTargetMethodEnum Method, double Target)
        {
            if (this.StageMoving)
                return false;
            return this.StartMove(Method, Target);
        }

        public bool HomeAndWait()
        {
            if (!this.HomeNoWait())
                return false;
            System.Threading.Thread.Sleep(300);
            while (this.StageMoving)
            {
                System.Threading.Thread.Sleep(300);
            }
            return true;
        }

        public bool MoveAndWait(MoveToTargetMethodEnum Method, double Target)
        {
            if (!this.MoveNoWait(Method, Target))
                return false;
            System.Threading.Thread.Sleep(300);
            while (this.StageMoving)
            {
                System.Threading.Thread.Sleep(300);
            }
            return true;
        }
        #endregion

    }

    public abstract class iRobotController
    {


        public enum MoveToTargetMethodEnum
        {
            Absolute,
            Relative
        }

        public enum ServoStatusMethodEnum
        {
            ServoOn,
            ServoOff
        }

        protected int mAxisCount;
        protected int mAxis;

        public iRobotController() : this(1)
        {
        }

        public iRobotController(int AxisCount)
        {
            mAxisCount = AxisCount;
            mAxis = 1;
        }

        public abstract int Axis { get; set; }
        public abstract string AxisName { get; }

        public abstract bool Initialize(string sConnection, bool RaiseError);
        public abstract void Close();

        #region "configuration"
        public abstract bool DriveEnabled { set; }
        public abstract double CurrentPosition { get; }

        public abstract iRobot.Position6D Current6DPosition { get; }

        public abstract double Velocity { get; set; }
        public abstract double Acceleration { get; set; }
        public abstract ServoStatusMethodEnum ServoStatus { get; set; }
        #endregion

        #region "Motion"
        public abstract bool InitializeMotion();
        protected abstract bool StartHome();
        protected abstract bool StartMove(MoveToTargetMethodEnum Method, double Target);
        protected abstract bool StartMove(MoveToTargetMethodEnum Method, iRobot.Position6D Target);

        public abstract bool KillMotion();
        public abstract bool HaltMotion();

        public abstract bool StageMoving { get; }

        public bool HomeNoWait()
        {
            if (this.StageMoving)
                return false;
            return this.StartHome();
        }

        public bool MoveNoWait(MoveToTargetMethodEnum Method, double Target)
        {
            if (this.StageMoving)
                return false;
            return this.StartMove(Method, Target);
        }

        public bool MoveNoWait(MoveToTargetMethodEnum Method, iRobot.Position6D Target)
        {
            if (this.StageMoving)
                return false;
            return this.StartMove(Method, Target);
        }


        public bool HomeAndWait()
        {
            if (!this.HomeNoWait())
                return false;
            System.Threading.Thread.Sleep(300);
            while (this.StageMoving)
            {
                System.Threading.Thread.Sleep(300);
            }
            return true;
        }

        public bool MoveAndWait(MoveToTargetMethodEnum Method, double Target)
        {
            if (!this.MoveNoWait(Method, Target))
                return false;
            System.Threading.Thread.Sleep(300);
            while (this.StageMoving)
            {
                System.Threading.Thread.Sleep(300);
            }
            return true;
        }

        public bool MoveAndWait(MoveToTargetMethodEnum Method, iRobot.Position6D Target)
        {
            if (!this.MoveNoWait(Method, Target))
                return false;
            System.Threading.Thread.Sleep(300);
            while (this.StageMoving)
            {
                System.Threading.Thread.Sleep(300);
            }
            return true;
        }

        

        #endregion  
    }
}

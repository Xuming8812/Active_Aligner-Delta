using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.MotionControl;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.GenericMotorCLI;
using Thorlabs.MotionControl.GenericMotorCLI.ControlParameters;
using Thorlabs.MotionControl.GenericMotorCLI.AdvancedMotor;
using Thorlabs.MotionControl.GenericMotorCLI.KCubeMotor;
using Thorlabs.MotionControl.GenericMotorCLI.Settings;
using Thorlabs.MotionControl.KCube.DCServoCLI;
using System.Windows.Forms;



namespace Instrument
{
    public class iThorlabsStage: iMotionController
    {
        private ThorlabsGenericCoreDeviceCLI mDevice;
        private GenericAdvancedMotorCLI mChannel;
        string mError = "";
        string mAxisString = "";

        public iThorlabsStage(int AxisCount) : base(AxisCount)
        {
        }

        #region "Initialize and close"

        public override bool Initialize(string sConnection, bool RaiseError)
        {
            List<string> Devices = default(List<string>);
            Devices = DeviceManagerCLI.GetDeviceList();

            mDevice = null;
            string SerialNumber = sConnection;
            Devices = DeviceManagerCLI.GetDeviceList(KCubeDCServo.DevicePrefix);
            if (Devices.Contains(SerialNumber))
            {
                mDevice = KCubeDCServo.CreateKCubeDCServo(SerialNumber);
            }

            mDevice.Connect(SerialNumber);

            this.Axis = 1;

            return true;

        }

        public override void Close()
        {
            mChannel.StopPolling();
            mDevice.Disconnect(true);
        }

        #endregion

        #region "Status"

        public override string ControllerVersion
        {
            get
            {
                string s = null;
                s = "HW" + mDevice.HardwareVersion.ToString() + "SW" + mDevice.SoftwareVersion.ToString();
                return s;
            }
        }

        public override int Axis
        {
            get
            {
                return 1;
            }
            set
            {
                mChannel = (GenericAdvancedMotorCLI)mDevice;
                mChannel.StartPolling(100);
                if (!mChannel.IsSettingsInitialized())
                {
                    mChannel.WaitForSettingsInitialized(50000);
                    //MotorConfiguration x = mChannel.GetMotorConfiguration(mDevice.SerialNo);
                    //MotorDeviceSettings y = mChannel.MotorDeviceSettings;
                }
            }
        }

        public override string AxisName
        {
            get { return mAxisString; }
        }

        public override bool StageReady
        {
            get
            {
                StatusBase status = default(StatusBase);
                status = mChannel.Status;
                if (status.IsError) return false;
                if (!status.IsHomed) return false;
                if (!status.IsEnabled) return false;

                return !(status.IsMoving | status.IsHoming | status.IsInMotion);
            }
        }

        public override bool StageMoving
        {
            get
            {
                StatusBase status = default(StatusBase);
                status = mChannel.Status;
                return !(status.IsMoving | status.IsHoming | status.IsInMotion);
            }
        }

        public override string LastError
        {
            get { return mError; }
        }

        public override bool DriveEnabled
        {
            get { return mChannel.IsEnabled; }
            set
            {
                if (value)
                {
                    mChannel.EnableDevice();

                }
                else
                {
                    mChannel.DisableDevice();
                    mChannel.StopPolling();
                }
            }
        }

        public override double CurrentPosition
        {
            get
            {
                mChannel.RequestPosition();
                return Convert.ToDouble(mChannel.Position);
            }
        }

#endregion

        #region "move halt and home"

        protected override bool StartHome()
        {
            StatusBase status = default(StatusBase);
            status = mChannel.Status;

            if (status.IsEnabled && status.IsHomed) return true;
            if (!this.DriveEnabled) this.DriveEnabled = true;
            mTaskComplete = false;
            try
            {
                mTaskID = mChannel.Home(CommandCompleteFunction);
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
            catch(Exception ex)
            {
                mError = ex.Message;
                return false;
            }
            return true;
        }

        protected override bool StartMove(MoveToTargetMethodEnum Method, double target)
        {
            if (Method == MoveToTargetMethodEnum.Relative) target += this.CurrentPosition;
            mTaskComplete = false;
            try
            {
                mTaskID = mChannel.MoveTo(Convert.ToDecimal(target),CommandCompleteFunction);
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                return false;
            }

            return true;
        }

        public override bool KillMotionAllAxis()
        {
            return this.KillMotion();
        }

        public override bool KillMotion()
        {
            mChannel.StopImmediate();
            return true;
        }

        public override bool HaltMotion()
        {
            return this.KillMotion();
        }

        public override bool InitializeMotion()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region"velocity accerleration"
        public override double Velocity
        {
            get
            {
                decimal Speed = 0;
                decimal Accerleration = 0;
                mChannel.GetVelocityParams(ref Speed, ref Accerleration);
                return Convert.ToDouble(Speed);
            }
            set
            {
                VelocityParameters x = default(VelocityParameters);
                x = mChannel.GetVelocityParams();
                x.MaxVelocity = Convert.ToDecimal(value);
                mChannel.SetVelocityParams(x);
            }
        }

        public override double VelocityMaximum
        {
            get
            {
                decimal Speed = 0;
                decimal Accerleration = 0;
                mChannel.GetVelocityParams(ref Speed, ref Accerleration);
                return Convert.ToDouble(Speed);
            }
            set
            {
                VelocityParameters x = default(VelocityParameters);
                x = mChannel.GetVelocityParams();
                x.MaxVelocity = Convert.ToDecimal(value);
                mChannel.SetVelocityParams(x);
            }
        }

        public override double Acceleration
        {
            get
            {
                decimal Speed = 0;
                decimal Accerleration = 0;
                mChannel.GetVelocityParams(ref Speed, ref Accerleration);
                return Convert.ToDouble(Accerleration);
            }
            set
            {
                VelocityParameters x = default(VelocityParameters);
                x = mChannel.GetVelocityParams();
                x.Acceleration = Convert.ToDecimal(value);
                mChannel.SetVelocityParams(x);
            }
        }

        public override double AccelerationMaximum 
        {
            get
            {
                decimal Speed = 0;
                decimal Accerleration = 0;
                mChannel.GetVelocityParams(ref Speed, ref Accerleration);
                return Convert.ToDouble(Accerleration);
            }
            set
            {
                VelocityParameters x = default(VelocityParameters);
                x = mChannel.GetVelocityParams();
                x.Acceleration = Convert.ToDecimal(value);
                mChannel.SetVelocityParams(x);
            }
        }

        public override double Deceleration 
        {
            get
            {
                decimal Speed = 0;
                decimal Accerleration = 0;
                mChannel.GetVelocityParams(ref Speed, ref Accerleration);
                return Convert.ToDouble(Accerleration);
            }
            set
            {
                VelocityParameters x = default(VelocityParameters);
                x = mChannel.GetVelocityParams();
                x.Acceleration = Convert.ToDecimal(value);
                mChannel.SetVelocityParams(x);
            }
        }
        #endregion

        #region "Task complete"
        private ulong mTaskID = 0;
        private bool mTaskComplete = true;
        private void CommandCompleteFunction(ulong TaskID)
        {
            if (TaskID > 0 && (TaskID == mTaskID))
            {
                mTaskComplete = true;
                mTaskID = 0;
            }
            else
            {
                mTaskComplete = false;
            }
        }
#endregion

    }
}

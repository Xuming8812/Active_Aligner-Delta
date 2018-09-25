using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Delta
{
    public partial class DeltaFunction
    {

        private StageFunctions mStage;

        public StageFunctions StageTool
        {
            get { return mStage; }
        }

        public class StageFunctions
        {
            #region "initialization, local"
            private DeltaFunction mTool;
            private DeltaParameters mPara;
            private w2.w2MessageService mMsgData;

            private w2.w2MessageService mMsgInfo;

            private iStage mStage;
            public StageFunctions(DeltaFunction hTool)
            {
                mTool = hTool;
                mPara = hTool.mPara;
                mMsgData = hTool.mMsgData;
                mMsgInfo = hTool.mMsgInfo;

                if (mTool.mInst.StageBase != null)
                {
                    mStage = mTool.mInst.StageBase;
                }

                this.HaveSavedPosition = false;
            }

            public w2.w2MessageService MessageService
            {
                get { return mMsgInfo; }
            }
            #endregion

            #region "Simple Move"

            private void HomeStage(int axis)
            {
                bool success = false;

                mTool.Instruments.XPS.Axis = axis;

                if (mTool.Instruments.XPS.StageReady)
                    return;

                success = mTool.Instruments.XPS.InitializeMotion();
                if (success)
                    success = mTool.Instruments.XPS.HomeNoWait();

                if (success)
                {
                    this.WaitStageToStop("Wait stage homing to finish", true);
                }
                else
                {
                    this.MessageService.PostMessage("X   Motor Error: " + mTool.Instruments.XPS.LastError);
                }
            }


            public bool MoveStage1Axis(string sAxis, string sMethod, double Amount)
            {
                iStage.AxisNameEnum eAxis = default(iStage.AxisNameEnum);
                Instrument.iMotionController.MoveToTargetMethodEnum eMethod = default(Instrument.iMotionController.MoveToTargetMethodEnum);

                try
                {
                    eAxis = (iStage.AxisNameEnum) Enum.Parse(typeof(iStage.AxisNameEnum),sAxis);
                }
                catch (Exception ex)
                {
                    mMsgInfo.PostMessage("X   Invalid stage axis selection : " + sAxis + "Error: " + ex.Message);
                    return false;
                }

                try
                {
                    eMethod = (Instrument.iMotionController.MoveToTargetMethodEnum)Enum.Parse(typeof(Instrument.iMotionController.MoveToTargetMethodEnum), sMethod);

                    //eMethod = (Instrument.iMotionController.MoveToTargetMethodEnum)sMethod;
                }
                catch (Exception ex)
                {
                    mMsgInfo.PostMessage("X   Invalid stage move methid: " + sMethod + "Error: " + ex.Message);
                    return false;
                }

                return this.MoveStage1Axis(eAxis, eMethod, Amount, true);
            }

            public double GetCurrentPosition(iStage.AxisNameEnum Axis)
            {
                return mStage.GetStagePosition(Axis);
            }

            public bool MoveStage1Axis(iStage.AxisNameEnum Axis, Instrument.iMotionController.MoveToTargetMethodEnum Method, double Amount, bool ShowDetail)
            {
                string s = null;
                string StageName = null;
                double CurrentPosition = 0;
                double TargetPosition = 0;
                bool success = false;

                //ack
                StageName = mStage.GetStageName(Axis);
                s = "    Moving " + StageName;
                switch (Method)
                {
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Absolute:
                        s += " to target position " + Amount.ToString("0.0000");
                        break;
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Relative:
                        s += " relative by " + Amount.ToString("0.0000");
                        break;
                }
                mMsgData.PostMessage(s, w2.w2MessageService.MessageDisplayStyle.NewStatus);
                if (ShowDetail)
                    mMsgData.PostMessage(s);

                //get positions
                CurrentPosition = mStage.GetStagePosition(Axis);

                //check travel limit
                switch (Method)
                {
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Absolute:
                        TargetPosition = Amount;
                        break;
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Relative:
                        TargetPosition = Amount + CurrentPosition;
                        break;
                }
                success = this.ValidateStagePosition(Axis, TargetPosition);
                if (!success)
                    return false;

                //check safety
                //success = mStage.IsMoveSafe(Axis, CurrentPosition, TargetPosition)
                //If Math.Abs(CurrentPosition - TargetPosition) > 0.2 And Not success Then
                //    s = String.Format("X   Direct move from {0:0.0000} to {1:0.0000} is not safe", CurrentPosition, TargetPosition)
                //    s += ControlChars.CrLf + "      Please move the Z/Package down first to make room for the move!"
                //    mMsgData.PostMessage(s)
                //    Return False
                //End If

                //move stage now
                success = mStage.MoveStageNoWait(Axis, Method, Amount);
                if (!success)
                {
                    this.ShowStageError(StageName);
                    return false;
                }

                //wait stage move to finish
                success = this.WaitStageToStop("Wait " + StageName + " to finish the move", ShowDetail);
                if (!success)
                    return false;

                //done
                return true;
            }

            public bool MoveStageBySteps(string sAxis, double Steps, double Amount)
            {
                iStage.AxisNameEnum eAxis = default(iStage.AxisNameEnum);
                int i = 0;
                try
                {
                    eAxis = (iStage.AxisNameEnum) Enum.Parse(typeof(iStage.AxisNameEnum),sAxis);
                }
                catch (Exception ex)
                {
                    mMsgInfo.PostMessage("X   Invalid stage axis selection : " + sAxis + "Error: " + ex.Message);
                    return false;
                }

                for (i = 0; i <= Convert.ToInt32(Amount) - 1; i++)
                {
                    this.MoveStage1Axis(eAxis, Instrument.iMotionController.MoveToTargetMethodEnum.Relative, Steps, true);
                }

                return true;

            }

            public bool MoveStage3Axis(double PositionX, double PositionY, double PositionZ, bool PauseForVisualCheck)
            {
                string s = null;
                string fmt = null;
                double X0 = 0;
                double Y0 = 0;
                double Z0 = 0;
                double BeamX = 0;
                double safe = 0;
                bool success = false;

                //ack
                fmt = "      Target position X, Y, Z = ({0:0.0000}, {1:0.0000}, {2:0.0000})";
                s = string.Format(fmt, PositionX, PositionY, PositionZ);
                mMsgData.PostMessage(s);

                //recover speed
                mStage.SetStageVelocity(iStage.AxisNameEnum.StageX, 20);
                mStage.SetStageVelocity(iStage.AxisNameEnum.StageY, 20);
                mStage.SetStageVelocity(iStage.AxisNameEnum.StageZ, 20);


                //mStage.RecoverStageDefaultVelocity(iStage.AxisNameEnum.StageX)
                //mStage.RecoverStageDefaultVelocity(iStage.AxisNameEnum.StageY)
                //mStage.RecoverStageDefaultVelocity(iStage.AxisNameEnum.StageZ)

                //get current position
                X0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                Y0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                Z0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);
                BeamX = mStage.GetStagePosition(iStage.AxisNameEnum.BeamScanX);

                safe = 250;

                if (BeamX > safe)
                {
                    fmt = "      Move NanoScan to {0:0.0000} to be safe";
                    s = string.Format(fmt, safe);
                    mMsgData.PostMessage(s);
                    success = this.MoveStage1Axis(iStage.AxisNameEnum.BeamScanX, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, safe, false);
                    if (!success)
                        return false;
                    System.Threading.Thread.Sleep(300);
                }


                //raise Z first
                safe = mStage.ZforSafeMove;
                if (Z0 > safe)
                {
                    fmt = "      RAISE Z to {0:0.0000} to be safe";
                    s = string.Format(fmt, safe);
                    mMsgData.PostMessage(s);
                    success = this.MoveStage1Axis(iStage.AxisNameEnum.StageZ, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, safe, false);
                    if (!success)
                        return false;
                    System.Threading.Thread.Sleep(300);
                }



                //Move Y back next
                //safe = mStage.YforSafeMove
                //If Y0 > safe Then
                //    fmt = "      Move Y back to {0:0.0000} to be safe"
                //    s = String.Format(fmt, safe)
                //    mMsgData.PostMessage(s)
                //    success = mStage.MoveStageNoWait(iStage.AxisNameEnum.StageY, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, safe)
                //    If Not success Then
                //        Me.ShowStageError("Stage Y")
                //        Return False
                //    End If
                //    success = Me.WaitStageToStop("Wait Stage Y to finish the move", True)
                //    If Not success Then Return False
                //    System.Threading.Thread.Sleep(300)
                //End If

                //move X to target - use base function to skip the safety check
                mMsgData.PostMessage("      Move X to the target");
                success = mStage.MoveStageNoWait(iStage.AxisNameEnum.StageX, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, PositionX);
                if (!success)
                {
                    this.ShowStageError("Stage X");
                    return false;
                }
                success = this.WaitStageToStop("Wait Stage X to finish the move", true);
                if (!success)
                    return false;
                System.Threading.Thread.Sleep(300);

                //move Y to target - use base function to skip the safety check
                mMsgData.PostMessage("      Move Y to the target");
                success = mStage.MoveStageNoWait(iStage.AxisNameEnum.StageY, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, PositionY);
                if (!success)
                {
                    this.ShowStageError("Stage Y");
                    return false;
                }
                success = this.WaitStageToStop("Wait Stage Y to finish the move", true);
                if (!success)
                    return false;
                System.Threading.Thread.Sleep(300);

                //move z half way for check
                //If PauseForVisualCheck AndAlso PositionZ > mStage.ZforVisualCheck Then
                //    mMsgData.PostMessage("      Move Z to the visual check position")

                //    safe = mStage.ZforVisualCheck
                //    success = Me.MoveStage1Axis(iStage.AxisNameEnum.StageZ, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, safe, False)
                //    If Not success Then Return False

                //    'add some wait here for the stage to stable
                //    'park here for a while so that XY and really stable
                //    mTool.WaitForTime(1.0, "Wait for XY stage stable!")

                //    success = Me.ConfirmVisualCheck(New iStage.Position3D(PositionX, PositionY, PositionZ))
                //    If Not success Then Return False
                //End If

                //park here for a while so that XY and really stable
                mTool.WaitForTime(1.0, "Wait for XY stage stable!");

                //move Z back to target
                mMsgData.PostMessage("      Move Z to the target");
                success = this.MoveStage1Axis(iStage.AxisNameEnum.StageZ, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, PositionZ, false);
                if (!success)
                    return false;

                //get final position
                System.Threading.Thread.Sleep(500);
                X0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                Y0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                Z0 = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);
                fmt = "       Final position X, Y, Z = ({0:0.0000}, {1:0.0000}, {2:0.0000})";
                s = string.Format(fmt, X0, Y0, Z0);
                mMsgData.PostMessage(s);

                //done
                return true;
            }

            public bool MoveHexapod1Axis(int Axis, Instrument.iMotionController.MoveToTargetMethodEnum Method, double Amount, bool ShowDetail = true)
            {
                string s = null;
                string StageName = null;
                double CurrentPosition = 0;
                double TargetPosition = 0;
                bool success = false;

                //ack
                mStage.PiHexapod.Axis = Axis;
                StageName = "Hexapod " + mStage.PiHexapod.AxisName;
                s = "    Moving " + StageName;
                switch (Method)
                {
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Absolute:
                        s += " to target position " + Amount.ToString("0.0000");
                        break;
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Relative:
                        s += " relative by " + Amount.ToString("0.0000");
                        break;
                }
                mMsgInfo.PostMessage(s, w2.w2MessageService.MessageDisplayStyle.NewStatus);
                if (ShowDetail)
                    mMsgInfo.PostMessage(s);

                //get positions
                CurrentPosition = mStage.PiHexapod.CurrentPosition;

                //check travel limit
                switch (Method)
                {
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Absolute:
                        TargetPosition = Amount;
                        break;
                    case Instrument.iMotionController.MoveToTargetMethodEnum.Relative:
                        TargetPosition = Amount + CurrentPosition;
                        break;
                }
                //success = Me.ValidateStagePosition(Axis, TargetPosition)
                //If Not success Then Return False

                //check safety
                //success = mStage.IsMoveSafe(Axis, CurrentPosition, TargetPosition)
                //If Math.Abs(CurrentPosition - TargetPosition) > 0.2 And Not success Then
                //    s = String.Format("X   Direct move from {0:0.0000} to {1:0.0000} is not safe", CurrentPosition, TargetPosition)
                //    s += ControlChars.CrLf + "      Please move the Z/Package down first to make room for the move!"
                //    mMsgData.PostMessage(s)
                //    Return False
                //End If

                //move stage now
                success = mStage.PiHexapod.MoveAndWait(Method, Amount);
                if (!success)
                {
                    this.ShowStageError(StageName);
                    return false;
                }

                //done
                return true;
            }

            #endregion

            #region "save position and move back"
            public bool HaveSavedPosition;

            private iStage.Position3D mSavedStagePosition;
            public bool SaveCurrentStagePosition()
            {
                string s = null;
                string fmt = null;
                double height = 0;


                //ack
                mMsgData.PostMessage("    Save optimized stage position for coming back");

                //read positions
                mSavedStagePosition.X = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                mSavedStagePosition.Y = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                mSavedStagePosition.Z = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);

                //read touch Z
                height = (mPara.Alignment.StageTouchZ - mSavedStagePosition.Z) * 1000;


                //show it
                fmt = "      " + w2String.Concatenate(Convert.ToString(ControlChars.Tab), "{0,8:0.0000}", "{1,8:0.0000}", "{2,8:0.0000}");
                s = "";
                s += string.Format(fmt, "Stage X", "Stage Y", "Stage Z") + ControlChars.CrLf;
                s += string.Format(fmt, mSavedStagePosition.X, mSavedStagePosition.Y, mSavedStagePosition.Z);
                mMsgData.PostMessage(s);
                mMsgInfo.PostMessage(s);
                fmt = "       The gap between aligned position and the substrate is {0:0.00}";
                s = "";
                s += string.Format(fmt, height);
                mMsgData.PostMessage(s);
                mMsgInfo.PostMessage(s);

                //flag
                this.HaveSavedPosition = true;

                return true;
            }

            public bool MoveStageBackToSavedPosition(bool DoVisualCheck)
            {
                string s = null;
                string fmt = null;
                bool success = false;
                double x = 0;
                double y = 0;
                double z = 0;
                double v = 0;
                int i = 0;

                fmt = "      " + w2String.Concatenate(Convert.ToString(ControlChars.Tab), "{0,8:0.0000}", "{1,8:0.0000}", "{2,8:0.0000}", "{3}");

                //ack
                mMsgData.PostMessage("    Moved back to saved positions");

                //check 
                if (!this.HaveSavedPosition)
                {
                    mMsgData.PostMessage("X   No position saved previously to go back");
                    return false;
                }

                //show header and the saved data
                s = string.Format(fmt, "Stage X", "Stage Y", "Stage Z", "Action");
                s += ControlChars.CrLf;
                s += string.Format(fmt, mSavedStagePosition.X, mSavedStagePosition.Y, mSavedStagePosition.Z, "Saved Position, our target");
                mMsgData.PostMessage(s);

                //move stage XYZ, but Z to the safe position
                success = this.MoveStage3Axis(mSavedStagePosition.X, mSavedStagePosition.Y, mStage.ZforSafeMove, false);
                if (!success)
                    return false;

                //read back - note that all the read back here are added not only for information, but also for the delay to slow down the Z move
                x = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                y = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                z = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);

                s = string.Format(fmt, x, y, z, "Move stage with Z offset to the safe position");
                mMsgData.PostMessage(s);

                //move stage Z further up just below the saved saved
                v = mSavedStagePosition.Z - mPara.zSlowMove.StepCount * mPara.zSlowMove.StepSize;
                success = this.MoveStage1Axis(iStage.AxisNameEnum.StageZ, Instrument.iMotionController.MoveToTargetMethodEnum.Absolute, v, false);
                if (!success)
                    return false;

                System.Threading.Thread.Sleep(mPara.zSlowMove.StepDelay);

                x = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                y = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                z = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);

                s = string.Format(fmt, x, y, z, "Move Stage Z above package to avoid touching epoxy");
                mMsgData.PostMessage(s);

                //set Z stage to slow velocity
                mStage.SetStageVelocity(iStage.AxisNameEnum.StageZ, mPara.zSlowMove.Velocity);

                //move Z down slowly
                s = "        Move Z down in {0} steps at {1:0.000} mm per step";
                mMsgData.PostMessage(string.Format(s, mPara.zSlowMove.StepCount, mPara.zSlowMove.StepSize));
                for (i = 1; i <= mPara.zSlowMove.StepCount; i++)
                {
                    success = this.MoveStage1Axis(iStage.AxisNameEnum.StageZ, Instrument.iMotionController.MoveToTargetMethodEnum.Relative, mPara.zSlowMove.StepSize, false);
                    if (!success)
                        return false;

                    System.Threading.Thread.Sleep(mPara.zSlowMove.StepDelay);

                    x = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                    y = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                    z = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);

                    s = string.Format(fmt, x, y, z, "Moving Stage Z down slowly, Step " + i.ToString());
                    mMsgData.PostMessage(s);
                }

                //get the final XYZ
                x = mStage.GetStagePosition(iStage.AxisNameEnum.StageX);
                y = mStage.GetStagePosition(iStage.AxisNameEnum.StageY);
                z = mStage.GetStagePosition(iStage.AxisNameEnum.StageZ);

                s = string.Format(fmt, x, y, z, v, "Final Position");
                mMsgData.PostMessage(s);

                //recover the stage velocity
                mStage.SetStageVelocity(iStage.AxisNameEnum.StageZ, 20);

                //done
                return true;

            }


            #endregion

            #region "utility functions"

            private bool ValidateStagePosition(iStage.AxisNameEnum axis, double TargetPosition)
            {
                double Min = 0;
                double Max = 0;
                string s = null;

                mStage.GetStageTravelLimit(axis, ref Min, ref Max);
                s = string.Format("[{0:0.000}, {1:0.000}]", Min, Max);

                if (TargetPosition < Min)
                {
                    mMsgData.PostMessage("X   Target position below the travel limit " + s);
                    return false;
                }
                else if (TargetPosition > Max)
                {
                    mMsgData.PostMessage("X   Target position above the travel limit " + s);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public bool WaitStageToStop(string Text, bool ExtraWait)
            {
                double elapse = 0;
                System.DateTime tStart = default(System.DateTime);

                const string fmt = "{0}, elapse {1:0.00} second";

                //monitor the motion process
                tStart = System.DateTime.Now;
                while (mStage.StageMoving)
                {
                    //check stop
                    if (mTool.CheckStop())
                    {
                        //halt current one
                        mStage.HaltMotion();
                        //done
                        return false;
                    }
                    //relax and show data
                    System.Threading.Thread.Sleep(100);
                    elapse = System.DateTime.Now.Subtract(tStart).TotalSeconds;
                    mMsgData.PostMessage(string.Format(fmt, Text, elapse), w2.w2MessageService.MessageDisplayStyle.NewStatus);
                }

                //add additional wait for stage to be ready
                //While Not mStage.IsStageReadyAll
                //    'relax and show data
                //    System.Threading.Thread.Sleep(100)
                //    elapse = Date.Now.Subtract(tStart).TotalSeconds
                //    mMsgData.PostMessage(String.Format(fmt, Text, elapse), w2.w2MessageService.MessageDisplayStyle.NewStatus)
                //End While

                //extra
                if (ExtraWait)
                    System.Threading.Thread.Sleep(500);

                return true;
            }

            private void ShowStageError(string stage)
            {
                string s = null;
                s = "";
                s += ControlChars.CrLf + "X   Failed to move stage: " + stage;
                s += ControlChars.CrLf + "              Error Code: " + mStage.XPSController.LastError;
                mMsgData.PostMessage(s);
            }
            #endregion

        }

    }
}

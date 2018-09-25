using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace Instrument
{
    public class iXPS : iMotionController
    {

        //0 for debug and 1 for release
        //Const Flag As Integer = 0
        #region "XPS_C8 API"
        const int Flag = 1;
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TCP_ConnectToServer(string Ip_Address, int Ip_Port, double TimeOut);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern void TCP_SetTimeout(int SocketIndex, double TimeOut);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern void TCP_CloseSocket(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern string TCP_GetError(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern string GetLibraryVersion();
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int ErrorStringGet(int SocketIndex, int ErrorCode, string ErrorString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int FirmwareVersionGet(int SocketIndex, string Version);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int ElapsedTimeGet(int SocketIndex, double ElapsedTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int TCLScriptExecute(int SocketIndex, string TCLFileName, string TaskName, string ParametersList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TCLScriptExecuteAndWait(int SocketIndex, string TCLFileName, string TaskName, string InputParametersList, string OutputParametersList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TCLScriptKill(int SocketIndex, string TaskName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int TimerGet(int SocketIndex, string TimerName, int FrequencyTicks);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TimerSet(int SocketIndex, string TimerName, int FrequencyTicks);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int Reboot(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int EventAdd(int SocketIndex, string PositionerName, string EventName, string EventParameter, string ActionName, string ActionParameter1, string ActionParameter2, string ActionParameter3);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EventGet(int SocketIndex, string PositionerName, string EventsAndActionsList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EventRemove(int SocketIndex, string PositionerName, string EventName, string EventParameter);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EventWait(int SocketIndex, string PositionerName, string EventName, string EventParameter);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int GatheringConfigurationGet(int SocketIndex, string TypeName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringConfigurationSet(int SocketIndex, int NbElements, string TypeNameList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringCurrentNumberGet(int SocketIndex, int CurrentNumber, int MaximumSamplesNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringStopAndSave(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExternalConfigurationSet(int SocketIndex, int NbElements, string TypeNameList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExternalConfigurationGet(int SocketIndex, string TypeName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExternalCurrentNumberGet(int SocketIndex, int CurrentNumber, int MaximumSamplesNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExternalStopAndSave(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GlobalArrayGet(int SocketIndex, int Number, string ValueString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GlobalArraySet(int SocketIndex, int Number, string ValueString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int GPIOAnalogGet(int SocketIndex, int NbElements, string GPIONameList, ref double AnalogValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GPIOAnalogSet(int SocketIndex, int NbElements, string GPIONameList, ref double AnalogOutputValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GPIOAnalogGainGet(int SocketIndex, int NbElements, string GPIONameList, ref int AnalogInputGainValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GPIOAnalogGainSet(int SocketIndex, int NbElements, string GPIONameList, ref int AnalogInputGainValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GPIODigitalGet(int SocketIndex, string GPIOName, ref ushort DigitalValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GPIODigitalSet(int SocketIndex, string GPIOName, ushort Mask, ushort DigitalOutputValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int GroupAnalogTrackingModeEnable(int SocketIndex, string GroupName, string TypeName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupAnalogTrackingModeDisable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupHomeSearch(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupHomeSearchAndRelativeMove(int SocketIndex, string GroupName, int NbElements, double TargetDisplacement);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupInitialize(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupInitializeWithEncoderCalibration(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupJogParametersSet(int SocketIndex, string GroupName, int NbElements, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupJogParametersGet(int SocketIndex, string GroupName, int NbElements, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupJogCurrentGet(int SocketIndex, string GroupName, int NbElements, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupJogModeEnable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupJogModeDisable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupKill(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupMoveAbort(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupMoveAbsolute(int SocketIndex, string GroupName, int NbElements, ref double TargetPosition);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupMoveRelative(int SocketIndex, string GroupName, int NbElements, ref double TargetDisplacement);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupMotionDisable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupMotionEnable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupPositionCurrentGet(int SocketIndex, string GroupName, int NbElements, ref double CurrentEncoderPosition);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupPositionSetpointGet(int SocketIndex, string GroupName, int NbElements, double SetPointPosition);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupPositionTargetGet(int SocketIndex, string GroupName, int NbElements, double TargetPosition);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupStatusGet(int SocketIndex, string GroupName, ref int Status);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupStatusStringGet(int SocketIndex, int GroupStatusCode, string GroupStatusString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int KillAll(int SocketIndex);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int PositionerAnalogTrackingPositionParametersGet(int SocketIndex, string PositionerName, string GPIOName, double Offset, double ScaleValue, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerAnalogTrackingPositionParametersSet(int SocketIndex, string PositionerName, string GPIOName, double Offset, double ScaleValue, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerAnalogTrackingVelocityParametersGet(int SocketIndex, string PositionerName, string GPIOName, double Offset, double ScaleValue, double DeadBandThreshold, int Order, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerAnalogTrackingVelocityParametersSet(int SocketIndex, string PositionerName, string GPIOName, double Offset, double ScaleValue, double DeadBandThreshold, int Order, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerBacklashGet(int SocketIndex, string PositionerName, double BacklashValue, string BacklaskStatus);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerBacklashSet(int SocketIndex, string PositionerName, double BacklashValue);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerBacklashEnable(int SocketIndex, string PositionerName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerBacklashDisable(int SocketIndex, string PositionerName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorNotchFiltersSet(int SocketIndex, string PositionerName, double NotchFrequency1, double NotchBandwith1, double NotchGain1, double NotchFrequency2, double NotchBandwith2, double NotchGain2);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorNotchFiltersGet(int SocketIndex, string PositionerName, double NotchFrequency1, double NotchBandwith1, double NotchGain1, double NotchFrequency2, double NotchBandwith2, double NotchGain2);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDFFAccelerationSet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDFFAccelerationGet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDFFVelocitySet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainVelocity);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDFFVelocityGet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainVelocity);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDDualFFVoltageSet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainVelocity, double FeedForwardGainAcceleration, double Friction);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIDDualFFVoltageGet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double KD, double KS, double IntegrationTime, double DerivativeFilterCutOffFrequency, double GKP,
        double GKI, double GKD, double KForm, double FeedForwardGainVelocity, double FeedForwardGainAcceleration, double Friction);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIPositionSet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double IntegrationTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorPIPositionGet(int SocketIndex, string PositionerName, bool ClosedLoopStatus, double KP, double KI, double IntegrationTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCorrectorTypeGet(int SocketIndex, string PositionerName, string CorrectorType);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCurrentVelocityAccelerationFiltersSet(int SocketIndex, string PositionerName, double CurrentVelocityCutOffFrequency, double CurrentAccelerationCutOffFrequency);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerCurrentVelocityAccelerationFiltersGet(int SocketIndex, string PositionerName, double CurrentVelocityCutOffFrequency, double CurrentAccelerationCutOffFrequency);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerEncoderAmplitudeValuesGet(int SocketIndex, string PositionerName, double MaxSinusAmplitude, double CurrentSinusAmplitude, double MaxCosinusAmplitude, double CurrentCosinusAmplitude);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerEncoderCalibrationParametersGet(int SocketIndex, string PositionerName, double SinusOffset, double CosinusOffset, double DifferentialGain, double PhaseCompensation);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerErrorGet(int SocketIndex, string PositionerName, int ErrorCode);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerErrorStringGet(int SocketIndex, int PositionerErrorCode, string PositionerErrorString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerHardwareStatusGet(int SocketIndex, string PositionerName, int HardwareStatus);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerHardwareStatusStringGet(int SocketIndex, int PositionerHardwareStatus, string PositonerHardwareStatusString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerHardInterpolatorFactorGet(int SocketIndex, string PositionerName, int InterpolationFactor);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerHardInterpolatorFactorSet(int SocketIndex, string PositionerName, int InterpolationFactor);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerMaximumVelocityAndAccelerationGet(int SocketIndex, string PositionerName, ref double MaximumVelocity, ref double MaximumAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerMotionDoneGet(int SocketIndex, string PositionerName, double PositionWindow, double VelocityWindow, double CheckingTime, double MeanPeriod, double TimeOut);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerMotionDoneSet(int SocketIndex, string PositionerName, double PositionWindow, double VelocityWindow, double CheckingTime, double MeanPeriod, double TimeOut);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerPositionCompareGet(int SocketIndex, string PositionerName, double MinimumPosition, double MaximumPosition, double PositionStep, bool EnableState);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerPositionCompareSet(int SocketIndex, string PositionerName, double MinimumPosition, double MaximumPosition, double PositionStep);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerPositionCompareEnable(int SocketIndex, string PositionerName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerPositionCompareDisable(int SocketIndex, string PositionerName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerSGammaExactVelocityAjustedDisplacementGet(int SocketIndex, string PositionerName, double DesiredDisplacement, double AdjustedDisplacement);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerSGammaParametersGet(int SocketIndex, string PositionerName, ref double Velocity, ref double Acceleration, ref double MinimumTjerkTime, ref double MaximumTjerkTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerSGammaParametersSet(int SocketIndex, string PositionerName, double Velocity, double Acceleration, double MinimumTjerkTime, double MaximumTjerkTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerSGammaPreviousMotionTimesGet(int SocketIndex, string PositionerName, double SettingTime, double SettlingTime);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerUserTravelLimitsGet(int SocketIndex, string PositionerName, ref double UserMinimumTarget, ref double UserMaximumTarget);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerUserTravelLimitsSet(int SocketIndex, string PositionerName, double UserMinimumTarget, double UserMaximumTarget);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int MultipleAxesPVTVerification(int SocketIndex, string GroupName, string FileName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int MultipleAxesPVTVerificationResultGet(int SocketIndex, string PositionerName, string FileName, double MinimumPosition, double MaximumPosition, double MaximumVelocity, double MaximumAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int MultipleAxesPVTExecution(int SocketIndex, string GroupName, string FileName, int ExecutionNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int MultipleAxesPVTParametersGet(int SocketIndex, string GroupName, string FileName, int CurrentElementNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SingleAxisSlaveModeEnable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SingleAxisSlaveModeDisable(int SocketIndex, string GroupName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SingleAxisSlaveParametersSet(int SocketIndex, string GroupName, string PositionerName, double Ratio);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SingleAxisSlaveParametersGet(int SocketIndex, string GroupName, string PositionerName, double Ratio);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYLineArcVerification(int SocketIndex, string GroupName, string FileName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYLineArcVerificationResultGet(int SocketIndex, string PositionerName, string FileName, double MinimumPosition, double MaximumPosition, double MaximumVelocity, double MaximumAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYLineArcExecution(int SocketIndex, string GroupName, string FileName, double Velocity, double Acceleration, int ExecutionNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYLineArcParametersGet(int SocketIndex, string GroupName, string FileName, double Velocity, double Acceleration, int CurrentElementNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYZSplineVerification(int SocketIndex, string GroupName, string FileName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYZSplineVerificationResultGet(int SocketIndex, string PositionerName, string FileName, double MinimumPosition, double MaximumPosition, double MaximumVelocity, double MaximumAcceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYZSplineExecution(int SocketIndex, string GroupName, string FileName, double Velocity, double Acceleration);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int XYZSplineParametersGet(int SocketIndex, string GroupName, string FileName, double Velocity, double Acceleration, int CurrentElementNumber);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EEPROMCIESet(int SocketIndex, int CardNumber, string ReferenceString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EEPROMDACOffsetCIESet(int SocketIndex, int PlugNumber, double DAC1Offset, double DAC2Offset);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EEPROMDriverSet(int SocketIndex, int PlugNumber, string ReferenceString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EEPROMINTSet(int SocketIndex, int CardNumber, string ReferenceString);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int CPUCoreAndBoardSupplyVoltagesGet(int SocketIndex, double VoltageCPUCore, double SupplyVoltage1P5V, double SupplyVoltage3P3V, double SupplyVoltage5V, double SupplyVoltage12V, double SupplyVoltageM12V, double SupplyVoltageM5V, double SupplyVoltage5VSB);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int CPUTemperatureAndFanSpeedGet(int SocketIndex, double CPUTemperature, double CPUFanSpeed);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]

        private static extern int ActionListGet(int SocketIndex, string ActionList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int ActionExtendedListGet(int SocketIndex, string ActionList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int APIExtendedListGet(int SocketIndex, string Method);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int APIListGet(int SocketIndex, string Method);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int ErrorListGet(int SocketIndex, string ErrorsList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EventListGet(int SocketIndex, string EventList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EventExtendedListGet(int SocketIndex, string EventList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringListGet(int SocketIndex, string list);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExtendedListGet(int SocketIndex, string list);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringExternalListGet(int SocketIndex, string list);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GroupStatusListGet(int SocketIndex, string GroupStatusList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int HardwareInternalListGet(int SocketIndex, string InternalHardwareList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int HardwareDriverAndStageGet(int SocketIndex, int PlugNumber, string DriverName, string StageName);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int ObjectsListGet(int SocketIndex, string ObjectsList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerErrorListGet(int SocketIndex, string PositionerErrorList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int PositionerHardwareStatusListGet(int SocketIndex, string PositionerHardwareStatusList);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GatheringUserDatasGet(int SocketIndex, double UserData1, double UserData2, double UserData3, double UserData4, double UserData5, double UserData6, double UserData7, double UserData8);
        [DllImport("XPS_C8_drivers.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TestTCP(int SocketIndex, string InputString, string ReturnString);
        #endregion

        private string mAdrs;
        private int mSocketID = -1;
        private string mGroup;
        private string mPositioner;
        private int mError = 0;

        private double mTimeOut;
        public iXPS(int AxisCount) : base(AxisCount)
        {
        }

        public override bool Initialize(string IPAdrs, bool RaiseError)
        {
            return this.Initialize(IPAdrs, 5001, RaiseError);
        }

        public bool Initialize(string IPAdrs, int iPort, bool RaiseError)
        {
            //initial value for timeout - 2 seconds
            this.TimeOut = 2;

            //try connection with long timeout
            mSocketID = TCP_ConnectToServer(IPAdrs, iPort, mTimeOut);

            //error
            if ((mSocketID == -1) & RaiseError)
            {
                MessageBox.Show("Error connecting to Newport XPS at " + IPAdrs + " Port " + iPort.ToString(), "Newport XPS");
            }

            //passdown
            mAdrs = IPAdrs;

            //setup axis group name
            this.TimeOut = 0.5;
            //500ms timeout
            this.Axis = 1;

            //return
            return (mSocketID >= 0 & !string.IsNullOrEmpty(this.ControllerVersion));
        }

        public override void Close()
        {
            if (mSocketID >= 0)
                TCP_CloseSocket(mSocketID);
        }

        public string IPAddress
        {
            get { return mAdrs; }
        }

        #region "system error and info"
        public override string ControllerVersion
        {
            get
            {
                string s = Strings.Space(513);
                int p = 0;

                mError = FirmwareVersionGet(mSocketID, s);
                if (mError == 0)
                {
                    p = s.IndexOf(Strings.Chr(0));
                    if (p != -1)
                    {
                        return s.Substring(0, p);
                    }
                    else
                    {
                        return s;
                    }
                }
                else
                {
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
                int p = 0;
                string s = Strings.Space(251);

                switch (mError)
                {
                    case 0:
                        return "";
                    case -2:
                        return "Timeout";
                    default:
                        p = ErrorStringGet(mSocketID, mError, s);
                        if (p == 0)
                        {
                            p = s.IndexOf(Strings.Chr(0));
                            if (p != -1)
                            {
                                return s.Substring(0, p);
                            }
                            else
                            {
                                return s;
                            }
                        }
                        else
                        {
                            return "Have error and also failed to find the error string";
                        }
                }
            }
        }

        public string DLLVersion
        {
            get { return GetLibraryVersion(); }
        }

        public double TimeOut
        {
            get { return mTimeOut; }
            set
            {
                mTimeOut = value;
                TCP_SetTimeout(mSocketID, mTimeOut);
            }
        }

        //using the following hardware info API cause the stage in brake state 
        //and requires system reboot.

        //Public ReadOnly Property ObjectsList() As String
        //    Get
        //        Dim s As String = Space(512)
        //        mError = ObjectsListGet(mSocketID, s)
        //        If mError = 0 Then
        //            s = s.Substring(0, s.IndexOf(Chr(0)))
        //            Return s
        //        Else
        //            Return Me.LastError
        //        End If
        //    End Get
        //End Property

        //Public ReadOnly Property HardwareInfo() As String
        //    Get
        //        Dim s As String = Space(512)
        //        mError = HardwareInternalListGet(mSocketID, s)
        //        If mError = 0 Then
        //            s = s.Substring(0, s.IndexOf(Chr(0)))
        //            Return s
        //        Else
        //            Return Me.LastError
        //        End If
        //    End Get
        //End Property

        //Public ReadOnly Property HardwareInfo(ByVal iSlot As Integer) As String
        //    Get
        //        Dim sDriver As String = Space(255)
        //        Dim sStage As String = Space(255)
        //        mError = HardwareDriverAndStageGet(mSocketID, iSlot, sDriver, sStage)
        //        If mError = 0 Then
        //            sDriver = sDriver.Substring(0, sDriver.IndexOf(Chr(0)))
        //            sStage = sStage.Substring(0, sStage.IndexOf(Chr(0)))
        //            If sDriver = "" Then
        //                Return ""
        //            Else
        //                Return sDriver & "," & sStage
        //            End If
        //        Else
        //            Return ""
        //        End If
        //    End Get
        //End Property

        #endregion

        #region "Config"
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
                if (Flag == 0)
                {
                    //mGroup = "GROUP" + mAxis.ToString();
                    //mPositioner = mGroup + ".POSITIONER";
                }
                else
                {
                    mGroup = "Group" + mAxis.ToString();
                    mPositioner = mGroup + ".Pos";
                }
            }
        }

        public override string AxisName
        {
            get { return mGroup; }
        }

        public bool DriverDisabled
        {
            get
            {
                int lStatus = this.StatusCode;
                return (lStatus >= 20) & (lStatus <= 36);
            }
        }

        public override bool DriveEnabled
        {
            get { return this.StageReady; }
            set
            {
                if (value)
                {
                    mError = GroupMotionEnable(mSocketID, mGroup);
                }
                else
                {
                    mError = GroupMotionDisable(mSocketID, mGroup);
                }
            }
        }

        public override double CurrentPosition
        {
            get
            {
                int i = 0;
                const int MaxTrial = 10;

                //somehow the position will reture the end-of-motion status code
                //re-query if it is read
                double v = 0;
                i = 0;
                while (true)
                {
                    mError = GroupPositionCurrentGet(mSocketID, mGroup, 1, ref v);
                    if (mError == 0)
                    {
                        //If Not (v = 11.0# Or v = 12.0#) Then Return v
                        //loop if not
                        return v;
                    }
                    else
                    {
                        i += 1;
                        if (i == MaxTrial)
                            return double.NaN;
                    }
                }
            }
        }

        public override double Velocity
        {
            get
            {
                double v = 0;
                double a = 0;
                double tmin = 0;
                double tmax = 0;
                mError = PositionerSGammaParametersGet(mSocketID, mPositioner, ref v, ref a, ref tmin, ref tmax);
                return v;
            }
            set
            {
                double v = 0;
                double a = 0;
                double tmin = 0;
                double tmax = 0;
                //cap value
                v = this.VelocityMaximum;
                if (value > v)
                    value = v;
                //get parameters
                mError = PositionerSGammaParametersGet(mSocketID, mPositioner, ref v, ref a, ref tmin, ref tmax);
                if (mError != 0)
                    return;
                //set new parameter
                v = value;
                mError = PositionerSGammaParametersSet(mSocketID, mPositioner, v, a, tmin, tmax);
            }
        }

        public override double VelocityMaximum
        {
            get
            {
                double v = 0;
                double a = 0;
                mError = PositionerMaximumVelocityAndAccelerationGet(mSocketID, mPositioner, ref v, ref a);
                if (mError == 0)
                {
                    return v;
                }
                else
                {
                    return double.NaN;
                }
            }
            //do nothing
            set { }
        }

        public override double Acceleration
        {

            get { return 0.0; }
            //do nothing
            set { }
        }

        public override double Deceleration
        {
            get { return this.Acceleration; }
            set { this.Acceleration = value; }
        }

        public override double AccelerationMaximum
        {
            get
            {
                double v = 0;
                double a = 0;
                mError = PositionerMaximumVelocityAndAccelerationGet(mSocketID, mPositioner, ref v, ref a);
                if (mError == 0)
                {
                    return a;
                }
                else
                {
                    return double.NaN;
                }
            }
            //do nothing
            set { }
        }
        #endregion

        #region "Travel limits"
        public bool GetTravelLimit(ref double Min, ref double Max)
        {
            mError = PositionerUserTravelLimitsGet(mSocketID, mPositioner, ref Min, ref Max);
            return (mError == 0);
        }

        public bool SetTravelLimit(double Min, double Max)
        {
            mError = PositionerUserTravelLimitsSet(mSocketID, mPositioner, Min, Max);
            return (mError == 0);
        }
        #endregion

        #region "Status"
        //------------------------------------------------------------------------------------Status Code-------------------------
        //0 Not initialized state
        //1 Not initialized state due to an emergency brake : see positioner status
        //2 Not initialized state due to an emergency stop : see positioner status
        //3 Not initialized state due to a following error during homing
        //4 Not initialized state due to a following error
        //5 Not initialized state due to an homing timeout
        //6 Not initialized state due to a motion done timeout during homing
        //7 Not initialized state due to a KillAll command
        //8 Not initialized state due to an end of run after homing
        //9 Not initialized state due to an encoder calibration error
        //10 Ready state due to an AbortMove command
        //11 Ready state from homing
        //12 Ready state from motion
        //13 Ready State due to a MotionEnable command
        //14 Ready state from slave
        //15 Ready state from jogging
        //16 Ready state from analog tracking
        //17 Ready state from trajectory
        //18 Ready state from spinning
        //20 Disable state
        //21 Disabled state due to a following error on ready state
        //22 Disabled state due to a following error during motion
        //23 Disabled state due to a motion done timeout during moving
        //24 Disabled state due to a following error on slave state
        //25 Disabled state due to a following error on jogging state
        //26 Disabled state due to a following error during trajectory
        //27 Disabled state due to a motion done timeout during trajectory
        //28 Disabled state due to a following error during analog tracking
        //29 Disabled state due to a slave error during motion
        //30 Disabled state due to a slave error on slave state
        //31 Disabled state due to a slave error on jogging state
        //32 Disabled state due to a slave error during trajectory
        //33 Disabled state due to a slave error during analog tracking
        //34 Disabled state due to a slave error on ready state
        //35 Disabled state due to a following error on spinning state
        //36 Disabled state due to a slave error on spinning state
        //37 Disabled state due to a following error on auto-tuning
        //38 Disabled state due to a slave error on auto-tuning
        //40 Emergency braking
        //41 Motor initialization state
        //42 Not referenced state
        //43 Homing state
        //44 Moving state
        //45 Trajectory state
        //46 Slave state due to a SlaveEnable command
        //47 Jogging state due to a JogEnable command
        //48 Analog tracking state due to a TrackingEnable command
        //49 Analog interpolated encoder calibrating state
        //50 Not initialized state due to a mechanical zero inconsistency during homing
        //51 Spinning state due to a SpinParametersSet command

        //63 Not initialized state due to a motor initialization error
        //64 Referencing state

        //66 Not initialized state due to a perpendicularity error homing
        //67 Not initialized state due to a master/slave error during homing
        //68 Auto-tuning state
        //69 Scaling calibration state
        //70 Ready state from auto-tuning
        //71 Not initialized state from scaling calibration
        //72 Not initialized state due to a scaling calibration error
        //73 Excitation signal generation state
        //74 Disable state due to a following error on excitation signal generation state
        //75 Disable state due to a master/slave error on excitation signal generation state
        //76 Disable state due to an emergency stop on excitation signal generation state
        //77 Ready state from excitation signal generation
        //------------------------------------------------------------------------------------Status Code-------------------------

        public override bool StageReady
        {
            get
            {
                int lStatus = this.StatusCode;
                return (lStatus >= 10) & (lStatus <= 18);
            }
        }

        public int StatusCode
        {
            get
            {
                int lStatus = 0;
                while (true)
                {
                    mError = GroupStatusGet(mSocketID, mGroup, ref lStatus);
                    switch (mError)
                    {
                        case 0:
                            return lStatus;
                        case -1:
                            break;
                        //loop back
                        default:
                            return mError;
                    }
                }
            }
        }

        public string StatusString
        {
            get
            {
                int lStatus = 0;
                string s = Strings.Space(256);

                lStatus = this.StatusCode;
                if (mError == 0)
                {
                    mError = GroupStatusStringGet(mSocketID, lStatus, s);
                    if (mError == 0)
                    {
                        return s.Substring(0, s.IndexOf(Strings.Chr(0)));
                    }
                    else
                    {
                        return this.LastError;
                    }
                }
                else
                {
                    return this.LastError;
                }
            }
        }
        #endregion

        #region "Home, Move, and more"
        public override bool InitializeMotion()
        {
            mError = GroupInitialize(mSocketID, mGroup);
            return (mError == 0);
        }

        public override bool StageMoving
        {
            get
            {
                int lStatus = this.StatusCode;
                return (lStatus == 43) | (lStatus == 44);
            }
        }

        public override bool KillMotionAllAxis()
        {
            mError = KillAll(mSocketID);
            return (mError == 0);
        }

        public override bool KillMotion()
        {
            mError = GroupKill(mSocketID, mGroup);
            return (mError == 0);
        }

        public override bool HaltMotion()
        {
            int lStatus = 0;

            lStatus = this.StatusCode;
            if (mError == 0)
            {
                switch (lStatus)
                {
                    case 44:
                        //moving, then abort the motion
                        mError = GroupMoveAbort(mSocketID, mGroup);
                        break;
                    case 43:
                        //homing, then kill the motion
                        this.KillMotion();
                        break;
                    default:
                        mError = 0;
                        break;
                }
                //timeout OK
                if (mError == -2)
                    mError = 0;
                //emergency signal ok
                if (mError == -26)
                    mError = 0;
            }

            return (mError == 0);
        }

        protected override bool StartMove(iMotionController.MoveToTargetMethodEnum Method, double Target)
        {

            //check ready
            if (!this.StageReady)
                return false;

            //move
            switch (Method)
            {
                case MoveToTargetMethodEnum.Absolute:
                    mError = GroupMoveAbsolute(mSocketID, mGroup, 1, ref Target);
                    break;
                case MoveToTargetMethodEnum.Relative:
                    mError = GroupMoveRelative(mSocketID, mGroup, 1, ref Target);
                    break;
            }

            //return
            return (mError == 0) | (mError == -2);
        }

        protected override bool StartHome()
        {
            int lStatus = 0;

            //check for status
            lStatus = this.StatusCode;

            //stage is ready [10-18] or NOTREF [42], proceed
            if (((lStatus >= 10) & (lStatus <= 18)) | lStatus == 42)
            {
                mError = GroupHomeSearch(mSocketID, mGroup);
                return (mError == 0) | (mError == -2);
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region "GPIO"
        //for digital IO, valid port is 1,2,3,4 for GPIO1, GPIO2, GPIO3, and GPIO4 connectors
        //note that the bit is zero based while the Input/Output on GPIO connector is one-based
        //so there is an offset (1) bteween bit number and the Input/Output label number
        public int ReadDigitInput(int Port)
        {
            if (Port < 1)
                return -1;
            if (Port > 4)
                return -1;
            ushort Value = 0;
            mError = GPIODigitalGet(mSocketID, "GPIO" + Port + ".DI", ref Value);
            if (mError == 0)
            {
                return Convert.ToInt32(Value);
            }
            else
            {
                return -1;
            }
        }


        public bool ReadDigitInput(int Port,int Bit)
        {
                int Value = 0;
                int Mask = 0;

                Value = this.ReadDigitInput(Port);
                Mask = (1 << Bit);
                return (Value & Mask) == Mask;
        }

        public int ReadDigitOutput(int Port)
        {
            ushort v = 0;
            GPIODigitalGet(mSocketID, "GPIO" + Port + ".DO", ref v);
            return v;
        }
        public bool SetDigitOutput(int Port, int value)
        {
            return this.SetDigitalOutput(Port, 0xff, value);
        }

        //we will overload this because the XPS can set indivudal line with a mask
        public bool ReadDigitOutput(int Port,int Bit)
        {
            int v = 0;
            int mask = 0;
            v = this.ReadDigitOutput(Port);
            mask = 1 << Bit;
            return (v & mask) == mask;
        }
        //we will overload this because the XPS can set indivudal line with a mask
        public bool SetDigitOutput(int Port, int Bit,bool value)
        {
            int Mask = 0;
            int Data = 0;

            Mask = (1 << Bit);
            if (value)
            {
                Data = (1 << Bit);
            }
            else
            {
                Data = 0;
            }

            return this.SetDigitalOutput(Port, Mask, Data);
        }

      
        public bool SetDigitalOutput(int Port, int Mask, int Value)
        {
            //there are only 3 connectors/ports for output
            switch (Port)
            {
                case 1:
                case 3:
                case 4:
                    break;
                //OK
                default:
                    return false;
            }

            mError = GPIODigitalSet(mSocketID, "GPIO" + Port + ".DO", Convert.ToUInt16(Mask), Convert.ToUInt16(Value));
            return (mError == 0);
        }

        #region "range control for ADC"
        public enum ADCRangeEnum
        {
            PNError = 0,
            PN10V = 1,
            PN05V = 2,
            PN2_5V = 4,
            PN1_25V = 8
        }

        public ADCRangeEnum ReadAIRange(int Channel)
        {
            if (Channel < 1)
                return ADCRangeEnum.PNError;
            if (Channel > 4)
                return ADCRangeEnum.PNError;

            int Gain = 0;
            mError = GPIOAnalogGainGet(mSocketID, 1, "GPIO2.ADC" + Channel, ref Gain);
            if (mError == 0)
            {
                return (ADCRangeEnum)Gain;
            }
            else
            {
                return ADCRangeEnum.PNError;
            }
        }

        public void SetAIRange(int Channel, ADCRangeEnum value)
        {
            if (Channel < 1)
                return;
            if (Channel > 4)
                return;

            int Gain = Convert.ToInt32(value);
            mError = GPIOAnalogGainSet(mSocketID, 1, "GPIO2.ADC" + Channel, ref Gain);

        }

        #endregion

        public double ReadAnalogyInput(int Channel)
        {
            double Value = 0;

            if (Channel < 1)
                return double.NaN;
            if (Channel > 4)
                return double.NaN;

            mError = GPIOAnalogGet(mSocketID, 1, "GPIO2.ADC" + Channel, ref Value);
            if (mError == 0)
            {
                return Value;
            }
            else
            {
                return double.NaN;
            }
        }

        public bool SetAnalogyOutput(int Channel, double Value)
        {
            if (Channel < 1)
                return false;
            if (Channel > 4)
                return false;

            mError = GPIOAnalogSet(mSocketID, 1, "GPIO2.DAC" + Channel, ref Value);
            return (mError == 0);
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing;
using NanoScan;
using Thorlabs.BP2;
using System.Windows.Forms;
using Spiricon.BeamGage.Automation;
using Spiricon.BeamGage.Automation.Interfaces;


namespace Instrument
{
    public class iNewportNanoScan : iBeamProfiler
    {
        private NanoScan.INanoScan mNS;

        private int mError;
        public override bool Initialize(string sDataSource)
        {
            short nDevice = 0;

            //this will take time
            mNS = new NanoScan.NsAs();

            //get device
            mError = mNS.NsAsGetNumDevices(nDevice);
            if (mError != 0)
            {
                MessageBox.Show("Error initializing Nano Scan tool. Error Cdoe = " + this.GetLastError(), "Nano Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (nDevice < 1)
            {
                MessageBox.Show("No NanoScan device found", "Nano Scan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            mNS.NsAsShowWindow = true;
            mNS.NsAsDataAcquisition = true;
            System.Threading.Thread.Sleep(100);
            mNS.NsAsAutoROI = true;
            mError = mNS.NsAsAutoFind();
            mNS.NsAsDataAcquisition = true;


            return true;
        }

        public override bool Close()
        {
            mNS.NsAsDataAcquisition = false;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mNS);
            return true;
        }

        public override iBeamProfiler.SimpleData AcquireData(int Samples, DataAcquisitionMode Mode)
        {
            int iMode = 0;
            short iShort = 0;
            float v = 0;
            SimpleData data = default(SimpleData);

            const int ROI = 0;

            //select parameters to extract
            iMode = (int)NsAsParameterSelection.NSAS_SELECT_PARAM_POS_CENTR + (int)NsAsParameterSelection.NSAS_SELECT_PARAM_TOTAL_POWER + (int)NsAsParameterSelection.NSAS_SELECT_PARAM_WIDTH_1;

            if (Mode == DataAcquisitionMode.PeakWidthEnergyEllipicity)
            {
                iMode += (int)NsAsParameterSelection.NSAS_SELECT_PARAM_ELLIPTIC;
            }


            if (Mode == DataAcquisitionMode.AllSummaryData)
            {
            }


            if (Mode == DataAcquisitionMode.DataAndFrame)
            {
            }

            //set up measurement
            mError = mNS.NsAsSelectParameters(iMode);

            //set flag, zero data
            data.SampleCount = Samples;
            data.HaveFrameInfo = false;

            data.FrameData = null;
            data.DoubleData = null;

            // Power Energy Data
            data.TotalEnergy = 0;
            data.AveragePowerDensity = 0;
            data.PeakPower = 0;
            data.Minimum = 0;

            // Spatial Data
            data.CentroidX = 0;
            data.CentroidY = 0;
            data.PeakLocationX = 0;
            data.PeakLocationY = 0;
            data.D4SigmaX = 0;
            data.D4SigmaY = 0;
            data.D4SigmaDiameter = 0;
            data.FWHMX = 0;
            data.FWHMY = 0;



            data.Orientation = 0;
            data.Ellipticity = 0;
            data.Eccentricity = 0;


            for (iMode = 1; iMode <= Samples; iMode++)
            {
                mError = mNS.NsAsAcquireSync1Rev();
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For

                mError = mNS.NsAsRunComputation();
                //If mError <> 0 Then Exit For

                // Power Energy Data
                mError = mNS.NsAsGetTotalPower(v);
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For
                data.TotalEnergy += v;

                //mError = mNS.NsAsGetPower(ROI, v)
                //If mError <> 0 Then Exit For
                //data.PeakPower += v

                // Spatial Data
                mError = mNS.NsAsGetCentroidPosition(0, ROI, v);
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For
                data.CentroidX += v;

                mError = mNS.NsAsGetCentroidPosition(1, ROI, v);
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For
                data.CentroidY += v;

                //mError = mNS.NsAsGetPeakPosition(0, ROI, v)
                //If mError <> 0 Then Exit For
                //data.PeakLocationX += v

                //mError = mNS.NsAsGetPeakPosition(1, ROI, v)
                //If mError <> 0 Then Exit For
                //data.PeakLocationY += v

                //mError = mNS.NsAsGetBeamWidth4Sigma(0, ROI, v)
                //If mError <> 0 Then Exit For
                //data.D4SigmaX += v

                //mError = mNS.NsAsGetBeamWidth4Sigma(1, ROI, v)
                //If mError <> 0 Then Exit For
                //data.D4SigmaY += v

                //mError = mNS.NsAsGetBeamWidth(0, ROI, 50.0F, v)
                //If mError <> 0 Then Exit For
                //data.FWHMX += v

                //mError = mNS.NsAsGetBeamWidth(1, ROI, 50.0F, v)
                //If mError <> 0 Then Exit For
                //data.FWHMY += v

                //Add by Ming to get 13.5% Width
                mError = mNS.NsAsGetBeamWidth(0, ROI, 13.5f, v);
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For
                data.FWHMX += v;

                mError = mNS.NsAsGetBeamWidth(1, ROI, 13.5f, v);
                if (mError != 0)
                    break; // TODO: might not be correct. Was : Exit For
                data.FWHMY += v;

                //If Mode = DataAcquisitionMode.PeakWidthEnergyEllipicity Then
                //    mError = mNS.NsAsGetBeamEllipticity(ROI, v)
                //    If mError <> 0 Then Exit For
                //    data.Ellipticity += v
                //End If

                //Public AveragePowerDensity As Double
                //Public Minimum As Double
                //Public Orientation As Double
                //Public Ellipticity As Double
                //Public Eccentricity As Double

            }

            data.Valid = (mError == 0);

            //average data once we are done
            // Power Energy Data
            data.TotalEnergy /= data.SampleCount;
            data.AveragePowerDensity /= data.SampleCount;
            data.PeakPower /= data.SampleCount;
            data.Minimum /= data.SampleCount;

            // Spatial Data - convert to mm unit

            //Changed By Ming to convert to um unit
            data.CentroidX /= data.SampleCount;
            data.CentroidY /= data.SampleCount;
            //data.PeakLocationX /= data.SampleCount
            //data.PeakLocationY /= data.SampleCount
            //data.D4SigmaX /= data.SampleCount
            //data.D4SigmaY /= data.SampleCount
            data.FWHMX /= data.SampleCount;
            data.FWHMY /= data.SampleCount;
            //data.D4SigmaDiameter = Math.Sqrt(data.D4SigmaX ^ 2 + data.D4SigmaY ^ 2)

            //data.CentroidX /= (1000 * data.SampleCount)
            //data.CentroidY /= (1000 * data.SampleCount)
            //data.PeakLocationX /= (1000 * data.SampleCount)
            //data.PeakLocationY /= (1000 * data.SampleCount)
            //data.D4SigmaX /= (1000 * data.SampleCount)
            //data.D4SigmaY /= (1000 * data.SampleCount)
            //data.D4SigmaDiameter = Math.Sqrt(data.D4SigmaX ^ 2 + data.D4SigmaY ^ 2)

            data.Orientation /= data.SampleCount;
            data.Ellipticity /= data.SampleCount;
            data.Eccentricity /= data.SampleCount;

            //use radius
            //data.D4SigmaX /= 2
            //data.D4SigmaY /= 2

            //get condition info
            mError = mNS.NsAsGetGain(0, iShort);
            data.Gain = iShort;
            //data.Exposure = Me.Exposure
            //data.BlackLevel = Me.BlackLevel

            data.IsFarmeCalibrated = false;
            data.IsBaselineCalibrated = false;

            //return data
            data.TimeStamp = System.DateTime.Now;
            return data;

        }

        public override string GetLastError()
        {
            return "0x" + Convert.ToString(mError, 16);
        }

        public NanoScan.NsAsPowerUnits PowerUnit
        {
            get { return (NanoScan.NsAsPowerUnits)mNS.NsAsPowerUnits; }
            set { mNS.NsAsPowerUnits = Convert.ToInt16(value); }
        }

        #region "config"
        public override bool AutoGain
        {
            get { return false; }
            //do nothing
            set { }
        }

        public override double Gain
        {
            get
            {
                short g1 = 0;
                short g2 = 0;
                mError = mNS.NsAsGetGain(0, g1);
                mError = mNS.NsAsGetGain(1, g2);
                return 0.5 * (g1 + g2);
            }
            set
            {
                short g = 0;
                g = Convert.ToInt16(value);
                mError = mNS.NsAsSetGain(0, g);
                mError = mNS.NsAsSetGain(1, g);
            }
        }

        public override double SamplingFrequency
        {
            get
            {
                float v = 0;
                mError = mNS.NsAsGetRotationFrequency(v);
                return v;
            }
            set { mError = mNS.NsAsSetRotationFrequency(Convert.ToSingle(value)); }
        }

        public override double SamplingFrequencyMeasured
        {
            get
            {
                float v = 0;
                mError = mNS.NsAsGetMeasuredRotationFreq(v);
                return v;
            }
        }

        public override double SamplingResolution
        {
            get
            {
                float vx = 0;
                float vy = 0;
                mError = mNS.NsAsGetSamplingResolution(0, vx);
                mError = mNS.NsAsGetSamplingResolution(1, vy);
                return 0.5 * (vx + vy);
            }
            set { mError = mNS.NsAsSetSamplingResolution(Convert.ToSingle(value)); }
        }

        public override double Wavelenth_nm { get; set; }
        #endregion
    }

    public class iThorlabsBP200BeamScan : iBeamProfiler
    {


        private Thorlabs.BP2.TLBP2 mBP2;

        private int mStatus;
        public override bool Initialize(string sDataSource)
        {
            Thorlabs.BP2.bp2_device[] Devices = null;
            UInt32 iDevices = default(UInt32);

            if (!sDataSource.StartsWith("USB0::"))
            {
                mBP2 = new Thorlabs.BP2.TLBP2(new IntPtr());
                try
                {
                    mStatus = mBP2.get_connected_devices(null, out iDevices);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to get conencted Thorlabs BP200 series beam scan: " + ex.Message, "BP200", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (mStatus != 0)
                {
                    MessageBox.Show("Failed to get conencted Thorlabs BP200 series beam scan", "BP200", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (iDevices == 0)
                {
                    MessageBox.Show("No BP200 beam scan found", "BP200", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ERROR: Not supported in C#: ReDimStatement

                mStatus = mBP2.get_connected_devices(Devices, out iDevices);

                sDataSource = Devices[0].ResourceString;
                //mBP2.Dispose()
                //mBP2 = Nothing
            }

            mBP2 = new Thorlabs.BP2.TLBP2(sDataSource, false, false);
            //Me.AutoGain = True

            //Dim iError As Short
            //Dim s As New System.Text.StringBuilder
            //mStatus = mBP2.self_test(iError, s)

            ushort sampleCount = 0;
            double sampleResolution = 0;
            mStatus = mBP2.set_drum_speed_ex(10.0, out  sampleCount, out sampleResolution);
            mStatus = mBP2.set_position_correction(true);
            mStatus = mBP2.set_auto_gain(true);
            mStatus = mBP2.set_speed_correction(true);

            return true;
        }

        public override bool Close()
        {
            mBP2.Dispose();
            mBP2 = null;
            return true;
        }

        public override iBeamProfiler.SimpleData AcquireData(int Samples, iBeamProfiler.DataAcquisitionMode Mode)
        {
            short[] NewSample = new short[2];
            //Dim iStatus As UShort
            SimpleData data = default(SimpleData);
            Thorlabs.BP2.bp2_slit_data[] SlitData = new Thorlabs.BP2.bp2_slit_data[4];
            Thorlabs.BP2.bp2_calculations[] Result = new Thorlabs.BP2.bp2_calculations[4];
            double power = 0;
            double[] PowerData = new double[2128];
            float powerSaturation = 0;

            //null data
            data.Valid = false;
            data.SampleCount = Samples;
            data.HaveFrameInfo = false;
            data.FrameData = null;
            data.DoubleData = null;

            //get status
            //While True
            //    Dim deviceStatus As UShort
            //    mStatus = mBP2.get_device_status(deviceStatus)
            //    If (mStatus <> 0) Then Return data
            //    If (deviceStatus And 2) = 2 Then Exit While
            //End While

            //get daya
            try
            {
                mStatus = mBP2.get_slit_scan_data(SlitData, Result, out power, out powerSaturation, null);
                if ((mStatus != 0))
                    return data;

                //assign data
                data.Valid = true;
                data.TotalEnergy = power;

                data.CentroidX = Result[0].CentroidPos;
                data.CentroidY = Result[1].CentroidPos;

                data.PeakLocationX = Result[0].PeakPosition;
                data.PeakLocationY = Result[1].PeakPosition;
                data.PeakPower = 0.5 * (Result[0].PeakIntensity + Result[1].PeakIntensity);

                data.D4SigmaX = Result[0].BeamWidthClip;
                data.D4SigmaY = Result[1].BeamWidthClip;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //data.HaveFrameInfo = True
            //data.FrameData[0] = SlitData[0].SlitSamplesIntensities

            return data;

        }

        public override string GetLastError()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            mStatus = mBP2.error_query(out mStatus, s);
            return s.ToString();
        }

        #region "config"
        public override bool AutoGain
        {
            get
            {
                bool b = false;
                mStatus = mBP2.get_auto_gain(out b);
                return b & mStatus == 0;
            }
            set { mStatus = mBP2.set_auto_gain(value); }
        }

        public override double Gain
        {
            get
            {
                byte[] bGain = new byte[1];
                byte pGain = 0;
                mStatus = mBP2.get_gains(bGain, out pGain);
                return (bGain[0] + bGain[1] + pGain) / 3;
            }
            set
            {
                byte min = 0;
                byte max = 0;
                byte[] bGain = new byte[1];
                byte pGain = 0;

                mStatus = mBP2.get_gain_range(out min, out max);
                pGain = Convert.ToByte(value);
                if (pGain < min)
                    pGain = min;
                if (pGain > max)
                    pGain = max;
                bGain[0] = pGain;
                bGain[1] = pGain;

                mStatus = mBP2.set_gains(bGain, pGain);
            }
        }

        public override double SamplingFrequency
        {
            get
            {
                double freq = 0;
                mStatus = mBP2.get_drum_speed(out freq);
                return freq;
            }
            set
            {
                double min = 0;
                double max = 0;
                mStatus = mBP2.get_drum_speed_range(out min, out max);
                if (value < min)
                    value = min;
                if (value > max)
                    value = max;
                mStatus = mBP2.set_drum_speed(value);
            }
        }

        public override double SamplingFrequencyMeasured
        {
            get { return this.SamplingFrequency; }
        }

        public override double SamplingResolution { get; set; }



        public override double Wavelenth_nm
        {
            get
            {
                double v = 0;
                mStatus = mBP2.get_wavelength(out v);
                return v;
            }
            set
            {
                ushort min = 0;
                ushort max = 0;
                mStatus = mBP2.get_wavelength_range(out min, out max);
                if (value < min)
                    value = min;
                if (value > max)
                    value = max;
                mStatus = mBP2.set_wavelength(value);
            }
        }
        #endregion

        #region "Thorlabs specific"
        public string SerialNumber
        {
            get
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder();
                mStatus = mBP2.get_serial_number(s);
                return s.ToString();
            }
        }

        public void Reset()
        {
            mStatus = mBP2.reset();
        }

        #endregion

    }

    public abstract class iBeamProfiler
    {
        public struct DoublePoint
        {
            public DoublePoint(double X0, double Y0)
            {
                X = X0;
                Y = Y0;
            }
            public double X;
            public double Y;
        }

        public struct SimpleData
        {
            public bool Valid;

            public int SampleCount;

            public DateTime TimeStamp;
            public bool HaveFrameInfo;
            public Point FrameSize;
            public double[][] FrameData;

            public double[][] DoubleData;
            // Power Energy Data
            public double TotalEnergy;
            public double AveragePowerDensity;
            public double PeakPower;

            public double Minimum;
            // Spatial Data
            public double CentroidX;
            public double CentroidY;
            public double PeakLocationX;

            public double PeakLocationY;
            public double D4SigmaX;
            public double D4SigmaY;

            public double D4SigmaDiameter;
            public double FWHMX;

            public double FWHMY;

            public double Orientation;
            public double Ellipticity;
            public double Eccentricity;
            //Public CrossSectionArea As Double
            //Public CursorToCrosshair As Double
            //Public CentroidToCrosshair As Double

            //condition
            public double Gain;
            public double Exposure;
            public double BlackLevel;
            public bool IsFarmeCalibrated;
            public bool IsBaselineCalibrated;
        }

        public enum DataAcquisitionMode
        {
            PeakWidthEnergy,
            PeakWidthEnergyEllipicity,
            AllSummaryData,
            DataAndFrame
        }

        public abstract bool Initialize(string sDataSource);
        public abstract bool Close();
        public abstract SimpleData AcquireData(int Samples, DataAcquisitionMode Mode);
        public abstract string GetLastError();

        //config
        public abstract bool AutoGain { get; set; }
        public abstract double Gain { get; set; }
        public abstract double SamplingFrequency { get; set; }
        public abstract double SamplingFrequencyMeasured { get; }
        public abstract double SamplingResolution { get; set; }
        public abstract double Wavelenth_nm { get; set; }


    }

    public class iSpiriconBeamGage
    {

        #region "beam gage utility"
        //Class needed to instantiate and start an IV5AppServer

        private BeamGageAutomation mBeamgageAutomation;
        // Core Logic Trunk that provides access to all of the automation interfaces

        private IAutomationCLT mAutomationCLT;
        // Controls for Ultracal, SetupEGB and AutoX
        private IACalibration mIACalibration;
        // Event to wait for calibration to finish
        //Dim _calibrationStatusChanged As System.Threading.ManualResetEvent

        // Interface for data source control

        IADataSource mIADataSource;
        // On new frame events proxy for automation clients that don't fully support remoted events

        private AutomationCalibrationEvents mAutomationCalibrationEvents;
        // The interface for frame availability and frame data

        private IAFrame mIAFrame;
        // On new frame events proxy for automation clients that don't fully support remoted events

        private AutomationFrameEvents mAutomationFrameEvents;
        // Interface for control of the data source's Exposure, Gain and BlackLevel

        private IADataSourceEGB mIADataSourceEGB;
        // Interface for Enable/Disable AutoAperture

        private IAAutoAperture mIAAutoAperture;
        // Interface for retrieving Power/Energy results

        private IAResultsPowerEnergy mIAResultsPowerEnergy;
        // Interface for retrieving frame information results

        private IAResultsFrameInfo mIAResultsFrameInfo;
        // Interface for retrieving spatial analysis results

        private IAResultsSpatial mIAResultsSpatial;
        // Interface for loading and saving setups

        private IASaveLoadSetup mIASaveLoadSetup;
        // Flag to remember the staus of the Start/Stop button
        //Private mRunning As Boolean
        // Flag so that we don't try to access BGP after we call Shutdown()

        private bool mShutDown;
        //Public Delegate Sub UpdateFormDelegate()
        #endregion

        private bool mHasCal;

        private bool mIsAutoX;
        public iSpiriconBeamGage()
        {
            mShutDown = false;
        }

        public void Close()
        {
            IAutomationInstance AutomationInstance = default(IAutomationInstance);

            //Functions for control of the automation instance
            AutomationInstance = (IAutomationInstance)mAutomationCLT.GetInterfaceX("AUTOMATION_INSTANCE");

            // Flag so that we don't try to access BGP after we call Shutdown()
            mShutDown = true;

            // Remove the new frame event because we are shutting down
            mAutomationFrameEvents.OnNewFrame -= OnNewFrame;

            // Shutdown BGP when the form is closed
            AutomationInstance.Shutdown();
        }

        public bool Initialize(string sDataSource)
        {
            object x = null;
            string s = null;

            //assume it is not
            mHasCal = false;
            mIsAutoX = false;

            //Class needed to instantiate and start an IV5AppServer
            mBeamgageAutomation = new BeamGageAutomation();

            //Core Logic Trunk that provides access to all of the automation interfaces 
            //This may take quite a long time because it launches the whole BeamGage software!
            mAutomationCLT = mBeamgageAutomation.GetV5Instance("BGPVisualBasicExample4", true);

            // Interface for retrieving Power/Energy results
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_RESULTS_POWER_ENERGY");
            mIAResultsPowerEnergy = (IAResultsPowerEnergy)x;

            // Interface for retrieving frame information results
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_RESULTS_FRAME_INFO");
            mIAResultsFrameInfo = (IAResultsFrameInfo)x;

            // Interface for retrieving spatial results
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_RESULTS_SPATIAL");
            mIAResultsSpatial = (IAResultsSpatial)x;

            // Interface for data source control
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_DATA_SOURCE");
            mIADataSource = (IADataSource)x;
            s = mIADataSource.DataSource;
            if (s != sDataSource)
                mIADataSource.DataSource = sDataSource;

            //flag to prevent event OnNewFrame running
            mHaveData = true;
            //mIADataSource.Stop()

            // initialize whether BGP is running or not
            //mRunning = (dataSource.Status = ADataSourceStatus.RUNNING)

            // The interface for frame availability and frame data
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_RESULTS_PRIORITY");
            mIAFrame = (IAFrame)x;

            // The event that is called when a new frame is available
            mAutomationFrameEvents = new AutomationFrameEvents(mIAFrame);
            mAutomationFrameEvents.OnNewFrame += OnNewFrame;

            // Controls for Ultracal, SetupEGB and AutoX
            x = mAutomationCLT.GetInterfaceX("AUTOMATION_CALIBRATION");
            mIACalibration = (IACalibration)x;

            x = mAutomationCLT.GetInterfaceX("AUTOMATION_DATA_SOURCE_EGB");
            mIADataSourceEGB = (IADataSourceEGB)x;

            x = mAutomationCLT.GetInterfaceX("AUTOMATION_AUTO_APERTURE");
            mIAAutoAperture = (IAAutoAperture)x;

            x = mAutomationCLT.GetInterfaceX("AUTOMATION_SAVELOAD_SETUP");
            mIASaveLoadSetup = (IASaveLoadSetup)x;

            // The event that is called when the calibration status changes
            mAutomationCalibrationEvents = new AutomationCalibrationEvents(mIACalibration);
            mAutomationCalibrationEvents.OnCalibrationStatusChange += OnCalibrationStatusChange;

            return true;
        }

        #region "frame data, result"
        public struct SimpleData
        {

            public int SampleCount;

            public DateTime TimeStamp;
            public bool HaveFrameInfo;
            public Point FrameSize;
            public double[,] FrameData;

            public double[,] DoubleData;
            // Power Energy Data
            public double TotalEnergy;
            public double AveragePowerDensity;
            public double Peak;

            public double Minimum;
            // Spatial Data
            public double CentroidX;
            public double CentroidY;
            public double PeakLocationX;
            public double PeakLocationY;
            public double D4SigmaX;
            public double D4SigmaY;
            public double D4SigmaDiameter;
            public double Orientation;
            public double Ellipticity;
            public double Eccentricity;
            //Public CrossSectionArea As Double
            //Public CursorToCrosshair As Double
            //Public CentroidToCrosshair As Double

            //condition
            public double Gain;
            public double Exposure;
            public double BlackLevel;
            public bool IsFarmeCalibrated;
            public bool IsBaselineCalibrated;
        }

        private SimpleData mData;
        private bool mHaveData;
        private int mSampleIndex;

        private bool mTimeout;
        public void Stop()
        {
            mHaveData = true;
        }

        public bool Timeout
        {
            get { return mTimeout; }
        }

        public IAResultsPowerEnergy ResultPowerEnergy
        {
            get { return mIAResultsPowerEnergy; }
        }

        public IAResultsFrameInfo ResultFrameInfo
        {
            get { return mIAResultsFrameInfo; }
        }

        public SimpleData AcquireData(int Samples, bool TakeFramData, bool TurnOffBeamGage, double Timeout)
        {
            System.DateTime tStart = default(System.DateTime);

            //flag data
            mHaveData = false;
            mSampleIndex = 0;

            //set flag, zero data
            mData.SampleCount = Samples;
            mData.HaveFrameInfo = TakeFramData;

            // Power Energy Data
            mData.TotalEnergy = 0;
            mData.AveragePowerDensity = 0;
            mData.Peak = 0;
            mData.Minimum = 0;

            // Spatial Data
            mData.CentroidX = 0;
            mData.CentroidY = 0;
            mData.PeakLocationX = 0;
            mData.PeakLocationY = 0;
            mData.D4SigmaX = 0;
            mData.D4SigmaY = 0;
            mData.D4SigmaDiameter = 0;
            mData.Orientation = 0;
            mData.Ellipticity = 0;
            mData.Eccentricity = 0;

            //start data source
            mIADataSource.Start();

            //wait until we have data
            tStart = System.DateTime.Now;
            mTimeout = false;
            while (!mHaveData)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
                //time out
                if (System.DateTime.Now.Subtract(tStart).TotalSeconds > Timeout)
                {
                    mTimeout = true;
                    mHaveData = true;
                }
            }

            //get condition info
            mData.Gain = this.Gain;
            mData.Exposure = this.Exposure;
            mData.BlackLevel = this.BlackLevel;

            mData.IsFarmeCalibrated = this.IsFrameCalibrated;
            mData.IsBaselineCalibrated = this.IsBaselineCalibrated;

            //close
            if (TurnOffBeamGage)
                mIADataSource.Stop();

            //return data
            return mData;
        }

        private void OnNewFrame()
        {
            //falg
            if (mHaveData)
                return;

            //keep track of samples
            mSampleIndex += 1;

            // The date and time as passed as an "OLE Automation Date" and must be converted to VB Date structure
            mData.TimeStamp = System.DateTime.FromOADate(mIAResultsFrameInfo.Timestamp);

            // Display the Total PowerEnergy result
            mData.TotalEnergy += mIAResultsPowerEnergy.Total;
            mData.AveragePowerDensity += mIAResultsPowerEnergy.AveragePowerDensity;
            mData.Peak += mIAResultsPowerEnergy.Peak;
            mData.Minimum += mIAResultsPowerEnergy.Minimum;

            //other spatial info
            mData.CentroidX += mIAResultsSpatial.CentroidX;
            mData.CentroidY += mIAResultsSpatial.CentroidY;
            mData.PeakLocationX += mIAResultsSpatial.PeakLocationX;
            mData.PeakLocationY += mIAResultsSpatial.PeakLocationY;
            mData.D4SigmaX += mIAResultsSpatial.D4SigmaMajor;
            mData.D4SigmaY += mIAResultsSpatial.D4SigmaMinor;

            mData.D4SigmaDiameter += mIAResultsSpatial.D4SigmaDiameter;
            mData.Orientation += mIAResultsSpatial.Orientation;
            mData.Ellipticity += mIAResultsSpatial.Ellipticity;
            mData.Eccentricity += mIAResultsSpatial.Eccentricity;
            //mData.CrossSectionArea = mIAResultsSpatial.CrossSectionArea
            //mData.CursorToCrosshair = mIAResultsSpatial.CursorToCrosshair
            //mData.CentroidToCrosshair = mIAResultsSpatial.CentroidToCrosshair


            //average data once we are done
            if (mSampleIndex == mData.SampleCount)
            {
                // Power Energy Data
                mData.TotalEnergy /= mData.SampleCount;
                mData.AveragePowerDensity /= mData.SampleCount;
                mData.Peak /= mData.SampleCount;
                mData.Minimum /= mData.SampleCount;

                // Spatial Data
                mData.CentroidX /= (1000 * mData.SampleCount);
                mData.CentroidY /= (1000 * mData.SampleCount);
                mData.PeakLocationX /= (1000 * mData.SampleCount);
                mData.PeakLocationY /= (1000 * mData.SampleCount);
                mData.D4SigmaX /= (1000 * mData.SampleCount);
                mData.D4SigmaY /= (1000 * mData.SampleCount);
                mData.D4SigmaDiameter /= (1000 * mData.SampleCount);
                mData.Orientation /= mData.SampleCount;
                mData.Ellipticity /= mData.SampleCount;
                mData.Eccentricity /= mData.SampleCount;

                //use radius
                mData.D4SigmaX /= 2;
                mData.D4SigmaY /= 2;
            }

            //frame data - we will only take this when needed and only we get the last sample
            //             we will not do average here
            if (mData.HaveFrameInfo & mSampleIndex == mData.SampleCount)
            {
                this.GetFrameInfo();
            }

            //flag
            mHaveData = (mSampleIndex >= mData.SampleCount);
        }

        private void GetFrameInfo()
        {
            //frame size
            int i = 0;
            int j = 0;
            int idx = 0;
            int[] FrameData = null;
            double[] DoubleData = null;

            mData.FrameSize.X = Convert.ToInt32(mIAResultsFrameInfo.Width);
            mData.FrameSize.Y = Convert.ToInt32(mIAResultsFrameInfo.Height);

            mData.FrameData = new double[mData.FrameSize.X + 1, mData.FrameSize.Y + 1];
            mData.DoubleData = new double[mData.FrameSize.X + 1, mData.FrameSize.Y + 1];

            // DoubleData is a 1-dimensional array of the entire frame of data
            FrameData = mIAFrame.FrameData;
            DoubleData = mIAFrame.DoubleData;

            idx = 0;
            for (j = 0; j <= mData.FrameSize.Y - 1; j++)
            {
                for (i = 0; i <= mData.FrameSize.X - 1; i++)
                {
                    mData.FrameData[i, j] = FrameData[idx];
                    mData.DoubleData[i, j] = DoubleData[idx];
                    idx += 1;
                }
            }
        }

        public bool SaveImage(string sFile, AExportFormat format)
        {
            IAExport IAExport = default(IAExport);
            IAFrameBuffer IAFrameBuffer = default(IAFrameBuffer);

            IAExport = (IAExport)mAutomationCLT.GetInterfaceX("AUTOMATION_EXPORT");
            IAFrameBuffer = (IAFrameBuffer)mAutomationCLT.GetInterfaceX("AUTOMATION_FRAME_BUFFER");
            IAExport.Save2DImage(sFile, 0, IAFrameBuffer.Current, format);

            return true;
        }

        public bool SaveData(string sFile)
        {
            this.SaveImage(sFile, AExportFormat.ASCII);
            return true;
        }

        /// <summary>
        /// The calibration (BaselineCalibrationStatus) status for this frame
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public BaselineCalibrationStatus BaselineCalibrationStatus
        {
            get { return mIAFrame.BaselineCalibrationStatus; }
        }

        public double[] CalibratedData
        {
            get { return mIAFrame.CalibratedData; }
        }

        /// <summary>
        /// What time of Calibration has been done
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public FrameCalibrationStatus Calibration
        {
            get { return mIAFrame.Calibration; }
        }

        public bool IsBaselineCalibrated
        {
            get { return mIAFrame.IsBaselineCalibrated; }
        }

        public bool IsFrameCalibrated
        {
            get { return mIAFrame.IsFrameCalibrated; }
        }
        #endregion

        #region "Data Source"
        public string[] DataSourceList
        {
            get { return mIADataSource.DataSourceList; }
        }

        public string DataSource
        {
            get { return mIADataSource.DataSource; }
            set { mIADataSource.DataSource = value; }
        }

        public ADataSourceStatus Status
        {
            get { return mIADataSource.Status; }
        }

        #endregion

        #region "Calibration"
        /// <summary>
        /// Currently Enable, Calling will Disable. Vice Versa. UltraCal will disable AutoX
        /// </summary>
        /// <remarks></remarks>
        public bool AutoX
        {
            get
            {
                this.CheckAutoX();
                return mIsAutoX;
            }
            set
            {
                this.CheckAutoX();

                if (mIsAutoX == value)
                {
                    //do nothing 
                }
                else
                {
                    mIACalibration.AutoX();
                    mIsAutoX = !mIsAutoX;

                }
            }
        }

        private void CheckAutoX()
        {
            string s = null;
            DialogResult r = default(DialogResult);

            if (!mHasCal)
            {
                s = "Unknown AutoX state. Please check BeamGage software to see if AutoX is on. Click Yes is it is On.";
                r = MessageBox.Show(s, "BeamGage AutoX", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                mIsAutoX = r == DialogResult.Yes;
            }
        }

        public void IgnoreBeam()
        {
            mIACalibration.IgnoreBeam();
        }

        public void StartEGBSetupAsync()
        {
            System.Threading.Thread xThread = default(System.Threading.Thread);

            xThread = new System.Threading.Thread(new System.Threading.ThreadStart(SetupEGBAndWait));
            xThread.Start();

            mCalibrating = true;
        }

        /// <summary>
        /// Find optimal settings for exposure, gain and blacklevel for analysis of the beam
        /// </summary>
        /// <remarks></remarks>
        public void SetupEGBAndWait()
        {
            // this will later trigger an cal process, which will not return until the calibration is complete
            // detail is handled in the OnCalibrationStatusChange event
            mIACalibration.SetupEGB();
        }

        private bool mCalibrating;

        private bool mCalibrationFailed;
        public bool CalibrationInProgress
        {
            get { return mCalibrating; }
        }

        public bool CalibrationSuccess
        {
            get { return !mCalibrationFailed; }
        }

        public void StartCalibrationAsync()
        {
            System.Threading.Thread xThread = default(System.Threading.Thread);

            xThread = new System.Threading.Thread(new System.Threading.ThreadStart(RunCalibrationAndWait));
            xThread.Start();

            mCalibrating = true;
        }

        public void RunCalibrationAndWait()
        {
            // Calibrate - will not return until the calibration is complete
            // detail is handled in the OnCalibrationStatusChange event
            // MessageBox.Show("Please block the beam", "BeamGage Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information)
            mIACalibration.Ultracal();

            //we know for sure auto X is off
            mHasCal = true;
            mIsAutoX = false;
        }

        private void OnCalibrationStatusChange()
        {
            //Dim s As String

            //assume but failed
            mCalibrationFailed = true;

            switch (mIACalibration.Status)
            {
                case CalibrationStatus.CALIBRATING:
                    //only this flag the progress is still running
                    mCalibrating = true;

                    break;
                case CalibrationStatus.NOT_SUPPORTED:
                    MessageBox.Show("The current data source does not support calibration.", "BeamGage Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mCalibrating = false;

                    break;
                case CalibrationStatus.BEAM_DETECTED:
                    MessageBox.Show("Calibration is halted because a beam is believed to be present.", "BeamGage Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mCalibrating = false;

                    break;
                case CalibrationStatus.FAILED:
                    mCalibrating = false;
                    break;
                //do nothing, we already falged a failure

                case CalibrationStatus.READY:
                    mCalibrationFailed = false;
                    mCalibrating = false;

                    break;
            }

        }
        #endregion

        #region "Exposure, Gain and Black Level"
        public double ExposureRangeMax
        {
            get { return mIADataSourceEGB.RangeMax(EGBDesignator.EXPOSURE); }
        }

        public double ExposureRangeMin
        {
            get { return mIADataSourceEGB.RangeMin(EGBDesignator.EXPOSURE); }
        }

        public double ExposureIncrement
        {
            get { return mIADataSourceEGB.Increment(EGBDesignator.EXPOSURE); }
        }

        public string ExposureUnits
        {
            get { return mIADataSourceEGB.Units(EGBDesignator.EXPOSURE); }
        }

        public double Exposure
        {
            get { return mIADataSourceEGB.Get(EGBDesignator.EXPOSURE); }
            set
            {
                if (value < ExposureRangeMax & value > ExposureRangeMin)
                {
                    mIADataSourceEGB.Set(EGBDesignator.EXPOSURE, value);
                }
                else
                {
                    MessageBox.Show("Exposure Setting Value is out of range!");
                }
            }
        }

        public double GainRangeMax
        {
            get { return mIADataSourceEGB.RangeMax(EGBDesignator.GAIN); }
        }

        public double GainRangeMin
        {
            get { return mIADataSourceEGB.RangeMin(EGBDesignator.GAIN); }
        }

        public double GainIncrement
        {
            get { return mIADataSourceEGB.Increment(EGBDesignator.GAIN); }
        }

        public string GainUnits
        {
            get { return mIADataSourceEGB.Units(EGBDesignator.GAIN); }
        }

        public double Gain
        {
            get { return mIADataSourceEGB.Get(EGBDesignator.GAIN); }
            set
            {
                if (value < GainRangeMax & value > GainRangeMin)
                {
                    mIADataSourceEGB.Set(EGBDesignator.GAIN, value);
                }
                else
                {
                    MessageBox.Show("Gain Setting Value is out of range!");
                }
            }
        }

        public double BlackLevelRangeMax
        {
            get { return mIADataSourceEGB.RangeMax(EGBDesignator.BLACKLEVEL); }
        }

        public double BlackLevelRangeMin
        {
            get { return mIADataSourceEGB.RangeMin(EGBDesignator.BLACKLEVEL); }
        }

        public double BlackLevelIncrement
        {
            get { return mIADataSourceEGB.Increment(EGBDesignator.BLACKLEVEL); }
        }

        public string BlackLevelUnits
        {
            get { return mIADataSourceEGB.Units(EGBDesignator.BLACKLEVEL); }
        }

        public double BlackLevel
        {
            get { return mIADataSourceEGB.Get(EGBDesignator.BLACKLEVEL); }
            set
            {
                if (value < BlackLevelRangeMax & value > BlackLevelRangeMin)
                {
                    mIADataSourceEGB.Set(EGBDesignator.BLACKLEVEL, value);
                }
                else
                {
                    MessageBox.Show("Gain Setting Value is out of range!");
                }
            }
        }
        #endregion

        public bool AutoAperture
        {
            get { return mIAAutoAperture.Enabled; }
            set { mIAAutoAperture.Enabled = value; }
        }

        public void SaveSetup(string fileName)
        {
            mIASaveLoadSetup.SaveSetup(fileName);
        }

        public void LoadSetup(string filename)
        {
            mIASaveLoadSetup.LoadSetup(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delta
{
    public class DeltaParameters
    {

        private w2.w2IniFileXML mIniFile;

        public w2.w2IniFile IniFile
        {
            get { return mIniFile; }
        }

        #region "structures"
        //--------------------------------------------------------------------------------------------------Laser Diode
        public struct LaserDiodeInfo
        {
            public double DefaultCurrent;
            public double MinCurrent;
            public double MaxCurrent;
            public double CurrentStep;
            public double MinVoltage;
            public double MaxCurrentError;
        }

        private LaserDiodeInfo mLaserDiode;
        public LaserDiodeInfo LaserDiode
        {
            get { return mLaserDiode; }
        }

        //--------------------------------------------------------------------------------------------------Beam Scan parameters
        public struct BeamScanInfo
        {
            public int Samples;
            public double Gain;
            public double SampleFrequency;
            public double SampleResolution;
        }

        private BeamScanInfo mBeamScan;
        public BeamScanInfo BeamScan
        {
            get { return mBeamScan; }
        }

        public struct LightSourceInfo
        {
            public int Channel1Value;
            public int Channel2Value;
        }

        private LightSourceInfo mLightSource1;
        private LightSourceInfo mLightSource2;
        public LightSourceInfo LightSource1
        {
            get { return mLightSource1; }
        }
        public LightSourceInfo LightSource2
        {
            get { return mLightSource2; }
        }

        //--------------------------------------------------------------------------------------------------Probe clamp
        public struct ProbeClampInfo
        {
            public double Speed;
            public double Force;
            public double OpenPosition;
        }

        private ProbeClampInfo mProbeClamp;
        public ProbeClampInfo ProbeClamp
        {
            get { return mProbeClamp; }
        }

        //--------------------------------------------------------------------------------------------------Vacuum
        public struct VacuumCdaInfo
        {
            public double MinVacuumChange;
        }

        private VacuumCdaInfo mVacuumCda;
        public VacuumCdaInfo VacuumCda
        {
            get { return mVacuumCda; }
        }

        //--------------------------------------------------------------------------------------------------Z Touch Sense
        public struct zTouchSenseInfo
        {
            public int Samples;
            public double Velocity;
            public double StepSize;

            public double MaxMove;
            public double BondLineLensMin;

            public double BondLineLensMax;

            public double BondLineBS;
            public double GapForPartPickup;
            public double GapForApplyEpoxy;

            public double GapForOther;
            public double ForceChangeThreshold;
        }

        private zTouchSenseInfo mzTouchSense;
        public zTouchSenseInfo zTouchSense
        {
            get { return mzTouchSense; }
        }

        //--------------------------------------------------------------------------------------------------Z Slow Move
        public struct zSlowMoveInfo
        {
            public double Velocity;
            public double StepSize;
            public int StepCount;

            public int StepDelay;
            public double OffsetDistance
            {
                get { return StepCount * StepSize; }
            }
        }

        private zSlowMoveInfo mzSlowMove;
        public zSlowMoveInfo zSlowMove
        {
            get { return mzSlowMove; }
        }

        //--------------------------------------------------------------------------------------------------Alignment parameters
        public struct AlignmentInfo
        {
            //Public FocalLength As Double
            public double MaxPitchError;
            public double MaxSteeringY;
            public double MaxSteeringZ;
            public double MinSteeringY;
            public double MinSteeringZ;
            public double MinPower;
            public double MaxPower;
            public double MinYY;
            public double MaxYY;
            public double MinZZ;

            public double MaxZZ;
            public double StageSpeed;
            public double StageStepX;
            public double StageStepFineX;
            //added by Ming to do initial alignment for total energy
            public double StageStepXforEnergy;

            public double StageStepFineXforEnergy;

            public double bsLength;
            public double MaxStageMoveX;
            public double MaxStageMoveY;
            public double MaxStageMoveZ;

            public double MaxStageMoveAngle;
            public double BeamScanXOffset;

            public double[] BeamScanXList;
            //post epoxy adjustment
            public double PostEpoxyStepSize;

            public double postEpoxyMaxMove;
            //the following are alignment result
            public double StageMinZ;
            public double StageMaxZ;

            public double StageTouchZ;
            //public iXpsStage.Position2D[] PackageOffsetXY;

            //public double[] PackageOffsetAngle;
            //public iXpsStage.Position2D PartOffsetXY;

            //public double PartOffsetAngle;
            //public iXpsStage.Position2D PartInTrayOffsetXY;
  
            //public double PartInTrayOffsetAngle;

            //public iXpsStage.Position2D OmuxOffsetXY;

            public double CcdAngle;
            public double PinOffsetX;
            public double PinOffsetY;

            public double PinOffsetZ;
        }

        private AlignmentInfo mAlignment;
        public AlignmentInfo Alignment
        {
            get { return mAlignment; }
        }

        #endregion

        public DeltaParameters(w2.w2IniFileXML hIniFile)
        {
            mIniFile = hIniFile;
            this.ReadParameters();
            this.ClearAlignmentParameters();
        }

        #region "wrote back from outside
        public void ClearAlignmentParameters()
        {

        }

        public void UpdateStageLimitForZ(double zMin, double zMax, double zTouch)
        {
            mAlignment.StageMinZ = zMin;
            mAlignment.StageMaxZ = zMax;
            mAlignment.StageTouchZ = zTouch;
        }

        //public void UpdatePackageOffset(int Index, iXpsStage.Position2D XY, double Angle)
        //{
        //    mAlignment.PackageOffsetXY(Index) = XY;
        //    mAlignment.PackageOffsetAngle(Index) = Angle;
        //}

        //public void UpdatePartOffset(iXpsStage.Position2D XY, double Angle)
        //{
        //    mAlignment.PartOffsetXY = XY;
        //    mAlignment.PartOffsetAngle = Angle;
        //}

        //public void UpdatePartOffsetInTray(iXpsStage.Position2D XY, double Angle)
        //{
        //    mAlignment.PartInTrayOffsetXY = XY;
        //    mAlignment.PartInTrayOffsetAngle = Angle;
        //}

        //public void UpdateOmuxOffset(iXpsStage.Position2D XY, double Angle)
        //{
        //    mAlignment.OmuxOffsetXY = XY;
        //}


        #endregion

        public void ReadParameters()
        {

            string section = null;


            //because we are modifying the data in another GUI, we will force a new read from file, otherwide, the XML is still in the buffer and does not get updated
            mIniFile = new w2.w2IniFileXML(mIniFile.FileName);

            //laser diode
            section = "LaserDiode";

            mLaserDiode.DefaultCurrent = double.Parse(mIniFile.ReadParameter(section, "DefaultCurrent", Convert.ToString(85.0)));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Delta
{
   public class iStage
    {
        #region "enum and structure"
        public struct Position2D
        {
            public Position2D(double X0, double Y0)
            {
                X = X0;
                Y = Y0;
            }
            public double X;

            public double Y;
            public override string ToString()
            {
                const string s = "({0,9:0000}, {1,9:0000})";
                return string.Format(s, X, Y);
            }
        }

        public struct Position3D
        {
            public Position3D(double X0, double Y0, double Z0)
            {
                X = X0;
                Y = Y0;
                Z = Z0;
            }
            public double X;
            public double Y;

            public double Z;
            public override string ToString()
            {
                const string s = "({0,9:0000}, {1,9:0000}, {2,9:0000})";
                return string.Format(s, X, Y, Z);
            }
        }

       
        public struct Position6D
        {
            public Position6D(double X0, double Y0, double Z0, double Rx0, double Ry0, double Rz0)
            {
                X = X0;
                Y = Y0;
                Z = Z0;
                Rx = Rx0;
                Ry = Ry0;
                Rz = Rz0;
            }


            public double X;
            public double Y;
            public double Z;
            public double Rx;
            public double Ry;
            public double Rz;

            public override string ToString()
            {
                const string s = "({0,9:0000}, {1,9:0000}, {2,9:0000}, {3,9:0000}, {4,9:0000}, {5,9:0000})";
                return string.Format(s, X, Y, Z, Rx,Ry,Rz);
            }

        }

        public enum AxisNameEnum
        {
            //starte from zero, and no jump, we are using this to do the loop
            //do not change the name, it matches that in the configuration file
            //do not change the order, they are the order by Configured Position Index
            StageX = 0,
            StageY,
            StageZ,
            BeamScanX,
            BeamScanY,
            BeamScanZ,
            Probe,
            //the following two are not on XPS system, but that is fine
            AngleMain,
            AngleHexapod,
            PiLS
        }

        public static int AxisCount = Enum.GetNames(typeof(AxisNameEnum)).Length;
        private const int AlignEpoxyEnumOffset = 10;
        public enum StagePositionEnum
        {
            //do not move the order or the value of the following items, 
            //their values are used externally in scirpt

            //alignment position
            Bs1Align = PartEnum.BS1,
            PbsAlign = PartEnum.PBS,
            Bs2Align = PartEnum.BS2,

            Lens1Align = PartEnum.Lens1,
            Lens2Align = PartEnum.Lens2,
            Lens3Align = PartEnum.Lens3,
            Lens4Align = PartEnum.Lens4,

            //Epoxy - note that epoxy position is offset from alignment by 10
            Bs1Epoxy = PartEnum.BS1 + AlignEpoxyEnumOffset,
            PbsEpoxy = PartEnum.PBS +  AlignEpoxyEnumOffset,
            Bs2Epoxy = PartEnum.BS2 +  AlignEpoxyEnumOffset,

            Lens1Epoxy = PartEnum.Lens1 +  AlignEpoxyEnumOffset,
            Lens2Epoxy = PartEnum.Lens2 +  AlignEpoxyEnumOffset,
            Lens3Epoxy = PartEnum.Lens3 +  AlignEpoxyEnumOffset,
            Lens4Epoxy = PartEnum.Lens4 +  AlignEpoxyEnumOffset,

            //the other are relative random
            LoadUnload = 20,

            //work location
            LensPickup,
            HexpodPickup,
            EpoxyDump,
            EpoxyCalibration,

            //CCD view
            CcdPackage1View = 30,
            CcdPackage2View,
            CcdPackage3View,
            CcdPackage4View,
            CcdPartTopView,
            CcdPartBottomView,
            CcdEpoxyView,
            CcdOmuxView,
            CcdRechekcView,
            CcdPbsRecheckView,

            //beam scan
            BeamScanNear = 60,
            BeamScanMid,
            BeamScanFar,

            AutoFocusForLens = 71,
            AutoFocusForBS,
            AutoFocusForPin,
            AutoFocusForPickup,

            //stage safety
            YforSafeMove = 100,
            ZforSafeMove,
            ZforCheck,

            Test = 111

        }

        public enum PartEnum
        {
            BS1,
            //= BS1
            PBS,
            //= PBS
            BS2,
            //= BS2
            Lens1,
            Lens2,
            Lens3,
            Lens4
        }

        public static int PartCount = Enum.GetNames(typeof(PartEnum)).Length;

        public enum StageEnum
        {
            Main,
            Hexapod
        }
        #endregion

        #region "utility class"
        public class StageInfo
        {
            public Instrument.iMotionController Controller;
            public string Name;
            public int Axis;
            public double Home;
            public double LimitLo;
            public double LimitHi;
            public double Velocity;

            public bool Installed;
            public StageInfo()
            {
                Controller = null;
                Name = "";
            }

            public static StageInfo Parse(string sConfig)
            {
                StageInfo x = null;
                string[] data = null;

                sConfig = sConfig.Trim();
                data = sConfig.Split(ControlChars.Tab);

                try
                {
                    x = new StageInfo();
                    x.Name = data[0];
                    x.Axis = Convert.ToInt32(data[1]);
                    x.Home = Convert.ToDouble(data[2]);
                    x.LimitLo = Convert.ToDouble(data[3]);
                    x.LimitHi = Convert.ToDouble(data[4]);
                    x.Velocity = Convert.ToDouble(data[5]);
                    x.Installed = (data[6] == "1");
                    return x;
                }
                catch (Exception ex)
                {

                    string ss = ex.ToString();
                    return null;
                }

            }
        }

        public class ConfiguredStagePosition
        {
            private string mLabel;

            private double[] mPositions;
            public ConfiguredStagePosition(string Label, double[] Positions)
            {
                mLabel = Label;
                mPositions = Positions;
            }

            public string Label
            {
                get { return mLabel; }
            }

            public double[] Positions
            {
                get { return mPositions; }
            }

            public string TableString
            {
                get
                {
                    string s = null;
                    int i = 0;

                    s = mLabel;
                    for (i = 0; i <= AxisCount - 1; i++)
                    {
                        s += ControlChars.Tab;
                        if (double.IsNaN(mPositions[i]))
                        {
                            s += string.Format("{0,7:0.000}", "NA");
                        }
                        else
                        {
                            s += string.Format("{0,7:0.000}", mPositions[i]);
                        }
                    }

                    return s;
                }
            }

            public override string ToString()
            {
                return mLabel;
            }

            #region "shared functions"
            public static string TableHeader
            {
                get
                {
                    string s = null;
                    string header = null;
                    string[] names = null;
                    names = Enum.GetNames(typeof(AxisNameEnum));
                    header = "Label";
                    foreach (string s_loopVariable in names)
                    {
                        s = s_loopVariable;
                        header += ControlChars.Tab + s;
                    }
                    return header;
                }
            }

            public static ConfiguredStagePosition Parse(string s)
            {
                string[] sData = null;
                int i = 0;
                double v = 0;
                double[] Positions = new double[AxisCount];

                sData = s.Split(ControlChars.Tab);
                if (sData.Length < (AxisCount + 1))
                    return null;

                for (i = 0; i <= AxisCount - 1; i++)
                {
                    if (double.TryParse(sData[i + 1], out v))
                    {
                        Positions[i] = v;
                    }
                    else
                    {
                        Positions[i] = double.NaN;
                    }
                }

                return new ConfiguredStagePosition(sData[0].Trim(), Positions);
            }

            #endregion

        }
        #endregion

        private StageInfo[] mStageData = new StageInfo[AxisCount];
        private Dictionary<string, ConfiguredStagePosition> mConfiguredPositions;

        private Dictionary<string, ConfiguredStagePosition> mHexapodConfiguredPositions;
        private Dictionary<int, Position2D> mPartPositionInTray;

        private SafetyWindow[] mXYSafetyWindow = new SafetyWindow[PartCount];

        private Instrument.iXPS mXPS;
        private Instrument.iPiGCS mHexapod;

        private Instrument.iPiGCS843 mPI;

        private Instrument.iPiLS65 mPiLS;
        private w2.w2IniFileXML mPara;

        #region "public access"
        public Instrument.iXPS XPSController
        {
            get { return mXPS; }
        }

        public Instrument.iPiGCS843 PiAngleController
        {
            get { return mPI; }
        }

        public Instrument.iPiGCS PiHexapod
        {
            get { return mHexapod; }
        }

        public Instrument.iPiLS65 PiLS
        {
            get { return mPiLS; }
        }

        //we need to add two additional motor here
        public bool Initialize(ref w2.w2IniFileXML hConfig, ref Instrument.iXPS hXPS, ref Instrument.iPiGCS843 hPI, ref Instrument.iPiGCS hHexapod, ref Instrument.iPiLS65 hPiLS)
        {
            string s = null;
            string[] data = null;
            int index = 0;
            double v = 0;
            StageInfo x = default(StageInfo);

            mXPS = hXPS;
            mPI = hPI;
            mHexapod = hHexapod;
            mPiLS = hPiLS;
            mPara = hConfig;

            //get stage data
            s = mPara.ReadParameter("MotionTable", "StageInfo", "");
            s = s.Trim();
            data = s.Split(ControlChars.Cr);
            foreach (string s_loopVariable in data)
            {
                s = s_loopVariable;
                x = StageInfo.Parse(s);
                if (x == null)
                    continue;

                index = Convert.ToInt32(Enum.Parse(typeof(AxisNameEnum), x.Name));
                mStageData[index] = x;
            }

            //change motor speed
            foreach (StageInfo xx in mStageData)
            {
                if (xx == null)
                    continue;
                if (!xx.Installed)
                    continue;

                if (xx.Axis > 20)
                {
                    //PI motor for angle
                    xx.Controller = hPI;
                    xx.Axis -= 20;
                }
                else if (xx.Axis > 10)
                {
                    xx.Controller = hHexapod;
                    xx.Axis -= 10;
                }
                else
                {
                    xx.Controller = hXPS;
                }
             
                if (xx.Controller != null)
                {
                    xx.Controller.Axis = xx.Axis;
                    v = xx.Controller.VelocityMaximum;
                    if (xx.Velocity > v)
                        xx.Velocity = v;
                    xx.Controller.Velocity = xx.Velocity;
                }
            }

            //known stage positions and their safety window
            if (!this.ParseConfiguredPositions())
                return false;
            if (!this.ParsePositionSafetyWindow())
                return false;
            if (!this.ParseHexapodConfiguredPositions())
                return false;

            //parts position in tray
            if (!this.ParsePartPositionsInTray())
                return false;

            //validate
            foreach (StageInfo xx in mStageData)
            {
                if (xx == null)
                {
                    MessageBox.Show("Missing one or more stage configuration info!", "Motor Stage", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }

        public string GetStageName(AxisNameEnum axis)
        {
            string s = null;

            switch (axis)
            {
                case AxisNameEnum.AngleMain:
                    s = "PI Angle Stage on Main Arm";

                    break;
                case AxisNameEnum.AngleHexapod:
                    s = "PI Angle Stage from Hexapod";

                    break;
                case AxisNameEnum.PiLS:
                    s = "PI Linear Stage";

                    break;
                case AxisNameEnum.Probe:
                    s = "Probe Pin Stage";

                    break;
                case AxisNameEnum.BeamScanX:
                    s = "Beam Scan Stage X";
                    break;
                case AxisNameEnum.BeamScanY:
                    s = "Beam Scan Stage Y";
                    break;
                case AxisNameEnum.BeamScanZ:
                    s = "Beam Scan Stage Z";

                    break;
                case AxisNameEnum.StageX:
                    s = "Main Stage X";
                    break;
                case AxisNameEnum.StageY:
                    s = "Main Stage Y";
                    break;
                case AxisNameEnum.StageZ:
                    s = "Main Stage Z";

                    break;
                default:
                    s = "Unknown stage " + axis;
                    break;
            }

            return s;
        }

        public string GetStagePositionLabel(StagePositionEnum Position)
        {
            string label = null;

            label = Enum.GetName(typeof(StagePositionEnum), Position);
            label = w2String.AddSpaceBetweenWords(label);

            return label;
        }

        public StageInfo[] StageData
        {
            get { return mStageData; }
        }
        #endregion

        #region "move, position, velocity"
        public bool IsControllerReady()
        {
            string s = null;
            int i = 0;
            int ii = 0;
            int iStatus = 0;
            Instrument.iMotionController ctrl = default(Instrument.iMotionController);

            ii = AxisCount - 1;
            for (i = 0; i <= ii; i++)
            {
                //no controller, skip
                if (mStageData[i].Controller == null)
                    continue;

                //get controller
                ctrl = mStageData[i].Controller;

                //select active axis
                ctrl.Axis = mStageData[i].Axis;

                if (ctrl == mXPS)
                {
                    iStatus = mXPS.StatusCode;
                    if (!string.IsNullOrEmpty(mXPS.LastError))
                    {
                        s = "Missing controller/motor for " + this.GetStageName((AxisNameEnum)i);
                        MessageBox.Show(s);
                        return false;
                    }
                }

                //try to enable driver
                if (!ctrl.DriveEnabled)
                    ctrl.DriveEnabled = true;
                //check if it can be enabled
                if (!ctrl.StageReady)
                {
                    s = "Stage " + this.GetStageName((AxisNameEnum)i) + " is not initialized or homed";
                    MessageBox.Show(s);
                    return false;
                }
            }

            return true;
        }

        public bool IsStageReady(AxisNameEnum axis)
        {
            //not ready is no stage
            if (mStageData[(int)axis].Controller == null)
                return false;

            var _with1 = mStageData[(int)axis].Controller;
            _with1.Axis = mStageData[(int)axis].Axis;
            if (!_with1.DriveEnabled)
                _with1.DriveEnabled = true;

            return _with1.StageReady;

        }

        public bool IsStageReadyAll()
        {
            int i = 0;

            for (i = 0; i <= AxisCount - 1; i++)
            {
                if (!this.IsStageReady((AxisNameEnum)i))
                    return false;
            }
            return true;
        }

        public bool StageMoving
        {
            get
            {
                int i = 0;
                for (i = 0; i <= AxisCount - 1; i++)
                {
                    if (mStageData[i].Controller == null)
                        continue;

                    mStageData[i].Controller.Axis = mStageData[i].Axis;

                    if (mStageData[i].Controller.StageMoving)
                        return true;
                }

                return false;
            }
        }


        public void SetStageVelocity(AxisNameEnum axis, double NewVelocity)
        {
            mStageData[(int)axis].Controller.Axis = mStageData[(int)axis].Axis;
            mStageData[(int)axis].Controller.Velocity = NewVelocity;
        }

        public void SetStageAccerleration(AxisNameEnum axis, double NewAccerleration)
        {
            mStageData[(int)axis].Controller.Axis = mStageData[(int)axis].Axis;
            mStageData[(int)axis].Controller.Acceleration = NewAccerleration;
        }

        public void RecoverStageDefaultVelocity(AxisNameEnum axis)
        {
            mStageData[(int)axis].Axis = mStageData[(int)axis].Axis;
            mStageData[(int)axis].Controller.Velocity = mStageData[(int)axis].Velocity;
        }

        public double GetStagePosition(AxisNameEnum axis)
        {
            double v = 0;
            mStageData[(int)axis].Controller.Axis = mStageData[(int)axis].Axis;
            v = mStageData[(int)axis].Controller.CurrentPosition;
            return v - mStageData[(int)axis].Home;
        }

        public void GetStageTravelLimit(AxisNameEnum axis, ref double Min, ref double Max)
        {
            Min = mStageData[(int)axis].LimitLo;
            Max = mStageData[(int)axis].LimitHi;
        }

        public void HaltMotion()
        {
            int i = 0;

            //do this for all axis
            for (i = 0; i <= AxisCount - 1; i++)
            {
                if (mStageData[i].Controller == null)
                    continue;

                mStageData[i].Controller.Axis = mStageData[i].Axis;
                mStageData[i].Controller.HaltMotion();
            }

        }

        public bool MoveStageNoWait(AxisNameEnum axis, Instrument.iMotionController.MoveToTargetMethodEnum Selection, double Target)
        {
            mStageData[(int)axis].Controller.Axis = mStageData[(int)axis].Axis;
            //If Selection = Instrument.iMotionController.MoveToTargetMethodEnum.Absolute Then
            //    'for absolute move, add software home offset to the hardware offset
            //    Target += mStageData[(int)axis].Home
            //End If
            return mStageData[(int)axis].Controller.MoveNoWait(Selection, Target);
        }

        #endregion

        #region "Part position in tray"
        private bool ParsePartPositionsInTray()
        {
            string s = null;
            string[] lines = null;
            string[] data = null;
            int index = 0;
            double X = 0;
            double Y = 0;

            //get storage
            mPartPositionInTray = new Dictionary<int, Position2D>();

            //get table 
            s = mPara.ReadParameter("MotionTable", "PartTray", "");
            s = s.Trim();
            lines = s.Split(ControlChars.Cr);

            //parse table
            foreach (string s_loopVariable in lines)
            {
                s = s_loopVariable;
                s = s.Trim();
                //first line is table header
                if (s.StartsWith("Index"))
                    continue;
                if (s.StartsWith(";"))
                    continue;
                if (string.IsNullOrEmpty(s))
                    continue;

                //parse row and column - we will ignore them this time - they are hard coded in the tary class

                if (s.StartsWith("Col") || s.StartsWith("Row"))
                {
                    continue;
                }

                //parse info
                data = s.Split(ControlChars.Tab);
                try
                {
                    index = Convert.ToInt32(data[0].Trim());
                    X = Convert.ToDouble(data[1].Trim());
                    Y = Convert.ToDouble(data[2].Trim());
                }
                catch (Exception ex)
                {
                    string ss = ex.ToString();

                    s = "Error parsing part positions in the tray: " + ControlChars.CrLf + s;
                    s += ControlChars.Tab + "This entry will be ignored.";
                    MessageBox.Show(s, "Lens Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                mPartPositionInTray.Add(index, new Position2D(X, Y));
            }

            return true;
        }

        public Position2D GetPartPositionInTray(int Index)
        {
            Position2D position = default(Position2D);

            if (mPartPositionInTray.ContainsKey(Index))
            {
                position = mPartPositionInTray[Index];
            }
            else
            {
                position = new Position2D(double.NaN, double.NaN);
            }

            return position;
        }
        #endregion

        #region "configured positions"
        public bool IsAlignPosition(StagePositionEnum Position)
        {
            string s = null;
            s = Position.ToString();
            return s.EndsWith("Align");
        }

        public bool IsEpoxyPosition(StagePositionEnum Position)
        {
            string s = null;
            s = Position.ToString();
            return s.EndsWith("Epoxy");
        }

        public bool IsCcdPosition(StagePositionEnum Position)
        {
            string s = null;
            s = Position.ToString();
            return s.StartsWith("Ccd") & s.EndsWith("View");
        }

        public StagePositionEnum GetAlignmentPsotionEnum(StagePositionEnum EpoxyPositionEnum)
        {
            return (StagePositionEnum)EpoxyPositionEnum - AlignEpoxyEnumOffset;
        }

        private bool ParseConfiguredPositions()
        {
            string s = null;
            string[] data = null;
            ConfiguredStagePosition x = default(ConfiguredStagePosition);

            //New
            mConfiguredPositions = new Dictionary<string, ConfiguredStagePosition>();

            //get table
            s = mPara.ReadParameter("MotionTable", "ConfiguredPositions", "");
            s = s.Trim();
            data = s.Split(ControlChars.Cr);

            //parse table
            foreach (string s_loopVariable in data)
            {
                s = s_loopVariable;
                s = s.Trim();
                //first line is table header
                if (s == ConfiguredStagePosition.TableHeader)
                    continue;
                //parse info
                x = ConfiguredStagePosition.Parse(s);
                if (x == null)
                {
                    s = "Error parsing configuraed stage information: " + ControlChars.CrLf + s;
                    MessageBox.Show(s, "Configured Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                mConfiguredPositions.Add(x.Label, x);
            }

            return true;
        }

        public void SaveConfiguredPositions()
        {
            string table = null;
            //ConfiguredStagePosition x = default(ConfiguredStagePosition);

            table = ControlChars.CrLf + ControlChars.Tab + ConfiguredStagePosition.TableHeader;

            foreach (ConfiguredStagePosition x in mConfiguredPositions.Values)
            {
                table += ControlChars.CrLf + ControlChars.Tab + x.TableString;
            }
            table += ControlChars.CrLf + "    ";

            mPara.WriteParameter("MotionTable", "ConfiguredPositions", table);
        }

        public bool SaveConfiguredPosition(string Label, iStage.Position3D target)
        {

            double[] Positions = new double[iStage.AxisCount];
            iStage.ConfiguredStagePosition x = default(iStage.ConfiguredStagePosition);
            //iStage.ConfiguredStagePosition x2 = default(iStage.ConfiguredStagePosition);

            //get currently displayed positions
            Positions[(int)iStage.AxisNameEnum.StageX] = target.X;
            Positions[(int)iStage.AxisNameEnum.StageY] = target.Y;
            Positions[(int)iStage.AxisNameEnum.StageZ] = target.Z;

            //build a new class
            x = new iStage.ConfiguredStagePosition(Label, Positions);

            //re-ask the question to confirm 
            if (HaveConfiguredPosition(x.Label))
            {
                //update the new value
                UpdateConfiguredPosition(x);

            }

            //commit this to config file
            SaveConfiguredPositions();

            return true;
        }

        public bool HaveConfiguredPosition(string Label)
        {
            return mConfiguredPositions.ContainsKey(Label); 
        }

        public Dictionary<string, ConfiguredStagePosition> ConfiguredPositions
        {
            get { return mConfiguredPositions; }
        }

        public ConfiguredStagePosition ConfiguredPosition(StagePositionEnum Position)
        {

                string label = null;
                label = this.GetStagePositionLabel(Position);

                if (mConfiguredPositions.ContainsKey(label))
                {
                    return mConfiguredPositions[label];
                }
                else
                {
                    return null;
                }

        }

        public ConfiguredStagePosition ConfiguredPosition(string Label)
        {

                if (mConfiguredPositions.ContainsKey(Label))
                {
                    return mConfiguredPositions[Label];
                }
                else
                {
                    return null;
                }
        }

        public void AddConfiguredPosition(ConfiguredStagePosition Position)
        {
            mConfiguredPositions.Add(Position.Label, Position);
            this.SaveConfiguredPositions();
        }

        public void RemoveConfiguredPosition(ConfiguredStagePosition Position)
        {
            mConfiguredPositions.Remove(Position.Label);
            this.SaveConfiguredPositions();
        }

        public void UpdateConfiguredPosition(ConfiguredStagePosition Position)
        {
            int i = 0;
            double[] OldPositions = null;

            //get old position
            OldPositions = mConfiguredPositions[Position.Label].Positions;

            //for the new position, we will NULL the value that was NULL in the original label
            for (i = 0; i <= AxisCount - 1; i++)
            {
                if (double.IsNaN(OldPositions[i]))
                    Position.Positions[i] = double.NaN;
            }

            //update the old data with new one
            mConfiguredPositions[Position.Label] = Position;

            //save it
            this.SaveConfiguredPositions();
        }

        public Position3D GetConfiguredStagePosition(StagePositionEnum Position)
        {
            string label = null;
            double x = 0;
            double y = 0;
            double z = 0;

            //get data entry label
            label = this.GetStagePositionLabel(Position);

            //reject if label is not valid
            if (!this.HaveConfiguredPosition(label))
            {
                return new Position3D(double.NaN, double.NaN, double.NaN);
            }

            x = mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageX];
            y = mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageY];
            z = mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageZ];

            return new Position3D(x, y, z);

        }

        public Position3D GetConfiguredBeamScanPosition(StagePositionEnum Position)
        {
            string label = null;
            double x = 0;
            double y = 0;
            double z = 0;

            //get data entry label
            label = this.GetStagePositionLabel(Position);

            //reject if label is not valid
            if (!this.HaveConfiguredPosition(label))
            {
                return new Position3D(double.NaN, double.NaN, double.NaN);
            }

            x = mConfiguredPositions[label].Positions[(int)AxisNameEnum.BeamScanX];
            y = mConfiguredPositions[label].Positions[(int)AxisNameEnum.BeamScanY];
            z = mConfiguredPositions[label].Positions[(int)AxisNameEnum.BeamScanZ];

            return new Position3D(x, y, z);

        }


        private bool ParseHexapodConfiguredPositions()
        {
            string s = null;
            string[] data = null;
            ConfiguredStagePosition x = default(ConfiguredStagePosition);

            //New
            mHexapodConfiguredPositions = new Dictionary<string, ConfiguredStagePosition>();

            //get table
            s = mPara.ReadParameter("MotionTable", "HexapodConfiguredPositions", "");
            s = s.Trim();
            data = s.Split(ControlChars.Cr);

            //parse table
            foreach (string s_loopVariable in data)
            {
                s = s_loopVariable;
                s = s.Trim();
                //first line is table header
                if (s == ConfiguredStagePosition.TableHeader)
                    continue;
                //parse info
                x = ConfiguredStagePosition.Parse(s);
                if (x == null)
                {
                    s = "Error parsing configuraed stage information: " + ControlChars.CrLf + s;
                    MessageBox.Show(s, "Configured Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                mHexapodConfiguredPositions.Add(x.Label, x);
            }

            return true;
        }

        public void SaveHexapodConfiguredPositions()
        {
            string table = null;
            //ConfiguredStagePosition x = default(ConfiguredStagePosition);

            table = ControlChars.CrLf + ControlChars.Tab + ConfiguredStagePosition.TableHeader;

            foreach (ConfiguredStagePosition x in mHexapodConfiguredPositions.Values)
            {
                table += ControlChars.CrLf + ControlChars.Tab + x.TableString;
            }
            table += ControlChars.CrLf + "    ";

            mPara.WriteParameter("MotionTable", "HexapodConfiguredPositions", table);
        }

        public bool HaveHexapodConfiguredPosition(string Label)
        {
            return mHexapodConfiguredPositions.ContainsKey(Label); 
        }

        public Dictionary<string, ConfiguredStagePosition> HexapodConfiguredPositions
        {
            get { return mHexapodConfiguredPositions; }
        }

        public ConfiguredStagePosition HexapodConfiguredPosition(StagePositionEnum Position)
        {

                string label = null;
                label = this.GetStagePositionLabel(Position);

                if (mHexapodConfiguredPositions.ContainsKey(label))
                {
                    return mHexapodConfiguredPositions[label];
                }
                else
                {
                    return null;
                }

        }

        public ConfiguredStagePosition HexapodConfiguredPosition(string Label)
        {

                if (mHexapodConfiguredPositions.ContainsKey(Label))
                {
                    return mHexapodConfiguredPositions[Label];
                }
                else
                {
                    return null;
                }
        }

        public void AddHexapodConfiguredPosition(ConfiguredStagePosition Position)
        {
            mHexapodConfiguredPositions.Add(Position.Label, Position);
            this.SaveHexapodConfiguredPositions();
        }

        public void UpdateHexapodConfiguredPosition(ConfiguredStagePosition Position)
        {
            int i = 0;
            double[] OldPositions = null;

            //get old position
            OldPositions = mHexapodConfiguredPositions[Position.Label].Positions;

            //for the new position, we will NULL the value that was NULL in the original label
            for (i = 0; i <= AxisCount - 1; i++)
            {
                if (double.IsNaN(OldPositions[i]))
                    Position.Positions[i] = double.NaN;
            }

            //update the old data with new one
            mHexapodConfiguredPositions[Position.Label] = Position;

            //save it
            this.SaveHexapodConfiguredPositions();
        }

        public Position3D GetConfiguredHexapodPosition(StagePositionEnum Position)
        {
            string label = null;
            double x = 0;
            double y = 0;
            double z = 0;

            //get data entry label
            label = this.GetStagePositionLabel(Position);

            //reject if label is not valid
            if (!this.HaveHexapodConfiguredPosition(label))
            {
                return new Position3D(double.NaN, double.NaN, double.NaN);
            }

            x = mHexapodConfiguredPositions[label].Positions[(int)AxisNameEnum.StageX];
            y = mHexapodConfiguredPositions[label].Positions[(int)AxisNameEnum.StageY];
            z = mHexapodConfiguredPositions[label].Positions[(int)AxisNameEnum.StageZ];

            return new Position3D(x, y, z);

        }
        #endregion

        #region "safty window"
        public class SafetyWindow
        {
            private double mXmin;
            private double mXmax;
            private double mYmin;

            private double mYmax;
            public SafetyWindow(double NominalX, double NominalY, double XMinus, double XPlus, double YMinus, double YPlus)
            {
                mXmin = NominalX - XMinus;
                mXmax = NominalX + XPlus;
                mYmin = NominalY - YMinus;
                mYmax = NominalY + YPlus;
            }

            public bool Valid
            {
                get
                {
                    if (double.IsNaN(mXmin))
                        return false;
                    if (double.IsNaN(mXmax))
                        return false;
                    if (double.IsNaN(mYmin))
                        return false;
                    if (double.IsNaN(mYmax))
                        return false;
                    return true;
                }
            }

            public bool IsInsideWindow(Position2D Position)
            {
                if (Position.X < mXmin)
                    return false;
                if (Position.X > mXmax)
                    return false;
                if (Position.Y < mYmin)
                    return false;
                if (Position.Y > mYmax)
                    return false;
                return true;
            }

            public bool IsBothInsideWindow(Position2D Position1, Position2D Position2)
            {
                if (!this.IsInsideWindow(Position1))
                    return false;
                if (!this.IsInsideWindow(Position2))
                    return false;
                return true;
            }
        }

        private bool ParsePositionSafetyWindow()
        {
            int i = 0;
            string sKey = null;
            StagePositionEnum ePosition = default(StagePositionEnum);
            Position3D NominalPosition = default(Position3D);
            double XMinus = 0;
            double XPlus = 0;
            double YMinus = 0;
            double YPlus = 0;

            for (i = 0; i <= PartCount - 1; i++)
            {
                //since we made the alignment postion enum the same value as part enum, we can do simple type change here
                ePosition = (StagePositionEnum)i;
                NominalPosition = this.GetConfiguredStagePosition(ePosition);

                //get window 
                sKey = Enum.GetName(typeof(PartEnum), ePosition);
                sKey += "SafetyWindow";
                XMinus = Convert.ToDouble(mPara.ReadParameter(sKey, "Xminus", Convert.ToString(0.1)));
                XPlus = Convert.ToDouble(mPara.ReadParameter(sKey, "Xplus", Convert.ToString(0.1)));
                YMinus = Convert.ToDouble(mPara.ReadParameter(sKey, "Yminus", Convert.ToString(0.1)));
                YPlus = Convert.ToDouble(mPara.ReadParameter(sKey, "Yplus", Convert.ToString(0.1)));

                //XPlus = mPara.ReadParameter(sKey, "Xplus", 0.2);
                //YMinus = mPara.ReadParameter(sKey, "Yminus", 0.1);
                //YPlus = mPara.ReadParameter(sKey, "Yplus", 0.1);

                //build class
                mXYSafetyWindow[i] = new SafetyWindow(NominalPosition.X, NominalPosition.Y, XMinus, XPlus, YMinus, YPlus);
            }
            return true;
        }

        public double YforSafeMove
        {
            get
            {
                string label = null;
                label = this.GetStagePositionLabel(StagePositionEnum.YforSafeMove);
                return mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageY];
            }
        }

        public double ZforSafeMove
        {
            get
            {
                string label = null;
                label = this.GetStagePositionLabel(StagePositionEnum.ZforSafeMove);
                return mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageZ];
            }
        }

        public double ZforVisualCheck
        {
            get
            {
                string label = null;
                label = this.GetStagePositionLabel(StagePositionEnum.ZforCheck);
                return mConfiguredPositions[label].Positions[(int)AxisNameEnum.StageZ];
            }
        }

        public bool IsMoveSafe(AxisNameEnum Axis, double Position1, double Position2)
        {
            Position2D P1 = default(Position2D);
            Position2D P2 = default(Position2D);
            double v = 0;

            //check Z first, if Z is low, it is fine
            v = this.GetStagePosition(AxisNameEnum.StageZ);
            if (v <= this.ZforSafeMove)
                return true;

            //Z is high, check window
            switch (Axis)
            {
                case AxisNameEnum.StageX:
                    v = this.GetStagePosition(AxisNameEnum.StageY);
                    P1 = new Position2D(Position1, v);
                    P2 = new Position2D(Position2, v);

                    return this.IsMoveSafe(P1, P2);
                case AxisNameEnum.StageY:
                    v = this.GetStagePosition(AxisNameEnum.StageX);
                    P1 = new Position2D(v, Position1);
                    P2 = new Position2D(v, Position2);

                    return this.IsMoveSafe(P1, P2);
                default:
                    //all the other axis are presumbly safe
                    return true;
            }

        }

        public bool IsMoveSafe(Position2D Position1, Position2D Position2)
        {
            int i = 0;
            //loop through all the possible windows
            for (i = 0; i <= mXYSafetyWindow.Length - 1; i++)
            {
                if (!mXYSafetyWindow[i].Valid)
                    continue;
                if (mXYSafetyWindow[i].IsBothInsideWindow(Position1, Position2))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}

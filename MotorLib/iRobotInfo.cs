using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Delta
{
    public class iRobot
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
                return string.Format(s, X, Y, Z, Rx, Ry, Rz);
            }

            public static Position6D operator +(Position6D A, Position6D B)
            {
                return new Position6D(A.X + B.X, A.Y + B.Y, A.Z + B.Z, A.Rx + B.Rx, A.Ry + B.Ry, A.Rz + B.Rz);
            }

        }

        public enum AxisNameEnum
        {
            X,
            Y,
            Z,
            Rx,
            Ry,
            Rz
        }

        public static int AxisCount = Enum.GetNames(typeof(AxisNameEnum)).Length;

        public enum RobotPositionEnum
        {
            LoadUnload,
        }

        #endregion

        #region "Utility"
        public class ConfiguredRobotPosition
        {
            private string mLabel;

            private double[] mPositions;
            public ConfiguredRobotPosition(string Label, double[] Positions)
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

            public static ConfiguredRobotPosition Parse(string s)
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

                return new ConfiguredRobotPosition(sData[0].Trim(), Positions);
            }

            #endregion

        }
        #endregion

        private Dictionary<string, ConfiguredRobotPosition> mConfiguredPositions;

        private Instrument.iRobotController mRobot;

        private w2.w2IniFileXML mPara;

        #region "public access"
        public Instrument.iRobotController Robot
        {
            get { return mRobot; }
        }

        public bool Initialize(ref w2.w2IniFileXML hConfig, ref Instrument.iRobotController hRobot)
        {
            mRobot = hRobot;
            mPara = hConfig;

            if (!this.ParseConfiguredPositions())
                return false;

            return true;
        }
        #endregion

        public string GetStagePositionLabel(RobotPositionEnum Position)
        {
            string label = null;

            label = Enum.GetName(typeof(RobotPositionEnum), Position);
            label = w2String.AddSpaceBetweenWords(label);

            return label;
        }

        #region "configured positions"
        private bool ParseConfiguredPositions()
        {
            string s = null;
            string[] data = null;
            ConfiguredRobotPosition x = default(ConfiguredRobotPosition);

            //New
            mConfiguredPositions = new Dictionary<string, ConfiguredRobotPosition>();

            //get table
            s = mPara.ReadParameter("RobotTable", "ConfiguredPositions", "");
            s = s.Trim();
            data = s.Split(ControlChars.Cr);

            //parse table
            foreach (string s_loopVariable in data)
            {
                s = s_loopVariable;
                s = s.Trim();
                //first line is table header
                if (s == ConfiguredRobotPosition.TableHeader)
                    continue;
                //parse info
                x = ConfiguredRobotPosition.Parse(s);
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

            table = ControlChars.CrLf + ControlChars.Tab + ConfiguredRobotPosition.TableHeader;

            foreach (ConfiguredRobotPosition x in mConfiguredPositions.Values)
            {
                table += ControlChars.CrLf + ControlChars.Tab + x.TableString;
            }
            table += ControlChars.CrLf + "    ";

            mPara.WriteParameter("RobotTable", "ConfiguredPositions", table);
        }

        public Dictionary<string, ConfiguredRobotPosition> ConfiguredPositions
        {
            get { return mConfiguredPositions; }
        }

        public bool HaveConfiguredPosition(string Label)
        {
            return mConfiguredPositions.ContainsKey(Label);
        }

        public ConfiguredRobotPosition ConfiguredPosition(RobotPositionEnum Position)
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

        public ConfiguredRobotPosition ConfiguredPosition(string Label)
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

        public void AddConfiguredPosition(ConfiguredRobotPosition Position)
        {
            mConfiguredPositions.Add(Position.Label, Position);
            this.SaveConfiguredPositions();
        }

        public void RemoveConfiguredPosition(ConfiguredRobotPosition Position)
        {
            mConfiguredPositions.Remove(Position.Label);
            this.SaveConfiguredPositions();
        }

        public void UpdateConfiguredPosition(ConfiguredRobotPosition Position)
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

        public Position3D GetConfiguredRobot3DPosition(RobotPositionEnum Position)
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

            x = mConfiguredPositions[label].Positions[(int)AxisNameEnum.X];
            y = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Y];
            z = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Z];


            return new Position3D(x, y, z);

        }

        public Position6D GetConfiguredRobot6DPosition(RobotPositionEnum Position)
        {
            string label = null;
            double x = 0;
            double y = 0;
            double z = 0;
            double rx = 0;
            double ry = 0;
            double rz = 0;

            //get data entry label
            label = this.GetStagePositionLabel(Position);

            //reject if label is not valid
            if (!this.HaveConfiguredPosition(label))
            {
                return new Position6D(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            }

            x = mConfiguredPositions[label].Positions[(int)AxisNameEnum.X];
            y = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Y];
            z = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Z];
            rx = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Rx];
            ry = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Rx];
            rz = mConfiguredPositions[label].Positions[(int)AxisNameEnum.Rx];

            return new Position6D(x, y, z,rx,ry,rz);

        }
        #endregion
    }
}
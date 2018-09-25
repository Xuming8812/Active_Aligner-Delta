using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Instrument
{
    public class iKeithley2200 : iPowerSupply
    {

        public override double Current
        {
            get { return this.QueryValue("MEAS:CURR?"); }
            set
            {
                string s = null;
                s = "SOUR:CURR " + value + "A";
                this.SendCmd(s);
            }
        }

        public override double CurrentCompliance
        {
            get { return this.QueryValue("SOUR:CURR?"); }
            set { this.Current = value; }
        }

        public override double CurrentRange
        {
            get { return this.CurrentCompliance; }
            set { this.Current = value; }
        }

        public override bool EnabledOutput
        {
            get
            {
                string s = null;
                s = this.QueryString("SOUR:OUTP?");
                return s == "1";
            }
            set
            {
                string s = null;
                s = (value ? "1" : "0").ToString();
                s = "SOUR:OUTP " + s;
                this.SendCmd(s);
            }
        }

        public override iPowerSupply.OperationMode OutputMode
        {
            get { return iPowerSupply.OperationMode.ConstantVoltage; }
            //do nothing
            set { }
        }

        public override iPowerSupply.SenseModeEnum SenseMode
        {
            get { return iPowerSupply.SenseModeEnum.Local; }
            //do nothing
            set { }
        }

        public override double Voltage
        {
            get { return this.QueryValue("MEAS:VOLT?"); }
            set
            {
                string s = null;
                s = "SOUR:VOLT " + value + "V";
                this.SendCmd(s);
            }
        }

        public override double VoltageSetpoint
        {
            get { return this.QueryValue("SOUR:VOLT?"); }
        }

        public override double VoltageCompliance
        {
            get { return this.QueryValue("SOUR:VOLT:PROT?"); }
            set
            {
                string s = null;
                s = "SOUR:VOLT:PROT " + value + "V";
                this.SendCmd(s);
                this.SendCmd("SOUR:VOLT:PROT:STAT 1");
            }
        }

        public override double VoltageRange
        {
            get { return this.QueryValue("SOUR:VOLT:RANG?"); }
            set
            {
                string s = null;
                s = "SOUR:VOLT:RANG " + value + "V";
                this.SendCmd(s);
            }
        }

        public override double MaxCurrent
        {
            get
            {
                string[] sData = null;
                sData = mModel.Split('-');
                return double.Parse(sData[2]);
            }
        }

        public override double MaxVoltage
        {
            get
            {
                string[] sData = null;
                sData = mModel.Split('-');
                return double.Parse(sData[1]);
            }
        }


    }

    public class iKeithley2400 : iPowerSupply
    {

        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            if (base.Initialize(AdrsBoard, AdrsInstrument, RaiseError))
            {
                base.GPIBDevice.IOTimeout = NationalInstruments.NI4882.TimeoutValue.T10s;
                this.ConfigVoltageCurrentMeasurement();
                return true;
            }

            return false;
        }

        #region "standard interface"
        public void ConfigVoltageCurrentMeasurement()
        {
            base.SendCmd(":SENS:FUNC:ON \"VOLT\", \"CURR\"");
            base.SendCmd(":FORM:ELEM VOLT,CURR");
        }

        public void Measure(ref double Voltage, ref double Current)
        {
            string s = null;
            string[] sData = null;

            s = base.QueryString("MEAS?");
            sData = s.Split(',');
            Voltage = Conversion.Val(sData[0]);
            Current = Conversion.Val(sData[1]);
        }

        public double MeasurementNPLC
        {
            get { return base.QueryValue(":SENS:VOLT:NPLC?"); }
            set { base.SendCmd(":SENS:VOLT:NPLC " + value); }
        }

        public override double Current
        {
            get
            {
                double i = 0;
                double v = 0;
                this.Measure(ref v, ref i);
                return i;
            }
            set { base.SendCmd("SOUR:CURR " + value); }
        }

        public override double CurrentCompliance
        {
            get { return base.QueryValue("SENS:CURR:DC:PROT:LEV?"); }
            set
            {
                double Max = 0;
                Max = this.CurrentComplianceMax;
                if (value > Max)
                    value = Max;
                base.SendCmd("SENS:CURR:DC:PROT:LEV " + value);
            }
        }

        public double CurrentComplianceMax
        {
            get { return base.QueryValue("SENS:CURR:DC:PROT:LEV? MAX"); }
        }

        public double CurrentComplianceMin
        {
            get { return base.QueryValue("SENS:CURR:DC:PROT:LEV? MIN"); }
        }

        public override double CurrentRange
        {
            get { return base.QueryValue("SOUR:CURR:RANG?"); }
            set
            {
                //check Max. Min does not need to be checked. The software will automatically find the higher range to accomodate that selection
                double Max = 0;
                Max = this.CurrentRangeMax;
                if (value > Max)
                    value = Max;
                base.SendCmd("SOUR:CURR:RANG " + value);
            }
        }

        public double CurrentRangeMax
        {
            get { return base.QueryValue("SENS:CURR:RANG? MAX"); }
        }

        public double CurrentRangeMin
        {
            get { return base.QueryValue("SENS:CURR:RANG? MIN"); }
        }

        public override bool EnabledOutput
        {
            get
            {
                string s = null;
                s = base.QueryString("OUTP?");
                return s == "1";
            }
            set
            {
                string s = null;
                s = (value ? "1" : "0").ToString();
                s = "OUTP " + s;
                base.SendCmd(s);
            }
        }

        public override iPowerSupply.OperationMode OutputMode
        {
            get
            {
                string s = null;
                s = base.QueryString("SOUR:FUNC:MODE?");
                switch (s)
                {
                    case "VOLT":
                        return OperationMode.ConstantVoltage;
                    case "CURR":
                        return OperationMode.ConstantCurrent;
                    case "EME":
                        return OperationMode.KeithleyMemorySweep;
                    default:
                        return OperationMode.Unknown;
                }
            }
            set
            {
                bool enabled = false;
                string s = null;

                switch (value)
                {
                    case OperationMode.ConstantVoltage:
                        s = "VOLT";
                        break;
                    case OperationMode.ConstantCurrent:
                        s = "CURR";
                        break;
                    case OperationMode.KeithleyMemorySweep:
                        s = "MEM";
                        break;
                    default:
                        return;
                }

                //disable output first before switch mode
                this.Voltage = 0.0;
                this.Current = 0.0;
                enabled = this.EnabledOutput;
                this.EnabledOutput = false;

                //change mode now
                base.SendCmd("SOUR:FUNC:MODE " + s);

                this.EnabledOutput = enabled;

                this.ConfigVoltageCurrentMeasurement();
            }
        }

        public override double Voltage
        {
            get
            {
                double i = 0;
                double v = 0;
                this.Measure(ref v, ref i);
                return v;
            }
            set { base.SendCmd("SOUR:VOLT " + value); }
        }

        public override double VoltageCompliance
        {
            get { return base.QueryValue("SENS:VOLT:DC:PROT:LEV?"); }
            set { base.SendCmd("SENS:VOLT:DC:PROT:LEV " + value); }
        }

        public override double VoltageRange
        {
            get { return base.QueryValue("SOUR:VOLT:RANG?"); }
            set { base.SendCmd("SOUR:VOLT:RANG " + value); }
        }

        public override SenseModeEnum SenseMode
        {
            get
            {
                string s = null;
                s = QueryString("SYST:RSEN?");
                if (s == "0" | s == "OFF")
                {
                    return SenseModeEnum.Local;
                }
                else
                {
                    return SenseModeEnum.Remote;
                }
            }
            set
            {
                string s = null;
                switch (value)
                {
                    case SenseModeEnum.Local:
                        s = "OFF";
                        break;
                    case SenseModeEnum.Remote:
                        s = "ON";
                        break;
                    default:
                        return;
                }
                s = "SYST:RSEN " + s;
                SendCmd(s);
            }
        }

        public override double MaxCurrent
        {
            get { return this.CurrentComplianceMax; }
        }

        public override double MaxVoltage
        {
            get { return base.QueryValue("SENS:VOLT:DC:PROT:LEV? MAX"); }
        }
        #endregion

        public enum Terminal
        {
            Front,
            Rear
        }

        public Terminal RoutingTerminal
        {
            get
            {
                string s = null;
                s = this.QueryString("ROUT:TERM?");
                switch (s)
                {
                    case "FRON":
                        return Terminal.Front;
                    default:
                        return Terminal.Rear;
                }
            }
            set
            {
                string s = null;

                switch (value)
                {
                    case Terminal.Front:
                        s = "FRON";
                        break;
                    case Terminal.Rear:
                        s = "REAR";
                        break;
                    default:
                        return;
                }

                s = "ROUT:TERM " + s;
                this.SendCmd(s);

            }
        }


    }

    public class iKeithley2600 : iPowerSupply
    {


        private string mChannel = "smua";
        private double mVMax;

        private double mIMax;

        public override bool Initialize(int AdrsBoard, int AdrsInstrument, bool RaiseError)
        {
            if (base.Initialize(AdrsBoard, AdrsInstrument, RaiseError))
            {
                mGPIB.IOTimeout = NationalInstruments.NI4882.TimeoutValue.T10s;

                if (mModel.Contains("2601") || mModel.Contains("2602"))
                {
                    mVMax = 40.0;
                    mIMax = 3.0;
                }
                else if (mModel.Contains("2611") || mModel.Contains("2612"))
                {
                    mVMax = 200.0;
                    mIMax = 3.0;
                }
                else if (mModel.Contains("2635") || mModel.Contains("2636"))
                {
                    mVMax = 200.0;
                    mIMax = 1.5;
                }
                else if (mModel.Contains("2651"))
                {
                    mVMax = 40.0;
                    mIMax = 20.0;
                }
                else
                {
                    mVMax = 100.0;
                    mIMax = 100.0;
                }
               
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            string s = null;
            s = mChannel + ".reset()";
            SendCmd(s);
        }

        public int OutputChannel
        {
            get
            {
                switch (mChannel)
                {
                    case "smua":
                        return 1;
                    case "smub":
                        return 2;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 1:
                        mChannel = "smua";
                        break;
                    case 2:
                        switch (this.ModelName)
                        {
                            case "Model 2602":
                            case "Model 2612":
                            case "Model 2636":
                            case "Model 2602A":
                            case "Model 2612A":
                                mChannel = "smub";
                                break;
                            default:
                                break;
                                //do nothing
                        }
                        break;
                    default:
                        break;
                        //do nothing
                }
            }
        }

        public override bool EnabledOutput
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.output)";
                return (base.QueryValue(s) == 1.0);
            }
            set
            {
                string s = null;
                s = mChannel + ".source.output = ";
                s += (value ? "1" : "0").ToString();
                SendCmd(s);
            }
        }


        #region "Current/voltage"
        public double MeasurementNPLC
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.nplc)";
                return base.QueryValue(s);
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.nplc = " + value;
                base.SendCmd(s);
            }
        }

        public bool AutoRangeCurrentMeasurement
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.autorangei)";
                return (QueryValue(s) == 1.0);
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.autorangei = ";
                s += (value ? "1" : "0").ToString();
                SendCmd(s);
            }
        }

        public bool AutoRangeVoltageMeasurement
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.autorangev)";
                return (this.QueryValue(s) == 1.0);
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.autorangev = ";
                s += (value ? "1" : "0").ToString();
                SendCmd(s);
            }
        }

        public override double Current
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.i())";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                s = mChannel + ".source.leveli = ";
                s += value;
                SendCmd(s);
            }
        }

        public double CurrentMeasurementRange
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.rangei)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.rangei = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double CurrentSetpoint
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.leveli)";
                return QueryValue(s);
            }
        }

        public override double Voltage
        {
            get
            {
                string s = null;
                double v = 0;
                s = "print(" + mChannel + ".measure.v())";
                v = QueryValue(s);
                if (v > 1E+37)
                {
                    v = VoltageSetpoint;
                }
                return v;
            }
            set
            {
                string s = null;
                s = mChannel + ".source.levelv = ";
                s += value;
                SendCmd(s);
            }
        }

        public double VoltageMeasurementRange
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".measure.rangev)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.rangev = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double VoltageSetpoint
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.levelv)";
                return QueryValue(s);
            }
        }

        #endregion

        #region "Range / compliance"
        public enum AutoZeroMode
        {
            Off,
            Once,
            Auto
        }

        public AutoZeroMode AutoZero
        {
            get
            {
                string s = null;
                double v = 0;
                s = "print(" + mChannel + ".measure.autozero)";
                v = base.QueryValue(s);
                return (AutoZeroMode)v;
            }
            set
            {
                string s = null;
                s = mChannel + ".measure.autozero = " + value;
                base.SendCmd(s);
            }
        }


        public bool AutoRangeCurrentOutput
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.autorangei)";
                return (QueryValue(s) == 1.0);
            }
            set
            {
                string s = null;
                s = mChannel + ".source.autorangei = ";
                s += (value ? "1" : "0").ToString();
                SendCmd(s);
            }
        }

        public bool AutoRangeVoltageOutput
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.autorangev)";
                return (this.QueryValue(s) == 1.0);
            }
            set
            {
                string s = null;
                s = mChannel + ".source.autorangev = ";
                s += (value ? "1" : "0").ToString();
                SendCmd(s);
            }
        }

        public override double CurrentRange
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.rangei)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                if (value <= 0)
                {
                    value = 0.1;
                }
                else if (value > mIMax)
                {
                    value = mIMax;
                }
                s = mChannel + ".source.rangei = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double CurrentCompliance
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.limiti)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                if (value <= 0)
                {
                    value = 0.1;
                }
                else if (value > mIMax)
                {
                    value = mIMax;
                }
                s = mChannel + ".source.limiti = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double VoltageRange
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.rangev)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                if (value <= 0)
                {
                    value = 0.1;
                }
                else if (value > mVMax)
                {
                    value = mVMax;
                }
                s = mChannel + ".source.rangev = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double VoltageCompliance
        {
            get
            {
                string s = null;
                s = "print(" + mChannel + ".source.limitv)";
                return QueryValue(s);
            }
            set
            {
                string s = null;
                if (value <= 0)
                {
                    value = 0.1;
                }
                else if (value > mVMax)
                {
                    value = mVMax;
                }
                s = mChannel + ".source.limitv = ";
                s += value;
                SendCmd(s);
            }
        }

        public override double MaxCurrent
        {
            get { return mIMax; }
        }

        public override double MaxVoltage
        {
            get { return mIMax; }
        }
        #endregion

        #region "mode"
        public override iPowerSupply.OperationMode OutputMode
        {
            get
            {
                string s = null;
                double v = 0;
                s = "print(" + mChannel + ".source.func)";
                v = QueryValue(s);
                switch (v)
                {
                    case 0.0:
                        return OperationMode.ConstantCurrent;
                    case 1.0:
                        return OperationMode.ConstantVoltage;
                    default:
                        return OperationMode.Unknown;
                }
            }
            set
            {
                string s = null;
                switch (value)
                {
                    case OperationMode.ConstantCurrent:
                        s = "0";
                        break;
                    case OperationMode.ConstantVoltage:
                        s = "1";
                        break;
                    default:
                        return;
                }
                s = mChannel + ".source.func = " + s;
                SendCmd(s);
                System.Threading.Thread.Sleep(100);
            }
        }

        public override SenseModeEnum SenseMode
        {
            get
            {
                string s = null;
                double v = 0;
                s = "print(" + mChannel + ".sense)";
                v = QueryValue(s);
                switch (v)
                {
                    case 0.0:
                        return SenseModeEnum.Local;
                    case 1.0:
                        return SenseModeEnum.Remote;
                    case 3.0:
                        return SenseModeEnum.Calibration;
                    default:
                        return SenseModeEnum.Unknown;
                }
            }
            set
            {
                string s = null;
                switch (value)
                {
                    case SenseModeEnum.Local:
                        s = "0";
                        break;
                    case SenseModeEnum.Remote:
                        s = "1";
                        break;
                    case SenseModeEnum.Calibration:
                        s = "3";
                        break;
                    default:
                        return;
                }
                s = mChannel + ".sense = " + s;
                SendCmd(s);
            }
        }
        #endregion

    }

    public abstract class iPowerSupply : iGPIB
    {

        public enum OperationMode
        {
            ConstantVoltage,
            ConstantCurrent,
            KeithleyMemorySweep,
            Unknown
        }

        public enum SenseModeEnum
        {
            Local = 0,
            Remote = 1,
            Calibration = 3,
            Unknown = 4
        }

        public abstract OperationMode OutputMode { get; set; }
        public abstract SenseModeEnum SenseMode { get; set; }

        public abstract bool EnabledOutput { get; set; }
        //Public MustOverride Property AutoRange() As Boolean

        public abstract double Current { get; set; }
        public abstract double CurrentCompliance { get; set; }
        public abstract double CurrentRange { get; set; }

        public abstract double Voltage { get; set; }
        public abstract double VoltageCompliance { get; set; }
        public abstract double VoltageRange { get; set; }

        public abstract double MaxVoltage { get; }
        public abstract double MaxCurrent { get; }

        //Public MustOverride Property EnabledOverVoltageProtection() As Boolean

        public virtual double CurrentSetpoint
        {
            get { return this.Current; }
        }

        public virtual double VoltageSetpoint
        {
            get { return this.Voltage; }
        }

        public double Current_mA
        {
            get { return 1000.0 * this.Current; }
            set { this.Current = 0.001 * value; }
        }
    }
}

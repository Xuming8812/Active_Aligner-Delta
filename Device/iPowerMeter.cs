using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace Instrument
{

    public class iThorlabsPM100 : iPowerMeter
    {


        private Thorlabs.PM100D.PM100D mPM100D;
        public override bool Initialize(string sResource, bool RaiseError)
        {
            string[] data = null;

            try
            {
                mPM100D = new Thorlabs.PM100D.PM100D(sResource, true, false);
            }
            catch (Exception ex)
            {
                return false;
            }

            if ((mPM100D.Handle != System.IntPtr.Zero))
            {
                base.mGPIB = null;
                base.mManufacturer = "Thorlabs";

                sResource = sResource.Replace("::", ":");
                data = sResource.Split(':');
                base.mSerialNumber = data[3];

                switch (data[2])
                {
                    case "0x8070":
                        base.mModel = "PD100D with DFU";
                        break;
                    case "0x8071":
                        base.mModel = "PD100A with DFU";
                        break;
                    case "0x8072":
                        base.mModel = "PD100USB with DFU";

                        break;
                    case "0x8078":
                        base.mModel = "PD100D without DFU";
                        break;
                    case "0x8079":
                        base.mModel = "PD100A without DFU";
                        break;
                    case "0x80B0":
                        base.mModel = "PM200";

                        break;
                    default:
                        base.mModel = data[2];
                        break;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        public override bool AutoRange
        {
            get
            {
                bool b = false;
                mPM100D.getPowerAutorange(out b);
                return b;
            }
            set { mPM100D.setPowerAutoRange(value); }
        }

        public override int AverageTime_ms
        {
            get { return 0; }
            //do nothing
            set { }
        }

        public override bool DetectorExist
        {
            get
            {
                 StringBuilder Name = default( StringBuilder);
                 StringBuilder SN = default( StringBuilder);
                 StringBuilder Msg = default( StringBuilder);
                short i = 0;

                Name = new  StringBuilder();
                SN = new  StringBuilder();
                Msg = new  StringBuilder();

                return i != 0;
            }
        }

        public override string DetectorSN
        {
            get
            {
                 StringBuilder Name = default( StringBuilder);
                 StringBuilder SN = default( StringBuilder);
                 StringBuilder Msg = default( StringBuilder);
                short i = 0;
                short j = 0;
                short k = 0;

                Name = new  StringBuilder();
                SN = new  StringBuilder();
                Msg = new  StringBuilder();

                mPM100D.getSensorInfo(Name, SN, Msg, out i, out j, out k);

                return SN.ToString();

            }
        }

        public override int DisplayedChannel
        {
            get { return 1; }
            //do nothing
            set { }
        }

        public override int MeterChannel
        {
            get { return 1; }
            //do nothing
            set { }
        }

        public override int MeterSlot
        {
            get { return 1; }
            //do nothing
            set { }
        }

        public override PowerUnitEnum PowerUnit
        {
            get
            {
                short i = 0;
                mPM100D.getPowerUnit(out i);
                switch (i)
                {
                    case Thorlabs.PM100D.PM100DConstants.PowerUnitDbm:
                        return PowerUnitEnum.dBm;
                    case Thorlabs.PM100D.PM100DConstants.PowerUnitWatt:
                        return PowerUnitEnum.mW;                                            
                }
                return PowerUnitEnum.dBm;
            }
            set
            {
                short i = 0;
                switch (value)
                {
                    case PowerUnitEnum.dBm:
                    case PowerUnitEnum.dB:
                        i = Thorlabs.PM100D.PM100DConstants.PowerUnitDbm;
                        break;
                    case PowerUnitEnum.mW:
                    case PowerUnitEnum.W:
                    case PowerUnitEnum.uW:
                        i = Thorlabs.PM100D.PM100DConstants.PowerUnitWatt;
                        break;
                    default:
                        return;
                }
                mPM100D.setPowerUnit(i);
            }
        }

        public override double ReadPower(int NumberOfSamples)
        {
            double v = 0;

            mPM100D.setAvgCnt(Convert.ToInt16(NumberOfSamples));
            mPM100D.measPower( out v);

            return v;
        }

        public override void StoreReference()
        {
            double v = 0;
            v = this.ReadPower(2);
            this.StoreReference(v);
        }

        public override void StoreReference(double ReferenceValue)
        {
            mPM100D.setPowerRef(ReferenceValue);
        }

        public override double Wavelength_nm
        {
            get
            {
                double v = 0;
                mPM100D.getWavelength(Thorlabs.PM100D.PM100DConstants.AttrSetVal, out v);
                return v;
            }
            set { mPM100D.setWavelength(value); }
        }
    }

    public class iThorlabsPM300 : iPowerMeter
    {


        private Thorlabs.PM300.PM300_Drv mPM300;
        public override bool Initialize(string sResource, bool RaiseError)
        {
            string[] data = null;

            try
            {
                mPM300 = new Thorlabs.PM300.PM300_Drv(sResource, 2000, false, true);
            }
            catch (Exception ex)
            {
                return false;
            }

            if ((mPM300 != null))
            {
                base.mGPIB = null;
                base.mManufacturer = "Thorlabs";

                sResource = sResource.Replace("::", ":");
                data = sResource.Split(':');
                base.mSerialNumber = data[3];

                base.mModel = data[2];

                return true;
            }
            else
            {
                return false;
            }

        }

        public override bool AutoRange
        {
            get
            {
                short range = 0;
                mPM300.getPowerRng(Channel, out range);
                return range == Thorlabs.PM300.PM300_DrvConstants.PrngAuto;
            }
            set
            {
                if (value)
                {
                    mPM300.setPowerRng(Channel, Thorlabs.PM300.PM300_DrvConstants.PrngAuto);
                }
                else
                {
                    mPM300.setPowerRng(Channel, Thorlabs.PM300.PM300_DrvConstants.Prng100mw);
                    // set Range to 100mW which is enough for laser chip
                }
            }
        }

        public override int AverageTime_ms
        {
            get { return 0; }
            set
            {
                throw new Exception("Funcion is not implemented!");
            }
        }

        public override bool DetectorExist
        {
            get
            {
                Int32 sensorID = default(Int32);
                Int32 sensorClass = default(Int32);
                 StringBuilder sensorName = default( StringBuilder);
                 StringBuilder sensorSN = default( StringBuilder);

                sensorID = (int)Constants.vbNull;
                sensorName = new  StringBuilder();
                sensorSN = new  StringBuilder();

                mPM300.sensorInfoQuery(Channel, out sensorID, out sensorClass, sensorName, sensorSN);
                return sensorID != (int)Constants.vbNull;
            }
        }

        public override string DetectorSN
        {
            get
            {
                Int32 sensorID = default(Int32);
                Int32 sensorClass = default(Int32);
                 StringBuilder sensorName = default( StringBuilder);
                 StringBuilder sensorSN = default( StringBuilder);

                sensorID = (int)Constants.vbNull;
                sensorName = new  StringBuilder();
                sensorSN = new  StringBuilder();

                mPM300.sensorInfoQuery(Channel, out sensorID, out sensorClass, sensorName, sensorSN);
                return sensorSN.ToString();

            }
        }

        public override int DisplayedChannel
        {
            get { return 1; }
            set
            {
                throw new Exception("Funcion is not implemented!");
            }
        }

        public override int MeterChannel
        {
            get { return Channel; }
            set { Channel = Convert.ToInt16(value); }
        }

        public override int MeterSlot
        {
            get { return 1; }
            set
            {
                throw new Exception("Funcion is not implemented!");
            }
        }

        public override PowerUnitEnum PowerUnit
        {
            get { return _powerUnit; }
            set { _powerUnit = value; }
        }

        public override double ReadPower(int NumberOfSamples)
        {
            double v = 0;

            mPM300.setAverage(Channel, Convert.ToInt16(NumberOfSamples));
            mPM300.getPower(Channel, out v);

            switch (_powerUnit)
            {
                case PowerUnitEnum.dBm:
                    v = Physics.mWTodBm(v * 1000);
                    if (v < -70)
                        v = -70;
                    return v;
                case PowerUnitEnum.mW:
                    return v * 1000;
                case PowerUnitEnum.W:
                    return v;
                default:
                    v = Physics.mWTodBm(v * 1000);
                    if (v < -70)
                        v = -70;
                    return v;
            }
        }

        public override void StoreReference()
        {
            double v = 0;
            v = this.ReadPower(2);
            this.StoreReference(v);
        }

        public override void StoreReference(double ReferenceValue)
        {
            throw new Exception("Funcion is not implemented!");
        }

        public override double Wavelength_nm
        {
            get
            {
                double v = 0;
                mPM300.getWavel(Channel, out v);
                return v * 1000000000.0;
            }
            set { mPM300.setWavel(Channel, value / 1000000000.0); }
        }

        public short Channel
        {
            get
            {
                if (_channel != 1 & _channel != 2)
                {
                    throw new Exception("Channel is not specified!");
                }
                else
                {
                    return _channel;
                }
            }
            set
            {
                if (value != 1 & value != 2)
                {
                    throw new Exception("Channel must be either 1 or 2! Please check the ini File!");
                }
                else
                {
                    _channel = value;
                }

            }
        }


        private short _channel;
        private PowerUnitEnum _powerUnit;
    }

    /// <summary>
    /// Abstract class for power meter
    /// </summary>
    /// <remarks></remarks>
    public abstract class iPowerMeter : iGPIB
    {

        public abstract bool Initialize(string sResource, bool RaiseError);

        public abstract double ReadPower(int NumberOfSamples);
        public virtual double Power
        {
            get { return ReadPower(1); }
        }

        public abstract double Wavelength_nm { get; set; }
        public virtual double Frequency_GHz
        {
            get { return Physics.nmToGHz(this.Wavelength_nm); }
            set { this.Wavelength_nm = Physics.GHzTonm(value); }
        }

        public abstract string DetectorSN { get; }
        public abstract bool DetectorExist { get; }
        public abstract int MeterSlot { get; set; }
        public abstract int MeterChannel { get; set; }
        public abstract int DisplayedChannel { get; set; }

        public abstract bool AutoRange { get; set; }
        public abstract PowerUnitEnum PowerUnit { get; set; }
        public abstract int AverageTime_ms { get; set; }

        public abstract void StoreReference();
        public abstract void StoreReference(double ReferenceValue);

    }
}

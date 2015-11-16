using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    class picWord : IComparable<picWord>
    {
        public enum DataTypes { Program, EOF, ExtendedAddress = 4 };
        public const int BYTEBLOCK = 2;
        protected RegisterFile rf;
        protected int binary;
        protected int BaseAddress;
        public asmLabel Label { get; set; }

        public virtual Boolean isInstruction() { return false; }

        public int getAddress()        {    return BaseAddress;        }
        public void setLabel(ref asmLabel label)
        {
            Label = label;
            Label.placed = true;
        }

        public picWord(int bin, int addr)
        {
            binary = bin;
            BaseAddress = addr;
        }

        public picWord(int bin, int addr, ref RegisterFile r)
        {
            binary = bin;
            BaseAddress = addr;
            rf = r;
        }

        public picWord()
        {
            binary = 0;
            BaseAddress = 0;
        }

        public int CompareTo(picWord obj)
        {
            // A null value means that this object is greater.
            if (obj == null)
                return 1;

            else
                return this.BaseAddress.CompareTo(obj.BaseAddress);
        }

        public override string ToString()
        {
            if(BaseAddress != 0x2007)
                return String.Format("{0,5}\t{3,6}\t{1,15}\t{2,-32}", BaseAddress.ToString("X4"), Label, "", binary.ToString("X4"));
            else
                return String.Format("{0,5}\t{3,6}\t{1,15}\t{2,-32}", BaseAddress.ToString("X4"), Label, decodeConfigWord(), binary.ToString("X4"));
        }

        private String decodeConfigWord()
        {
            String Osc, WatchDogTimer, PowerUpTimer, BrownoutReset, LowVoltageSupply, EEPROMProtection, FlashProgramWriteEnable, Debug, FlashCodeProtection;
            switch(binary&0x0003)
            {
                case 0:
                    // LP Oscillator
                    Osc = "Low Power Crystal Oscillator Installed.";
                    break;
                case 1:
                    // XT Oscillator
                    Osc = "Crystal/Resonator Oscillator Installed.";
                    break;
                case 2:
                    // HS Oscillator
                    Osc = "High-Speed Crystal/Resonator Oscillator Installed.";
                    break;
                case 3:
                    // RC Oscillator
                    Osc = "Resistor/Capacitor Oscillator Installed.";
                    break;
                default:
                    Osc = "";
                    break;
            }
            if ((binary & 0x0010) == 4)         WatchDogTimer = "Watchdog Timer Enabled.";
            else                                WatchDogTimer = "Watchdog Timer Disabled.";
            if ((binary & 0x0020) == 8)         PowerUpTimer = "Power-up Timer Enabled.";
            else                                PowerUpTimer = "Power-up Timer Disabled.";
            // Brown-out reset is a condition bit that tells the microcontroller if it should reset when the Voltage source dips bellow its operating value.
            if ((binary & 0x0040)>>6 == 1)      BrownoutReset = "Brown-out Reset Enabled.";
            else                                BrownoutReset = "Brown-out Reset Disabled.";
            if ((binary & 0x0080) >> 7 == 1)    LowVoltageSupply = "RB3 set for programming function: Low-voltage ICSP programming Enabled.";
            else                                LowVoltageSupply= "RB3 is set as digital I/O: MCLR' must be used for programming.";
            if ((binary & 0x0100) >> 8 == 1)    EEPROMProtection = "Data EEPROM code protection Enabled.";
            else                                EEPROMProtection = "Data EEPROM code protection Disabled.";
            
            switch((binary & 0x0300)>>9)
            {
                case 0:
                    FlashProgramWriteEnable = "0x0000 - 0x0FFF write protected;0x1000 - 0x1FFF may be written by ECON control.";
                    break;
                case 1:
                    FlashProgramWriteEnable = "0x0000 - 0x07FF write protected;0x0800 - 0x1FFF may be written by ECON control.";
                    break;
                case 2:
                    FlashProgramWriteEnable = "0x0000 - 0x00FF write protected;0x0100 - 0x1FFF may be written by ECON control.";
                    break;
                case 3:
                    FlashProgramWriteEnable = "Write protection Off; All program memory may be written by ECON control.";
                    break;
                default:
                    FlashProgramWriteEnable = "";
                    break;
            }

            if ((binary & 0x0800) >> 11 == 1)   Debug = "RB6, RB7 are digital I/O; In-Circuit Debbuger Disabled.";
            else                                Debug = "RB6, RB7 are for debugging; In-Circuit Debbuger Enabled.";

            if ((binary & 0x2000) >> 12 == 1)   FlashCodeProtection = "Code Protection is off.";
            else                                FlashCodeProtection = "All program memory code-protected.";

            return "\n\n"+Osc + "\n" + WatchDogTimer + "\n" + PowerUpTimer + "\n" + BrownoutReset + "\n" + LowVoltageSupply + "\n" + EEPROMProtection + "\n" + FlashProgramWriteEnable + "\n" + Debug + "\n" + FlashCodeProtection;
        }
    }
}

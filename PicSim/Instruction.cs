using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    class Instruction
    {
        public enum DataTypes { Program, EOF, ExtendedAddress = 4 };
        public const int BYTEBLOCK = 2;

        private int binary {get; set;}
        private int BaseAddress { get; set; }
        private String ASM { get; set; }
        public int RegisterPage { set; get; }

        public Instruction()
        {
            binary = 0;
            BaseAddress = 0;
            ASM = "NOP";
        }

        public Instruction(int Bin)
        {
            binary = Bin;
            BaseAddress = 0;
            ASM = asmLookUp(Bin);
        }

        private string asmLookUp(int Bin)
        {
            string ASM = "";
            if (Bin >> 10 == 0)         // Typical case for Byte Oriented File Register Instructions
            {
                if ((Bin & 0x0FFF) >> 8 == 0)   // Case 1 of 2 possible instructions for this nibble
                    if ((Bin & 0x009F) == 0)
                        ASM = "NOP";
                    else
                    {
                        int f = Bin & 0x007F;
                        ASM = "MOVWF " + decodeRegisterFile(f);
                    }
                else if ((Bin & 0x0FFF) >> 8 == 1) // Case 2 of 2 possible instructions for this nibble
                {
                    if ((Bin & 0x0080) == 0)
                        ASM = "CLRW";
                    else
                    {
                        int f = Bin & 0x007F;
                        ASM = "CLRF " + decodeRegisterFile(f);
                    } 
                }
                else                               // The rest of the Byte Oriented File Register Instructions
                {
                    switch(Bin & 0x0F00)
                    {
                        case 2:

                            break;
                        default:
                            break;
                    }
                }
            }
            else if (Bin >> 10 == 1)    // Typical case for Bit Oriented File Register Instructions
            {
                ASM = "";
            }
            else                        // Case for Literal and Control Operations
            {
                ASM = "";
            }
            return ASM;
        }

        private string decodeRegisterFile(int f)
        {
            RegisterFile RF = new RegisterFile();
            String reg = "";
            reg =  RF.RegFileNames[RegisterPage].ElementAt(f);
            if (!reg.Equals("") && !reg.Equals("N/I") && !reg.Equals("Reserved"))
                return reg;
            else throw new Exception("Illegal Register.");
        }

        /*
         public static List<String> decompile(List<String> hex)
        {
            int Bytes, BaseAddress, CheckSum, i;
            DataTypes DataType;
            List<int> DataBytes = new List<int>();
            List<String> sourceISR = new List<string>();
            foreach(String line in hex)
            {
                Bytes = Convert.ToInt32(line.Substring(1, BYTEBLOCK), 16);
                BaseAddress = Convert.ToInt32(line.Substring(BYTEBLOCK+1, 2 * BYTEBLOCK), 16) >> 1;
                DataType = (DataTypes)Convert.ToInt32(line.Substring(3 * BYTEBLOCK+1, BYTEBLOCK), 16);
                for (i = 0; i < Bytes*BYTEBLOCK; i+=2*BYTEBLOCK)
                    DataBytes.Add(Convert.ToInt32(  line.Substring(i + 5 * BYTEBLOCK + 1, BYTEBLOCK) + 
                                                    line.Substring(i + 4 * BYTEBLOCK + 1, BYTEBLOCK), 16));
                    // Due to the little endian design of the instruction format, bytes need to be reverse in order to be usable.
                CheckSum = Convert.ToInt32(line.Substring(line.Length - BYTEBLOCK, BYTEBLOCK), 16);
            }
            return sourceISR;
        }
         */

    }
}

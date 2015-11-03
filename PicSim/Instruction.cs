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
            int f, b, k;
            if (Bin >> 10 == 0)         // Typical case for Byte Oriented File Register Instructions
            {
                if ((Bin & 0x0FFF) >> 8 == 0)       // 6 instructions start with 00 0000 xxxx xxxx
                {
                    if ((Bin & 0x009F) == 0)        // Case NOP: 00 0000 0xx0 0000
                        ASM = "NOP";
                    else if ((Bin & 0x0080) == 1)   // Case MOVWF: 00 0000 1fff ffff
                    {
                        f = Bin & 0x007F;
                        ASM = "MOVWF " + decodeRegisterFile(f);
                    }
                    else  // Literal and Control Operations that start with 00 0000
                    {
                        if ((Bin) == 8)            // Case RETURN: 00 0000 0000 1000
                            ASM = "RETURN";
                        else if ((Bin) == 9)       // Case RETFIE: 00 0000 0000 1001
                            ASM = "RETFIE";
                        else if ((Bin) == 0x0063)  // Case SLEEP:  00 0000 0110 0011
                            ASM = "SLEEP";
                        else if ((Bin) == 0x0064)   // Case CLRWDT: 00 0000 0110 0100
                            ASM = "CLRWDT";
                    }
                }
                else if ((Bin & 0x0FFF) >> 8 == 1) // Instructions that start with 00 0001 xxxx xxxx
                {
                    if ((Bin & 0x0080) == 0)        // Case CLRW:   00 0001 0xxx xxxx
                        ASM = "CLRW";
                    else                            // Case CLRF:   00 0001 1fff ffff
                    {
                        f = Bin & 0x007F;
                        ASM = "CLRF " + decodeRegisterFile(f);
                    }
                }
                else                               // The rest of the Byte Oriented File Register Instructions
                {
                    f = Bin & 0x007F;
                    switch ((Bin & 0x0F00)>>8)
                    {
                        case 2:                     // Case SUBWF:  00 0010 dfff ffff
                            
                            ASM = "SUBWF ";
                            break;
                        case 3:
                            ASM = "DECF ";          // Case DECF:   00 0011 dfff ffff
                            break;
                        case 4:                     // Case IORWF:  00 0100 dfff ffff
                            ASM = "IORWF ";
                            break;
                        case 5:
                            ASM = "ANDWF ";         // Case ANDWF:  00 0101 dfff ffff
                            break;
                        case 6:                     // Case XORWF:  00 0110 dfff ffff
                            ASM = "XORWF ";
                            break;
                        case 7:                     // Case ADDWF: 00 0111 dfff ffff
                            ASM = "ADDWF ";
                            break;
                        case 8:
                            ASM = "MOVF ";          // Case MOVF:   00 1000 dfff ffff
                            break;
                        case 9:                     // Case COMF:   00 1001 dfff ffff
                            ASM = "COMF ";
                            break;
                        case 10:                    // Case INCF:   00 1010 dfff ffff
                            ASM = "INCF ";
                            break;
                        case 11:                    // Case DECFSZ: 00 1011 dfff ffff
                            ASM = "DECFSZ ";
                            break;
                        case 12:                    // Case RRF:    00 1100 dfff ffff
                            ASM = "RRF ";
                            break;
                        case 13:                    // Case RLF:    00 1101 dfff ffff
                            ASM = "RLF ";
                            break;
                        case 14:                    // Case SWAPF:  00 1110 dfff ffff
                            ASM = "SWAPF ";
                            break;
                        case 15:                    // Case INCFSZ: 00 1111 dfff ffff
                            ASM = "INCFSZ ";
                            break;
                        default:
                            throw new Exception("Unknown OpCode.");
                            break;
                    }
                    if ((Bin & 0x0080) >> 7 == 1)
                        ASM += decodeRegisterFile(f) + ", f";
                    else
                        ASM += decodeRegisterFile(f) + ", w";
                }
            }
            else if (Bin >> 10 == 1)    // Typical case for Bit Oriented File Register Instructions
            {
                f = Bin & 0x007F;
                b = (Bin & 0x0)>>0;
                switch ((Bin & 0x0C00)>>10)
                {
                    case 0:                         // Case BCF:    00 00bb bfff ffff
                        ASM = "BCF ";
                        break;
                    case 1:                         // Case BSF:    00 01bb bfff ffff
                        ASM = "BSF ";
                        break;
                    case 2:                         // Case BTFSC:  00 10bb bfff ffff
                        ASM = "BTFSC ";
                        break;
                    case 3:                         // Case BTFSS:  00 11bb bfff ffff
                        ASM = "BTFSS ";
                        break;
                    default:
                        throw new Exception("Unknown OpCode.");
                        break;
                }
                ASM += decodeRegisterFile(f) + ", " + b.ToString();
            }
            else                        // Case for Typical Literal and Control Operations
            {
                if((Bin & 0x3000)>>12 == 2)
                {
                    k = Bin & 0x07FF;
                    if((Bin & 0x0800)>>11 == 1)     // Case GOTO:   10 1kkk kkkk kkkk
                    {
                        ASM = "GOTO L"+k.ToString();
                    }
                    else                            // Case CALL:   10 0kkk kkkk kkkk
                    {
                        ASM = "CALL S"+k.ToString();
                    }
                }
                else
                {
                    if ((Bin & 0x0C00) >> 10 == 0)  // Case MOVLW:  11 00xx kkkk kkkk
                    {
                        ASM = "MOVLW ";
                    }
                    else if ((Bin & 0x0C00) >> 10 == 1) // Case RETLW:  11 01xx kkkk kkkk
                    {
                        ASM = "RETLW ";
                    }
                    else if ((Bin & 0x0F00) >> 8 == 8) // Case IORLW:  11 1000 kkkk kkkk
                    {
                        ASM = "IORLW ";
                    }
                    else if ((Bin & 0x0F00) >> 8 == 9) // Case ANDLW:   11 1001 kkkk kkkk
                    {
                        ASM = "ANDLW ";
                    }
                    else if ((Bin & 0x0F00) >> 8 == 10) // Case XORLW:  11 1010 kkkk kkkk
                    {
                        ASM = "XORLW ";
                    }
                    else if ((Bin & 0x0D00) >> 9 == 6) // Case SUBLW:   11 110x kkkk kkkk
                    {
                        ASM = "SUBLW ";
                    }
                    else if ((Bin & 0x0F00) >> 8 == 8) // Case ADDLW:   11 111x kkkk kkkk
                    {
                        ASM = "ADDLW ";
                    }
                    else
                        throw new Exception("Unknown OpCode.");
                    k = Bin & 0x00FF;
                    ASM += k.ToString();
                }
                
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
            else throw new Exception("Unknown Register.");
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

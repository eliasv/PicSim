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
        private RegisterFile rf;
        private int binary;
        private int BaseAddress;
        private String ASM { get; set; }
        //public int RegisterPage { set; get; }
        /// <summary>
        /// Empty Instruction constructor. Generates a NOP instruction at memory
        /// address 0x0000;
        /// </summary>
        public Instruction()
        {
            binary = 0;
            BaseAddress = 0;
            rf = new RegisterFile();
            ASM = "NOP";
            
        }
        /// <summary>
        /// Instruction constructor. Decompiles binary data into its assembly mnemonic.
        /// </summary>
        /// <param name="Bin">14-bit Binary containing the instruction.</param>
        public Instruction(int Bin)
        {
            binary = Bin;
            BaseAddress = 0;
            rf = new RegisterFile();
            ASM = asmLookUp(Bin);
        }
        /// <summary>
        /// Constructor for an Instruction with a known memory location. Decompiles 
        /// the binary data into its assembly mnemonic.
        /// </summary>
        /// <param name="Bin">14-bit Binary containing the instruction.</param>
        /// <param name="Address">Memory location of the instruction.</param>
        public Instruction(int Bin, int Address)
        {
            binary = Bin;
            rf = new RegisterFile();
            BaseAddress = Address;
            ASM = asmLookUp(Bin);
        }

        public Instruction(int Bin, int Address, RegisterFile regin)
        {
            binary = Bin;
            rf = regin;
            BaseAddress = Address;
            ASM = asmLookUp(Bin);
        }
        /// <summary>
        /// This function decodes a PIC16F877 14-bit instruction into its assembly reference.
        /// </summary>
        /// <param name="Bin">14 bit coded Instruction.</param>
        /// <returns>Decoded assembly instruction with arguments.</returns>
        private string asmLookUp(int Bin)
        {
            string ASM = "";
            int f, b, k;
            if (Bin >> 12 == 0)         // Typical case for Byte Oriented File Register Instructions
            {
                if ((Bin & 0x0FFF) >> 8 == 0)       // 6 instructions start with 00 0000 xxxx xxxx
                {
                    if ((Bin & 0x009F) == 0)        // Case NOP: 00 0000 0xx0 0000
                        ASM = "NOP";
                    else if ((Bin & 0x0080) == 1)   // Case MOVWF: 00 0000 1fff ffff
                    {
                        f = Bin & 0x007F;
                        ASM = "MOVWF " + rf.decodeResgiterFile(f);
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
                        ASM = "CLRF " + rf.decodeResgiterFile(f);
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
                        ASM += rf.decodeResgiterFile(f) + ", f";
                    else
                        ASM += rf.decodeResgiterFile(f) + ", w";
                }
            }
            else if (Bin >> 12 == 1)    // Typical case for Bit Oriented File Register Instructions
            {
                f = Bin & 0x007F;
                b = (Bin & 0x0380)>>7;
                switch ((Bin & 0x0C00)>>10)
                {
                    case 0:                         // Case BCF:    01 00bb bfff ffff
                        ASM = "BCF ";
                        break;
                    case 1:                         // Case BSF:    01 01bb bfff ffff
                        ASM = "BSF ";
                        break;
                    case 2:                         // Case BTFSC:  01 10bb bfff ffff
                        ASM = "BTFSC ";
                        break;
                    case 3:                         // Case BTFSS:  01 11bb bfff ffff
                        ASM = "BTFSS ";
                        break;
                    default:
                        throw new Exception("Unknown OpCode.");
                        break;
                }
                ASM += rf.decodeResgiterFile(f) + ", " + b.ToString();
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

        /// <summary>
        /// Convert the instruction into a string mneumonic in the ISA.
        /// </summary>
        /// <returns>Mneumonic of the instruction.</returns>
        public override String ToString()
        {
            return ASM;
        }
    }
}

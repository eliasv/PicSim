using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    class Program
    {
        enum DataTypes {Program=0, EOF=1, ExtendedAddress=4};
        const int BYTEBLOCK = 2;
        public static RegisterFile RF = new RegisterFile();
        public static List<String> ByteOps;
        public static List<String> BitOps;
        public static List<String> LitControlOps;
        public static Queue<asmLabel> Labels = new Queue<asmLabel>();
        static void Main(string[] args)
        {
            List<String> HexCode;
            List <Instruction> ASM;
            
            Init();
            HexCode = readHex("flash.hex");
            //NoLines = HexCode.Length;
            ASM = decompile(HexCode);
            foreach (var line in ASM)
                Console.WriteLine(line.ToString());
        }

        /// <summary>
        /// Initialize three (3) sets in which the ISA is devided:
        ///     - Byte-Oriented File Register Operations
        ///     - Bit-Oriented File Register Operations
        ///     - Literal and Control Operations
        ///     
        /// The Byte Operations and the Bit operations have access to the 
        /// register file while the Lieral and Control Ops generate jumps
        /// and subroutin calls.
        /// </summary>
        private static void Init()
        {
            ByteOps = new List<string>();
            BitOps = new List<string>();
            LitControlOps = new List<string>();
            String[] byteOp = { "ADDWF","ANDWF","CLRF","CLRW","COMF","DECF",
                                "DECFSZ","INCF","INCFSZ","IORWF","MOVF","MOVWF",
                                "NOP","RLF","RRF","SUBWF","SWAPF","XORWF"
                            };
            String[] bitOp = {"BCF","BSF","BTFSC","BTFSS"};
            String[] LitOps = { "ADDLW","ANDLW","CALL","CLRWDT","GOTO",
                                "IORLW","MOVLW","RETFIE","RETLW","RETURN",
                                "SLEEP","SUBLW","XORLW"
                              };
            ByteOps.AddRange(byteOp);
            BitOps.AddRange(bitOp);
            LitControlOps.AddRange(LitOps);
            
        }


        /// <summary>
        /// Function to decompile the lines read from a hex file.
        /// 
        /// </summary>
        /// <param name="hex">Hex code read as an list of strings.</param>
        /// <returns>List of strings with the decompiled assembly.</returns>
        public static List<Instruction> decompile(List<String> hex)
        {
            int Bytes, BaseAddress, CheckSum, i, bin;
            DataTypes DataType;
            List<int> DataBytes = new List<int>();
            List<Instruction> sourceISR = new List<Instruction>();
            foreach(String line in hex)
            {
                Bytes = Convert.ToInt32(line.Substring(1, BYTEBLOCK), 16);
                BaseAddress = Convert.ToInt32(line.Substring(BYTEBLOCK+1, 2 * BYTEBLOCK), 16) >> 1;
                DataType = (DataTypes)Convert.ToInt32(line.Substring(3 * BYTEBLOCK+1, BYTEBLOCK), 16);
                for (i = 0; i < (Bytes * BYTEBLOCK) && (DataType != DataTypes.EOF); i += 2 * BYTEBLOCK)
                {
                    Instruction I;
                    asmLabel L;
                    String label = "";
                    String[] args;
                    // Due to the little endian design of the instruction format, bytes need to be reverse in order to be usable.
                    bin = Convert.ToInt32(line.Substring(i + 5 * BYTEBLOCK + 1, BYTEBLOCK) +
                                                    line.Substring(i + 4 * BYTEBLOCK + 1, BYTEBLOCK), 16);
                    DataBytes.Add(bin);
                    try
                    {
                        // Start the BT by modifying the CPU Registers.
                        L = new asmLabel("", BaseAddress);
                        I = new Instruction(bin, BaseAddress + i / (2 * BYTEBLOCK), ref RF);
                        
                        if (!LitControlOps.Contains(I.getmnemonic()))
                        {
                            if (BitOps.Contains(I.getmnemonic()))
                            {
                                switch (I.getmnemonic())
                                {
                                    case "BCF":
                                        args = I.getargs();
                                        RF.set(args[0], RF.get(args[0]) & ~((0x1 << Convert.ToInt32(args[1], 16))));
                                        break;
                                    case "BSF":
                                        args = I.getargs();
                                        RF.set(args[0], RF.get(args[0]) | ((0x1 << Convert.ToInt32(args[1], 16))));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            int addr;
                            switch(I.getmnemonic())
                            {
                                // Managing labels while decompiling.
                                case "CALL":
                                    args = I.getargs();
                                    addr = bin & 0x07FF;
                                    L.label = args[0];
                                    L.address = addr;
                                    if (addr < I.getAddress())
                                        sourceISR.Find(x => x.getAddress() == addr).setLabel(ref L);
                                    else
                                        Labels.Enqueue(new asmLabel(args[0], addr));
                                    break;
                                case "GOTO":
                                    args = I.getargs();
                                    addr = bin & 0x07FF;
                                    L.label = args[0];
                                    L.address = addr;
                                    if (addr < I.getAddress())
                                        sourceISR.Find(x => x.getAddress() == addr).setLabel(ref L);
                                    else
                                        Labels.Enqueue(new asmLabel(args[0], addr));
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (I.getAddress() == Labels.Peek().address)
                        {
                            L = Labels.Dequeue();
                            I.setLabel(ref L);
                        }
                        sourceISR.Add(I);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something horrible happenned:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Instruction skipped.");
                    }
                    
                }
                CheckSum = Convert.ToInt32(line.Substring(line.Length - BYTEBLOCK, BYTEBLOCK), 16);
            }
            return sourceISR;
        }

        public void insertLabels()
        {

        }

        /// <summary>
        /// Function reads a PIC-formated hex file and returns the lines 
        /// encountered. Each line needs to be decoded in order to decompile
        /// the binary.
        /// </summary>
        /// <param name="fname">Name of file to open.</param>
        /// <returns>Lines of the read hexfile.</returns>
        public static List<string> readHex(string fname)
        {
            int NoLines=0;
            List<String> HexCode = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(fname);
               
                    NoLines = 0;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        HexCode.Add(line);
                        Console.WriteLine(line);
                        NoLines++;
                    }
                    sr.Close();
                    sr.Dispose();
                
            }

            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return HexCode;
        }
    }
}

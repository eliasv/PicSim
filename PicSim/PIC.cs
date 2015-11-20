using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PicSim
{
    class PIC
    {
        enum DataTypes { Program = 0, EOF = 1, ExtendedAddress = 4 };
        const int BYTEBLOCK = 2;

        protected List<picWord> FLASH;
        private Stack<int> ptrTOS = new Stack<int>();
        private List<String> HexCode;
        protected RegisterFile rf = new RegisterFile();
        protected List<short> EEPROM = new List<short>();
        protected Instruction current;
        protected Instruction next;

        // Still need  to include a free running clock to handle the execution speed. 
        // Best possible solution is a timer object which would generate the execution 
        // events and keep the architecture up to date.


        public PIC(List<String> Binary)
        {
            HexCode = Binary;
            FLASH = decompile();
        }

        public PIC(ref RegisterFile memorymap, List<String> Code)
        {
            rf = memorymap;
            HexCode = Code;
            FLASH = decompile(); 
        }

        public PIC(ref RegisterFile mm, List<picWord> program)
        {
            FLASH = program;
            rf = mm;
        }

        /// <summary>
        /// Function reads a PIC-formated hex file and returns the lines 
        /// encountered. Each line needs to be decoded in order to decompile
        /// the binary.
        /// </summary>
        /// <param name="fn">Name of file to open.</param>
        public PIC(String fn)
        {
                try
                {
                    StreamReader sr = new StreamReader(fn);

                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        HexCode.Add(line);
                        Console.WriteLine(line);
                    }
                    sr.Close();
                    sr.Dispose();
                    FLASH = decompile(); 
                }

                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
                
        }

        /// <summary>
        /// Function to decompile the lines read from a hex file.
        /// </summary>
        /// <returns>List with the flash memory abstraction.</returns>
        public List<picWord> decompile()
        {
            int Bytes, BaseAddress, CheckSum, i, bin;
            DataTypes DataType;
            List<int> DataBytes = new List<int>();
            List<picWord> sourceISR = new List<picWord>();
            foreach (String line in HexCode)
            {
                Bytes = Convert.ToInt32(line.Substring(1, BYTEBLOCK), 16);
                BaseAddress = Convert.ToInt32(line.Substring(BYTEBLOCK + 1, 2 * BYTEBLOCK), 16) >> 1;
                DataType = (DataTypes)Convert.ToInt32(line.Substring(3 * BYTEBLOCK + 1, BYTEBLOCK), 16);
                if (BaseAddress == 0x2007)
                {
                    bin = Convert.ToInt32(line.Substring(5 * BYTEBLOCK + 1, BYTEBLOCK) +
                                                        line.Substring(4 * BYTEBLOCK + 1, BYTEBLOCK), 16);
                    sourceISR.Add(new picWord(bin, BaseAddress));
                    continue;   // This is a configuration word... and its extremely poorly documented...
                }
                else if (DataType == DataTypes.ExtendedAddress)
                {
                    for (i = 0; i < (Bytes * BYTEBLOCK) && (DataType == DataTypes.Program); i += 2 * BYTEBLOCK)
                    {
                        // Due to the little endian design of the instruction format, bytes need to be reverse in order to be usable.
                        bin = Convert.ToInt32(line.Substring(i + 5 * BYTEBLOCK + 1, BYTEBLOCK) +
                                                        line.Substring(i + 4 * BYTEBLOCK + 1, BYTEBLOCK), 16);
                        sourceISR.Add(new picWord(bin, BaseAddress));
                    }
                }
                else
                {
                    for (i = 0; i < (Bytes * BYTEBLOCK) && (DataType == DataTypes.Program); i += 2 * BYTEBLOCK)
                    {
                        Queue<asmLabel> Labels = new Queue<asmLabel>();
                        Instruction I;
                        asmLabel L;
                        String[] args;
                        // Due to the little endian design of the instruction format, bytes need to be reverse in order to be usable.
                        bin = Convert.ToInt32(line.Substring(i + 5 * BYTEBLOCK + 1, BYTEBLOCK) +
                                                        line.Substring(i + 4 * BYTEBLOCK + 1, BYTEBLOCK), 16);
                        DataBytes.Add(bin);
                        try
                        {
                            // Start the BT by modifying the CPU Registers.
                            L = new asmLabel("", BaseAddress);
                            I = new Instruction(bin, BaseAddress + i / (2 * BYTEBLOCK), ref rf, ref ptrTOS);
                            args = I.getargs();
                            int addr = I.getAddress();
                            switch (I.getmnemonic())
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
                            if ((Labels.Count > 0) && (I.getAddress() == Labels.Peek().address))
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
                            //throw e;
                        }

                    }
                }
                CheckSum = Convert.ToInt32(line.Substring(line.Length - BYTEBLOCK, BYTEBLOCK), 16);
            }
            return sourceISR;
        }
    }
}

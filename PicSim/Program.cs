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
        enum DataTypes {Program, EOF, ExtendedAddress=4};
        const int BYTEBLOCK = 2;
        static void Main(string[] args)
        {
            int NoLines;
            List<String> HexCode, ASM;
            HexCode = readHex("flash.hex");
            //NoLines = HexCode.Length;
            ASM = decompile(HexCode);
        }
        /// <summary>
        /// Function to decompile the lines read from a
        /// hex file.
        /// </summary>
        /// <param name="hex">Hex code read as an array of strings.</param>
        /// <returns>Array of strings with the decompiled assembly.</returns>
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

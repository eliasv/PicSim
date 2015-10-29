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
        static void Main(string[] args)
        {
            int NoLines;
            String[] HexCode, ASM;
            HexCode = readHex("flash.hex");
            NoLines = HexCode.Length;
            ASM = decompile(HexCode);
        }
        /// <summary>
        /// Function to decompile the lines read from a
        /// hex file.
        /// </summary>
        /// <param name="hex">Hex code read as an array of strings.</param>
        /// <returns>Array of strings with the decompiled assembly.</returns>
        public static string[] decompile(String[] hex)
        {
            int Bytes, BaseAddress, DataType, DataBytes, CheckSum;
            String[] sourceISR = new String[0];
            foreach(String line in hex)
            {
                Bytes = Convert.ToInt32(line.Substring(1, 2), 16);
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
        public static string[] readHex(string fname)
        {
            int NoLines=0;
            String[] HexCode;
            try
            {
                StreamReader sr = new StreamReader(fname);
               
                    NoLines = 0;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        Console.WriteLine(line);
                        NoLines++;
                    }
                    HexCode = new String[NoLines];
                    sr.Close();
                    sr.Dispose();

                    StreamReader nr = new StreamReader(fname);
                
                    int i = 0;
                    while (!nr.EndOfStream)
                    {
                        HexCode[i] = nr.ReadLine();
                    }
                    nr.Close();
                    nr.Dispose();
                
            }

            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                HexCode = new String[0];
            }
            return HexCode;
        }
    }
}

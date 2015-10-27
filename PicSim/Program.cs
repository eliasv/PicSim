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
            String[] HexCode;
            HexCode = readHex("flash.hex");
            NoLines = HexCode.Length;
        }

        public static string[] decompile(string hex)
        {
            String[] sourceISR = new String[0];

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
                using (StreamReader sr = new StreamReader(fname))
                {
                    NoLines = 0;
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        Console.WriteLine(line);
                        NoLines++;
                    }
                    HexCode = new String[NoLines];
                    sr.DiscardBufferedData();
                    sr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                    int i = 0;
                    while (!sr.EndOfStream)
                    {
                        HexCode[i] = sr.ReadLine();
                    }
                }
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

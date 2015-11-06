using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    /// <summary>
    /// Abstraction of the Register File for the PIC18F877.
    /// Implemented as a CSV spreadsheet where the memory banks are represented as
    /// columns and the relative addreses are the rows.
    /// </summary>
    class RegisterFile
    {
        public List<String>[] RegFileNames;
        public List<int>[] RegFile;
        public int[] offset;
        private int RegisterPage { set; get; }
        private const int BANKS = 4;
        public RegisterFile()
        {
            RegFileNames = new List<string>[BANKS];
            RegFile = new List<int>[BANKS];
            offset = new int[BANKS];
            int i = 0;
            try
            {
                var lines = File.ReadAllLines("RegFile.csv");
                for (i = 0; i < 4; i++)
                    RegFileNames[i] = new List<string>();
                for (i = 0; i < 4; i++)
                    RegFile[i] = new List<int>();
                var parsed = from line in lines
                             select (line.Split(',')).ToArray();
                i = 0;
                foreach (var line in parsed)
                {
                    RegFile[0].Add(0); RegFile[1].Add(0); RegFile[2].Add(0); RegFile[3].Add(0);
                    RegFileNames[0].Add(parsed.ElementAt(i).ElementAt(0));
                    RegFileNames[1].Add(parsed.ElementAt(i).ElementAt(1));
                    RegFileNames[2].Add(parsed.ElementAt(i).ElementAt(2));
                    RegFileNames[3].Add(parsed.ElementAt(i++).ElementAt(3));
                }
                offset[0] = 0; offset[1] = 0x80;
                offset[2] = 0x100; offset[3] = 0x180;

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void set(String regName, int value)
        {
            bool[] present = new bool[4];
            for(int i =0; i < BANKS; i++)
            {
                present[i] = RegFileNames[i].Contains(regName);
            }
        }

        public int set(String regName)
        {
            return 0;
        }

        public string decodeResgiterFile(int f)
        {
            String reg = "";
            reg = RegFileNames[RegisterPage].ElementAt(f);
            if (!reg.Equals("") && !reg.Equals("N/I") && !reg.Equals("Reserved"))
                return reg;
            else throw new Exception("Unknown Register.");
        }

    }
}

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
        public List<register>[] RegFile;
        public int W;
        public int[] offset;
        private int RegisterPage { set; get; }
        private const int BANKS = 4;
        public RegisterFile()
        {
            RegFile = new List<register>[BANKS];
            offset = new int[BANKS];
            int i = 0;
            try
            {
                var lines = File.ReadAllLines("RegFile.csv");
                for (i = 0; i < 4; i++)
                    RegFile[i] = new List<register>();
                var parsed = from line in lines
                             select (line.Split(',')).ToArray();
                i = 0;
                foreach (var line in parsed)
                {
                    RegFile[0].Add(new register(parsed.ElementAt(i).ElementAt(0)));
                    RegFile[1].Add(new register(parsed.ElementAt(i).ElementAt(1)));
                    RegFile[2].Add(new register(parsed.ElementAt(i).ElementAt(2)));
                    RegFile[3].Add(new register(parsed.ElementAt(i++).ElementAt(3)));
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
            int i;
            int index = -1;
            if ((regName == "W") || (regName == "w"))
                W = value;
            else
            {
                for (i = 0; i < BANKS; i++)
                {
                    if (RegFile[i].Exists(x => x.name == regName))
                    {
                        index = RegFile[i].FindIndex(x => x.name == regName);
                        RegFile[i].ElementAt(index).value = value;
                        if (regName == "STATUS")
                        {
                            RegisterPage = (value & 0x60) >> 5;
                        }
                    }
                }
            }
            if (value == 0)
            {
                setZeroFlag();
            }
        }

        public void set(int address, int value)
        {
            int oldpage = RegisterPage;
            // Decode RF page from address
            RegisterPage = (address & 0x180) >> 8;
            RegFile[RegisterPage].ElementAt(address & 0x07F).value = value;
            if (value == 0)
            {
                setZeroFlag();
            }
        }

        private void setZeroFlag()
        {
            for (int i = 0; i < BANKS; i++)
            {
                if (RegFile[i].Exists(x => x.name == "STATUS"))
                {
                    int index = RegFile[i].FindIndex(x => x.name == "STATUS");
                    RegFile[i].ElementAt(index).value = RegFile[i].ElementAt(index).value & 0x04;
                }
            }
        }

        public int get(String regName)
        {
            int i;
            if ((regName== "W")|| (regName=="w"))
                return W;
            else
            {
                for (i = 0; i < BANKS; i++)
                {
                    if (RegFile[i].Exists(x => x.name == regName))
                    {
                        return RegFile[i].First(x => x.name == regName).value;
                    }
                }
            }
            
            for (i = 0; i < BANKS; i++)
            {
                if (RegFile[i].Exists(x => x.name == decodeResgiterFile(Convert.ToInt32(regName,16))))
                {
                    return RegFile[i].First(x => x.name == decodeResgiterFile(Convert.ToInt32(regName,16))).value;
                }
            }

            throw new Exception("Unknown Register.");
        }

        public string decodeResgiterFile(int f)
        {
            String reg = "";
            reg = RegFile[RegisterPage].ElementAt(f).name;
            if (!reg.Equals("") && !reg.Equals("Reserved") && !reg.Equals("N/I") && !reg.Equals("Accesses"))
                return reg;
            else
                return String.Format("0x{0}",(offset[RegisterPage] + f).ToString("X4"));
            // else throw new Exception("Unknown Register.");
        }

    }
}

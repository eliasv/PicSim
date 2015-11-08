﻿using System;
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
            for (i = 0; i < BANKS; i++)
            {
                if (RegFile[i].Exists(x=>x.name==regName))
                {
                    index = RegFile[i].FindIndex(x=>x.name==regName);
                    RegFile[i].ElementAt(index).value = value;
                }
            }
        }

        public int get(String regName)
        {
            int i;
            for (i = 0; i < BANKS; i++)
            {
                if (RegFile[i].Exists(x => x.name == regName))
                {
                    return RegFile[i].First(x => x.name == regName).value;
                }
            }
            throw new Exception("Unknown Register.");
        }

        public string decodeResgiterFile(int f)
        {
            String reg = "";
            reg = RegFile[RegisterPage].ElementAt(f).name;
            if (!reg.Equals("") && !reg.Equals("N/I") && !reg.Equals("Reserved") && !reg.Equals("Accesses"))
                return reg;
            else throw new Exception("Unknown Register.");
        }

    }
}

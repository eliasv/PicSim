using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    class RegisterFile
    {
        public List<String>[] RegFileNames;
        public List<int>[] RegFile;

        public RegisterFile()
        {
            RegFileNames = new List<string>[4];
            RegFile = new List<int>[4];
            int i = 0;
            var lines = File.ReadAllLines("RegFile.csv");
            for (i = 0; i < 4; i++ )
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
        }

    }
}

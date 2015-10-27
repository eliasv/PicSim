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
            try
            {
                using(StreamReader sr = new StreamReader("flash.hex"))
                {
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}

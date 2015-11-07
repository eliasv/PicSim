using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSim
{
    class register
    {
        public string name;
        public int value { set; get; }
        public register(string n)
        {
            name = n;
            value = 0;
        }

        public register(string n, int v)
        {
            name = n;
            value = v;
        }
        public register(int v, string n)
        {
            name = n;
            value = v;
        }
    }
}

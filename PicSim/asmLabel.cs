using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicLib
{
    public class asmLabel:IComparable<asmLabel>
    {
        public String label { get; set; }
        public int address { get; set; }
        public bool placed { get; set; }

        public asmLabel(string L, int Addr)
        {
            label = L;
            address = Addr;
            placed = false;
        }
                
        public int CompareTo(asmLabel Object)
        {
            return address.CompareTo(Object.address);
        }

        public override string ToString()
        {
            if (label == "")
                return "";
            else
                return label+":";
        }
    }
}

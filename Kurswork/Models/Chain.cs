using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurswork.Models
{
    public class Chain : ICloneable
    {
        public string Str { get; set; } = "";
        public string End { get; set; }
        public int Count => Str.Length;
        public List<string> RegularsList { get; set; }

        public object Clone()
        {
            var item = (Chain)this.MemberwiseClone();
            item.RegularsList = RegularsList.ToList();
            return item;
        }

        public override bool Equals(object obj)
        {
            var x = (Chain) obj;
            var isStr = Str.Equals(x.Str);
            return isStr;
            var isLamb = End.Equals(x.End);
            return isLamb & isStr;
        }

        public string GetRegularsListString()
        {
            if (RegularsList == null)
            {
                return "";
            }
            var str = "";
            foreach (var reg in RegularsList)
            {
                str += "->" + reg;
            }

            return str.Substring(2);
        }
    }

    public class VNSimvol
    {
        public string VN { get; set; }
        public int Index { get; set; }
    }
}

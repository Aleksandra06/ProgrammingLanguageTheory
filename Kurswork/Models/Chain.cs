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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            var x = (Chain) obj;
            var isStr = Str.Equals(x.Str);
            var isLamb = End.Equals(x.End);
            return isLamb & isStr;
        }
    }
}

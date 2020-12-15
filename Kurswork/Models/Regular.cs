using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurswork.Models
{
    public class Regular
    {
        public string left { get; set; }
        public List<List<string>> right { get; set; }

        public override string ToString()
        {
            if (right == null || right?.Count == 0)
            {
                return "";
            }

            var str = "";
            str += left + "->";
            foreach (var reg in right[0])
            {
                str += reg;
            }
            for (var index = 1; index < right.Count; index++)
            {
                var regular = right[index];
                str += "|";
                foreach (var reg in regular)
                {
                    str += reg;
                }
            }

            return str;
        }
    }
}

using System.Collections.Generic;

namespace Kurswork.Models
{
    public class Regular
    {
        public string Left { get; set; }
        public List<List<string>> Right { get; set; }

        public override string ToString()
        {
            if (Right == null || Right?.Count == 0)
            {
                return "";
            }

            var str = "";
            str += Left + "->";
            foreach (var reg in Right[0])
            {
                str += reg;
            }
            for (var index = 1; index < Right.Count; index++)
            {
                var regular = Right[index];
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

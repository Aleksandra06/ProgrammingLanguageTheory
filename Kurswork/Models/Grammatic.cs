using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurswork.Models
{
    public class Grammatic
    {
        public List<string> VT;
        public List<string> VN;
        public List<Regular> Regulation { get; set; }
        public string Lambda = "*";
        public string Start;

        public override string ToString()
        {
            try
            {
                if (Regulation == null)
                {
                    return "";
                }

                string str = "";
                foreach (var regulars in Regulation)
                {
                    str += Environment.NewLine + regulars.ToString();
                    //str += Environment.NewLine + regulars.left + "->";
                    //foreach (var reg in regulars.right[0])
                    //{
                    //    str += reg;
                    //}
                    //for (var index = 1; index < regulars.right.Count; index++)
                    //{
                    //    var regular = regulars.right[index];
                    //    str += "|";
                    //    foreach (var reg in regular)
                    //    {
                    //        str += reg;
                    //    }
                    //}
                }

                return str.Substring(1);
            }
            catch
            {
                throw new Exception("Ошибка построения грамматики в текстовом виде.");
            }
        }

        public Grammatic ConvertToBNFGrammatik()
        {
            var BNF = new Grammatic();
            BNF.VN = VN;
            BNF.VT = VT;
            BNF.Start = Start;
            BNF.Lambda = Lambda;
            BNF.Regulation = new List<Regular>();
            var regulation = Regulation.ToList();

            for (int i = 0; i < regulation.Count; i++)
            {
                if (regulation[i].right.Count != 1)
                {
                    continue;
                }

                if (regulation[i].right[0].Count != 1)
                {
                    continue;
                }

                if (!VN.Any(x => x.Equals(regulation[i].right.FirstOrDefault())))
                {
                    BNF.Regulation.Add(regulation[i]);
                    regulation.RemoveAt(i);
                    i--;
                }
            }

            foreach (var regular in regulation)
            {
                BNF.Regulation.Add(new Regular() { left = regular.left });
                BNF.Regulation.LastOrDefault().right = new List<List<string>>();
                var nowRegulation = BNF.Regulation.LastOrDefault().right;

                foreach (var p in regular.right)
                {
                    var vnCount = VNCount(p, VN);
                    //1
                    if ((vnCount == 2 && p.Count == 2) || (vnCount == 0 && p.Count == 1))
                    {
                        nowRegulation.Add(p);
                        continue;
                    }
                    //2 и 3
                    if (vnCount == 1 && p.Count == 2)
                    {
                        var term = VN.Any(x => x.Equals(p[0])) ? p[1] : p[0];
                        var simv = GetRegularVNByVT(BNF.Regulation, term);
                        if (string.IsNullOrEmpty(simv))
                        {
                            var newVN = CreateVN(BNF.VN);
                            BNF.VN.Add(newVN);
                            var newRegular = new List<string>();
                            string simvols = "";
                            foreach (var ch in p)
                            {
                                if (BNF.VN.Any(x => x.Equals(ch)))
                                {
                                    newRegular.Add(ch);
                                }
                                else
                                {
                                    newRegular.Add(newVN);
                                    simvols = ch;
                                }
                            }

                            nowRegulation.Add(newRegular);
                            BNF.Regulation.Add(new Regular()
                            {
                                left = newVN,
                                right = new List<List<string>>()
                                {
                                    new List<string>() {simvols}
                                }
                            });
                            continue;
                        }

                        var newRegular2 = new List<string>();
                        foreach (var ch in p)
                        {
                            if (BNF.VN.Any(x => x.Equals(ch)))
                            {
                                newRegular2.Add(ch);
                            }
                            else
                            {
                                newRegular2.Add(simv);
                            }
                        }

                        nowRegulation.Add(newRegular2);
                        continue;
                    }

                    //4
                    if (vnCount == 0 && p.Count == 2)
                    {
                        var sim1 = GetRegularVNByVT(BNF.Regulation, p[0]);
                        if (string.IsNullOrEmpty(sim1))
                        {
                            sim1 = CreateVN(BNF.VN);
                            BNF.VN.Add(sim1);
                            BNF.Regulation.Add(new Regular()
                            {
                                left = sim1,
                                right = new List<List<string>>()
                                {
                                    new List<string>() {p[0]}
                                }
                            });
                        }
                        var sim2 = GetRegularVNByVT(BNF.Regulation, p[1]);
                        if (string.IsNullOrEmpty(sim2))
                        {
                            sim2 = CreateVN(BNF.VN);
                            BNF.VN.Add(sim2);
                            BNF.Regulation.Add(new Regular()
                            {
                                left = sim2,
                                right = new List<List<string>>()
                                {
                                    new List<string>() {p[1]}
                                }
                            });
                        }
                        nowRegulation.Add(new List<string>() { sim1, sim2 });
                        continue;
                    }

                    //5
                    var newSim = CreateVN(BNF.VN);
                    BNF.VN.Add(newSim);
                    if (VN.Any(x => x.Equals(p[0])))
                    {
                        nowRegulation.Add(new List<string>() { p[0], newSim });
                    }
                    else
                    {
                        var sim2 = GetRegularVNByVT(BNF.Regulation, p[0]);
                        if (string.IsNullOrEmpty(sim2))
                        {
                            sim2 = CreateVN(BNF.VN);
                            BNF.VN.Add(sim2);
                            BNF.Regulation.Add(new Regular()
                            {
                                left = sim2,
                                right = new List<List<string>>()
                                {
                                    new List<string>() {p[0]}
                                }
                            });
                        }
                        nowRegulation.Add(new List<string>() { sim2, newSim });
                    }
                    for (var index = 1; index < p.Count - 1; index++)
                    {
                        Regular reg = new Regular();
                        reg.left = newSim;
                        reg.right = new List<List<string>>();
                        BNF.VN.Add(newSim);
                        var ch = p[index];
                        if (index == p.Count - 2)
                        {
                            var lastSim = p[p.Count - 1];
                            if (VN.Any(x => x.Equals(lastSim)))
                            {
                                newSim = lastSim;
                            }
                            else
                            {
                                var sim2 = GetRegularVNByVT(BNF.Regulation, lastSim);
                                if (string.IsNullOrEmpty(sim2))
                                {
                                    sim2 = CreateVN(BNF.VN);
                                    BNF.VN.Add(sim2);
                                    BNF.Regulation.Add(new Regular()
                                    {
                                        left = sim2,
                                        right = new List<List<string>>()
                                        {
                                            new List<string>() { lastSim }
                                        }
                                    });
                                }
                                newSim = sim2;
                            }
                        }
                        else
                        {
                            newSim = CreateVN(BNF.VN);
                        }
                        if (VN.Any(x => x.Equals(ch)))
                        {
                            reg.right.Add(new List<string>() { ch, newSim });
                        }
                        else
                        {
                            var sim2 = GetRegularVNByVT(BNF.Regulation, ch);
                            if (string.IsNullOrEmpty(sim2))
                            {
                                sim2 = CreateVN(BNF.VN);
                                BNF.VN.Add(sim2);
                                BNF.Regulation.Add(new Regular()
                                {
                                    left = sim2,
                                    right = new List<List<string>>()
                                    {
                                        new List<string>() { ch }
                                    }
                                });
                            }
                            reg.right.Add(new List<string>() { sim2, newSim });
                        }
                        BNF.Regulation.Add(reg);
                    }
                    //Regular reg1 = new Regular();
                    //reg1.left = newSim;
                    //reg1.right = new List<List<string>>();
                    //if (VN.Any(x => x.Equals(p[p.Count - 1])))
                    //{
                    //    reg1.right.Add(new List<string>() { p[p.Count - 1], newSim });
                    //}
                    //else
                    //{
                    //    var sim2 = GetRegularVNByVT(BNF.Regulation, p[p.Count - 1]);
                    //    if (string.IsNullOrEmpty(sim2))
                    //    {
                    //        var newVN = CreateVN(BNF.VN);
                    //        BNF.VN.Add(newVN);
                    //        BNF.Regulation.Add(new Regular()
                    //        {
                    //            left = newVN,
                    //            right = new List<List<string>>()
                    //            {
                    //                new List<string>() { p[p.Count - 1] }
                    //            }
                    //        });
                    //    }
                    //    reg1.right.Add(new List<string>() { sim2, newSim });
                    //}
                }
            }

            return BNF;
        }

        private int GetCountRegular(List<string> list)
        {
            var count = 0;
            foreach (var str in list)
            {
                count += str.Trim().Length;
            }

            return count;
        }

        private string GetRegularVNByVT(List<Regular> grammatic, string chain)
        {
            var list = grammatic.Where(x => x.right.Count == 1).ToList();
            if (list == null || list?.Count == 0)
            {
                return null;
            }

            var elements = list.Where(x => x.right[0].Count == 1).ToList();
            if (elements == null || elements?.Count == 0)
            {
                return null;
            }

            var element = elements.Where(x => x.right[0][0].Equals(chain)).ToList();
            if (element == null || element?.Count == 0)
            {
                return null;
            }

            return element.FirstOrDefault().left;
        }

        private string CreateVN(List<string> vn)
        {
            var simvols = "QWERTYUPPASDFGHJKLZXCVBNM";
            foreach (var simvol in simvols)
            {
                if (vn.Any(x => x.Equals(simvol.ToString())))
                {
                    continue;
                }

                return simvol.ToString();
            }

            throw new Exception("Нет больше символов для нетерминального алфавита!");
        }

        private int VNCount(List<string> regList, List<string> vnList)
        {
            int count = 0;
            foreach (var reg in regList)
            {
                if (vnList.Any(x => x.Equals(reg)))
                {
                    count++;
                }
            }

            return count;
        }

        //private bool CheckVTAndVNTogether(List<string> list, List<string> vt)
        //{
        //    bool? startVt = null;
        //    bool? endVt = null;
        //    bool? startVn = null;
        //    bool? endVn = null;
        //    for (var index = 0; index < list.Count; index++)
        //    {
        //        var ch = list[index];
        //        if (vt.Any(x => x.Equals(ch)))
        //        {
        //            startVt = true;
        //            if (endVt == true && endVn == false)
        //            {
        //                return false;
        //            }
        //            endVt = false;
        //            if (startVn == true)
        //            {
        //                endVn = true;
        //            }
        //        }
        //        else
        //        {
        //            startVn = true;
        //            if (endVn == true && endVt == false)
        //            {
        //                return false;
        //            }
        //            endVn = false;
        //            if (startVt == true)
        //            {
        //                endVt = true;
        //            }
        //        }
        //    }

        //    return true;
        //}

        public void SetGrammaticByFile(string str)
        {
            // Terminal
            var index1 = str.IndexOf("\n");
            var term = str.Substring(0, index1);
            SetVT(term);

            // NoTerminal
            str = str.Substring(index1 + 1);
            index1 = str.IndexOf("\n");
            var nterm = str.Substring(0, index1);
            SetVN(nterm);

            // start
            str = str.Substring(index1 + 1);
            index1 = str.IndexOf("\n");
            var start = str.Substring(0, index1 - 1);
            SetStartChar(start);

            //lambda
            str = str.Substring(index1 + 1);
            index1 = str.IndexOf("\n");
            var lamb = str.Substring(0, index1);
            Lambda = lamb.Trim();

            // Regular
            str = str.Substring(index1 + 1);
            SetGrammaticKS(str);
        }

        public void SetVN(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new Exception("Отсутствуют нетерминальные символы!");
            }
            var splitVtText = str.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList();
            List<string> symbols = new List<string>();
            foreach (var s in splitVtText)
            {
                if (s.Length > 1)
                {
                    throw new Exception("В нетерминальном алфавите имеется строка!");
                }
                if ("0987654321abcdefghijklmnopqrstuvwxyz".Contains(s[0]))
                {
                    throw new Exception("В нетерминальном алфавите имеются недопустимые символы!");
                }
                symbols.Add(s);
            }

            VN = symbols;
        }

        public void SetVT(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new Exception("Алфавит не заполнен!");
            }

            var splitVtText = str.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList();
            List<string> alphabetSymbols = new List<string>();
            foreach (string s in splitVtText)
            {
                if (s.Length > 1)
                {
                    throw new Exception("В алфавите имеется строка!");
                }
                if (!"0987654321abcdefghijklmnopqrstuvwxyz".Contains(s[0]))
                {
                    throw new Exception("В алфавите имеются недопустимые символы!");
                }
                alphabetSymbols.Add(s);
            }

            VT = alphabetSymbols;
        }

        public void SetStartChar(string str)
        {
            var count = 0;
            foreach (var ch in VN)
            {
                if (str.Equals(ch))
                {
                    count++;
                    break;
                }
            }

            if (count == 0)
            {
                throw new Exception("Неверно задан стартовый символ!");
            }
            else
            {
                Start = str;
            }
        }

        public void SetLambda(string str)
        {
            if (VN.Any(ch => str.Equals(ch)) || VT.Any(ch => str.Equals(ch)))
            {
                throw new Exception("Неверно задана лямбда!");
            }
            else
            {
                Lambda = str;
            }
        }

        public void SetGrammaticKS(string str)
        {
            try
            {
                str = str.Replace("\r", "");
                if (str[0] == '\n')
                {
                    str = str.Substring(1);
                }
                List<Regular> list = new List<Regular>();
                while (str.Length > 1)
                {
                    Regular regular = new Regular();
                    regular.right = new List<List<string>>();
                    var index = str.IndexOf("-");
                    var ch = str.Substring(0, index);
                    if (ch.Length != 1)
                    {
                        throw new Exception("В левой части правила больше одного символа!");
                    }

                    if ("0987654321abcdefghijklmnopqrstuvwxyz".Contains(ch))
                    {
                        throw new Exception("В левой части правила находится терминальный символ!");
                    }
                    regular.left = ch;

                    str = str.Substring(index + 2);
                    index = str.IndexOf("\n");
                    if (index < 0)
                    {
                        var tmp1 = ConvertStringToStringList(str.Substring(0), "|");
                        foreach (var tmp in tmp1)
                        {
                            regular.right.Add(ConvertStringToStringList(tmp));
                        }

                        //if (regular.right.Any(x => !x.Any(y => !"0987654321abcdefghijklmnopqrstuvwxyz".Contains(y) && !y.Equals(Lambda))))
                        //{
                        //    throw new Exception("в правой части правила находится только нетерминальный символ!");
                        //}

                        list.Add(regular);
                        break;
                    }

                    var tmp2 = ConvertStringToStringList(str.Substring(0, index), "|");
                    foreach (var tmp in tmp2)
                    {

                        regular.right.Add(ConvertStringToStringList(tmp));
                    }

                    str = str.Substring(index + 1);
                    list.Add(regular);
                }

                Regulation = list;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ex.Message))
                {
                    throw new Exception("Ошибка чтения КС-грамматики!");
                }
                else
                {
                    throw ex;
                }
            }
        }

        private List<string> ConvertStringToStringList(string str, string border)
        {
            List<string> list = new List<string>();
            while (str.Length > 0)
            {
                var index = str.IndexOf(border);
                if (index == -1)
                {
                    break;
                }

                list.Add(str.Substring(0, index));
                str = str.Substring(index + 1);
            }

            var index1 = str.IndexOf("\n");
            if (index1 < 0)
            {
                list.Add(str.Substring(0));
            }
            else
            {
                list.Add(str.Substring(0, index1));
            }

            return list;
        }

        private List<string> ConvertStringToStringList(string str)
        {
            List<string> list = new List<string>();

            if (Lambda.Equals(str))
            {
                list.Add(Lambda);
                return list;
            }

            foreach (var ch in str)
            {
                if (VN.Any(x => x.Equals(ch.ToString())) ||
                    VT.Any(x => x.Equals(ch.ToString())))
                {
                    list.Add(ch.ToString());
                }
                else
                {
                    throw new Exception("В грамматике присутствует неопределенный символ");
                }
            }

            //int move = 0;
            //for (int i = 0; i < str.Length; i++)
            //{
            //    foreach (var gramVt in VT)
            //    {
            //        if (gramVt.Equals(str[i].ToString()))
            //        {
            //            move++;
            //        }
            //    }
            //}
            //if (move > 0)
            //{
            //    list.Add(str.Substring(0, move));
            //}
            //str = str.Substring(move);

            //move = 0;
            //for (int i = 0; i < str.Length; i++)
            //{
            //    foreach (var gramVt in VN)
            //    {
            //        if (gramVt.Equals(str[i].ToString()))
            //        {
            //            move++;
            //        }
            //    }
            //}

            //if (move > 0)
            //{
            //    list.Add(str.Substring(0, 1));
            //}

            return list;
        }

        public string GetVTString()
        {
            return ListStringToString(VT);
        }

        public string GetVNString()
        {
            return ListStringToString(VN);
        }

        private string ListStringToString(List<string> list)
        {
            if (list == null)
            {
                return "";
            }

            string str = "";
            foreach (var item in list)
            {
                str += "," + item;
            }

            return str.Substring(1);
        }
    }
}

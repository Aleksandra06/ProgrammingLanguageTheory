using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kurswork.Models;

namespace Kurswork
{
    public partial class Form1 : Form
    {
        private List<string> VT;
        private List<string> VN;
        private List<Regular> Regulation { get; set; }
        private string Lambda;
        private string Start;

        private bool isSuccessfullyGrammaticKS = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Пляскина Александра Юрьевна ИП-715", "Автор", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void темаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = @"
Преобразования КС-грамматик и виды разбора.
Написать программу, которая будет принимать на вход контекстно-свободную грамматику в каноническом виде (проверить корректность задания и при отрицательном результате выдать соответствующее сообщение) и приведёт её к нормальной форме Хомского. 
Программа должна проверить построенную грамматику (БНФ) на эквивалентность исходной: по обеим грамматикам сгенерировать множества всех цепочек в заданном пользователем диапазоне длин и проверить их на идентичность. 
Для подтверждения корректности выполняемых действий предусмотреть возможность корректировки любого из построенных множеств пользователем (изменение цепочки, добавление, удаление…). 
При обнаружении несовпадения должна выдаваться диагностика различий – где именно несовпадения и в чём они состоят. Построить дерево вывода для любой выбранной цепочки из числа сгенерированных.
";
            MessageBox.Show(str, "Тема", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            try
            {
                InputVT(textBox3.Text);
                InputNV(textBox4.Text);
                InputStartChar(textBox5.Text);
                Lambda = textBox6.Text.Trim();
                InputGrammatic(textBox2.Text);

                isSuccessfullyGrammaticKS = true;
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
                isSuccessfullyGrammaticKS = false;
            }
        }
        private void InputGrammatic(string str)
        {
            try
            {
                str = str.Replace("\r", "");
                List<Regular> list = new List<Regular>();
                while (str.Length > 1)
                {
                    Regular regular = new Regular();
                    regular.right = new List<List<string>>();
                    var index = str.IndexOf("-");
                    var ch = str.Substring(0, index);
                    if (ch.Length > 1)
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

                        if (regular.right.Any(x => !x.Any(y => !"0987654321abcdefghijklmnopqrstuvwxyz".Contains(y))))
                        {
                            throw new Exception("в правой части правила находится только нетерминальный символ!");
                        }

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

                textBox1.Text += Environment.NewLine + "Грамматика считана успешно";
                Regulation = list;
            }
            catch(Exception ex)
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
                list.Add(Lambda.ToString());
                return list;
            }

            int move = 0;
            for (int i = 0; i < str.Length; i++)
            {
                foreach (var gramVt in VT)
                {
                    if (gramVt.Equals(str[i].ToString()))
                    {
                        move++;
                    }
                }
            }
            if (move > 0)
            {
                list.Add(str.Substring(0, move));
            }
            str = str.Substring(move);

            move = 0;
            for (int i = 0; i < str.Length; i++)
            {
                foreach (var gramVt in VN)
                {
                    if (gramVt.Equals(str[i].ToString()))
                    {
                        move++;
                    }
                }
            }

            if (move > 0)
            {
                list.Add(str.Substring(0, 1));
            }

            return list;
        }

        private void InputStartChar(string str)
        {
            var count = 0;
            foreach (var ch in VN)
            {
                if (str.Contains(ch))
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

            textBox1.Text += Environment.NewLine + "Стартовый символ считан успешно";
        }

        private void InputNV(string str)
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
            textBox1.Text += Environment.NewLine + "Нетерминальный алфавит считан успешно";
        }

        void InputVT(string str)
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
            textBox1.Text += Environment.NewLine + "Алфавит считан успешно";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            try
            {
                var a = int.Parse(textBox7.Text);
                var b = int.Parse(textBox8.Text);

                if (isSuccessfullyGrammaticKS)
                {
                    var list = CompRec(a, b);
                    int index = 0;
                    DataTable table = new DataTable();
                    table.Columns.Add(new DataColumn());
                    foreach (var chain in list)
                    {
                        DataRow row = table.NewRow();
                        row[0] = chain.Str;
                        table.Rows.Add(row);
                        index++;
                    }
                    dataGridView1.DataSource = table;
                }

                if (!isSuccessfullyGrammaticKS)
                {
                    textBox1.Text += Environment.NewLine + "Ошибка! Сначала введите грамматику!";
                    return;
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
            }
        }

        protected List<Chain> CompRec(int min, int max)
        {
            //Answer = CompRec(mGrammatic.Regulation[0].left, 0);
            var index = Regulation.FindIndex(x => x.left == Start);
            var tmp = Regulation[index];
            Regulation.Remove(tmp);
            Regulation.Insert(0, tmp);

            bool isOne = true;
            List<Chain> list = new List<Chain>();
            do
            {
                foreach (var regulars in Regulation)
                {
                    if (isOne)
                    {
                        foreach (var reg in regulars.right)
                        {
                            list.Add(new Chain());
                            foreach (var r in reg)
                            {
                                if (VN.FindIndex(x => IsRavnStr(x, r)) >= 0)
                                {
                                    list.LastOrDefault().End = r;
                                }
                                else if (IsRavnStr(Lambda, r))
                                {
                                    list.LastOrDefault().End = Lambda;
                                }
                                else
                                {
                                    list.LastOrDefault().Str =
                                        list.LastOrDefault().Str.Replace("\n", "").Replace("\t", "").Replace("r", "") +
                                        r;
                                    list.LastOrDefault().End = Lambda;
                                }
                            }
                        }

                        isOne = false;
                        continue;
                    }
                    //
                    for (var i = 0; i < list.Count; i++)
                    {
                        var listik = list[i];
                        if (!IsRavnStr(listik.End, regulars.left))
                        {
                            continue;
                        }

                        var isOne2 = true;
                        Chain listTmpOne = new Chain();
                        foreach (var reg in regulars.right)
                        {
                            Chain listTmp;
                            if (isOne2)
                            {
                                listTmp = list[i];
                                listTmpOne = (Chain)list[i].Clone();
                            }
                            else
                            {
                                list.Insert(i, new Chain());
                                listTmp = list[i];
                                listTmp.Str = listTmpOne.Str.ToString();
                                listTmp.End = listTmpOne.End.ToString();
                                i++;
                            }
                            foreach (var r in reg)
                            {
                                if (VN.FindIndex(x => IsRavnStr(x, r)) >= 0)
                                {
                                    listTmp.End = r;
                                }
                                else if (IsRavnStr(Lambda, r))
                                {
                                    listTmp.End = Lambda;
                                }
                                else
                                {
                                    listTmp.Str = listTmp.Str.Replace("\n", "").Replace("\t", "").Replace("r", "") + r;
                                    listTmp.End = Lambda;
                                }
                            }

                            isOne2 = false;
                        }
                    }

                }

                for (var i = 0; i < list.Count; i++)
                {
                    var model = list[i];
                    if (model.Count < min && model.End == Lambda)
                    {
                        list.Remove(model);
                        i--;
                        continue;
                    }

                    if (model.Count > max)
                    {
                        list.Remove(model);
                        i--;
                    }
                }
            } while (Check(list));//пока есть не законченные цепочки

            return ChainsDistinct(list);
            return list.Where(x => list.Count(i => x.Str.Equals(i.Str)) <= 1).ToList();
            return list.Distinct().ToList();
        }

        private List<Chain> ChainsDistinct(List<Chain> list)
        {
            var otvet = new List<Chain>();
            foreach (var t in list) 
            {
                if (otvet.Any(x => x.Str.Equals(t.Str)))
                {
                    break;
                }

                otvet.Add(t);
            }

            return otvet;
        }

        private bool IsRavnStr(string str1, string str2)
        {
            if (str1.Replace("\n", "").Replace("\t", "").Replace("r", "") ==
                str2.Replace("\n", "").Replace("\t", "").Replace("r", ""))
            {
                return true;
            }

            return false;
        }
        private bool Check(List<Chain> list)//пока есть не законченные цепочки
        {
            if (list == null || list?.Count == 0)
            {
                return false;
            }

            foreach (var model in list)
            {
                if (!IsRavnStr(model.End, Lambda))
                {
                    return true;
                }
            }

            return false;
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader streamReader = new StreamReader("text.txt"); //Открываем файл для чтения
                string textFromFile = ""; //Объявляем переменную, в которую будем записывать текст из файла
                textFromFile += streamReader.ReadToEnd(); //В переменную str по строчно записываем содержимое файл

                // Terminal
                var index1 = textFromFile.IndexOf("\n");
                var term = textFromFile.Substring(0, index1);
                InputVT(term);
                textBox3.Text = term;

                // NoTerminal
                textFromFile = textFromFile.Substring(index1 + 1);
                index1 = textFromFile.IndexOf("\n");
                var nterm = textFromFile.Substring(0, index1);
                InputNV(nterm);
                textBox4.Text = nterm;

                // Begin NoTerminal
                textFromFile = textFromFile.Substring(index1 + 1);
                index1 = textFromFile.IndexOf("\n");
                var start = textFromFile.Substring(0, index1 - 1);
                InputStartChar(start);
                textBox5.Text = start;

                //lambda
                textFromFile = textFromFile.Substring(index1 + 1);
                index1 = textFromFile.IndexOf("\n");
                var lamb = textFromFile.Substring(0, index1);
                Lambda = lamb.Trim();
                textBox6.Text = Lambda;

                // Regular
                textFromFile = textFromFile.Substring(index1 + 1);
                InputGrammatic(textFromFile);
                textBox2.Text = textFromFile;

                isSuccessfullyGrammaticKS = true;
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
                isSuccessfullyGrammaticKS = false;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}

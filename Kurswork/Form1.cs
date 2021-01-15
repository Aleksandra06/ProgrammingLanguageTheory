using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kurswork.Models;

namespace Kurswork
{
    public partial class Form1 : Form
    {
        private Grammatic KS { get; set; }
        private Grammatic BNF { get; set; }
        private List<Chain> ChainBNF { get; set; }
        private List<Chain> ChainKS { get; set; }

        private bool isSuccessfullyGrammaticKS = false;
        private bool isSuccessfullyGrammaticBNF = false;

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
            label12.Text = "Статус грамматик";
            isSuccessfullyGrammaticBNF = false;
            isSuccessfullyGrammaticKS = false;
            BNF = null;
            textBox9.Text = null;
            textBox10.Text = null;
            textBox12.Text = null;
            textBox11.Text = null;
            dataGridView2.DataSource = null;
            dataGridView1.DataSource = null;
            textBox13.Text = null;
            try
            {
                KS = new Grammatic();
                KS.SetVT(textBox3.Text);
                textBox1.Text += Environment.NewLine + "Алфавит считан успешно";
                KS.SetVN(textBox4.Text);
                textBox1.Text += Environment.NewLine + "Нетерминальный алфавит считан успешно";
                KS.SetStartChar(textBox5.Text);
                textBox1.Text += Environment.NewLine + "Стартовый символ считан успешно";
                KS.SetLambda(textBox6.Text.Trim());
                if (string.IsNullOrEmpty(KS.Lambda))
                {
                    throw new Exception("Введите лямбду");
                }
                KS.SetGrammaticKS(textBox2.Text);
                textBox1.Text += Environment.NewLine + "Грамматика считана успешно";

                isSuccessfullyGrammaticKS = true;
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label12.Text = "Статус грамматик";
            try
            {
                if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
                {
                    throw new Exception("Введите размерность цепочки!");
                }
                var a = int.Parse(textBox7.Text);
                var b = int.Parse(textBox8.Text);

                if (!isSuccessfullyGrammaticKS)
                {
                    textBox1.Text += Environment.NewLine + "Ошибка! Сначала введите грамматику!";
                    return;
                }

                //KS
                var list = CompRec2(a, b, KS);
                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn());
                foreach (var chain in list)
                {
                    DataRow row = table.NewRow();
                    row[0] = chain.Str;
                    table.Rows.Add(row);
                }
                dataGridView1.DataSource = table;
                ChainKS = list;
                textBox1.Text += Environment.NewLine + "КС цепочки построены";

                if (!isSuccessfullyGrammaticBNF)
                {
                    textBox1.Text += Environment.NewLine + "Граматика БНФ отсутствует!";
                    return;
                }

                //БНФ
                var list2 = CompRec2(a, b, BNF);
                DataTable table2 = new DataTable();
                table2.Columns.Add(new DataColumn());
                foreach (var chain in list2)
                {
                    DataRow row = table2.NewRow();
                    row[0] = chain.Str;
                    table2.Rows.Add(row);
                }
                dataGridView2.DataSource = table2;
                ChainBNF = list2;
                textBox1.Text += Environment.NewLine + "БНФ цепочки построены";

            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
            }
        }

        protected List<Chain> CompRec2(int min, int max, Grammatic grammatic)
        {
            var index = grammatic.Regulation.FindIndex(x => x.Left == grammatic.Start);
            var tmp = grammatic.Regulation[index];
            grammatic.Regulation.Remove(tmp);
            grammatic.Regulation.Insert(0, tmp);

            bool isOne = true;
            List<Chain> list = new List<Chain>();
            do
            {
                foreach (var regulars in grammatic.Regulation)
                {
                    if (isOne)
                    {
                        foreach (var reg in regulars.Right)
                        {
                            list.Add(new Chain());
                            var str = "";
                            foreach (var r in reg)
                            {
                                str += r;
                            }
                            list.LastOrDefault().Str = str.Replace("\n", "").Replace("\t", "").Replace("\r", "");
                            list.LastOrDefault().RegularsList = new List<string>() { str };
                        }

                        isOne = false;
                        continue;
                    }
                    //
                    for (var i = 0; i < list.Count; i++)
                    {
                        var listik = list[i];
                        for (int j = 0; j < listik.Str.Length; j++)
                        {
                            if (grammatic.VT.Any(x => x.Equals(listik.Str[j].ToString())))
                            {
                                continue;
                            }

                            if (!listik.Str[j].Equals(regulars.Left[0]))
                            {
                                continue;
                            }

                            var isOne2 = true;
                            Chain listTmpOne = new Chain();
                            string str = "";
                            foreach (var reg in regulars.Right)
                            {
                                str = "";
                                Chain listTmp = list[i];
                                foreach (var ch in reg)
                                {
                                    str += ch;
                                }
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
                                    listTmp.RegularsList = listTmpOne.RegularsList.ToList();
                                    i++;
                                }
                                listTmp.Str = listTmp.Str.Remove(j, 1);
                                listTmp.Str = listTmp.Str.Insert(j, str);
                                listTmp.RegularsList.Add(str);

                                isOne2 = false;
                            }

                            j += str.Length;
                        }
                    }
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var model = list[i];
                    if (model.Count < min && !grammatic.VN.Any(x => x.Equals(model.ToString())))
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

                if (list.Count > 1000)
                {
                    textBox1.Text += "Цепочек более 1000. Выполнение остановленно.";
                    for (var i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        for (var index1 = 0; index1 < item.Str.Length; index1++) 
                        {
                            var ch = item.Str[index1];
                            if (grammatic.VN.Any(x => x.Equals(ch.ToString())))
                            {
                                list.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }

                    break;
                }
            } while (CheckWhile(list, grammatic.Lambda, grammatic.VN));

            list.ForEach(x => x.Str = x.Str.Replace(grammatic.Lambda, ""));
            return ChainsDistinct(list).OrderBy(x => x.Count).ToList();
        }

        private bool CheckWhile(List<Chain> list, string lambda, List<string> vn)
        {
            if (list == null || list?.Count == 0)
            {
                return false;
            }

            foreach (var model in list)
            {
                foreach (var ch in model.Str)
                {
                    if (vn.Any(x => x.Equals(ch.ToString())))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<Chain> ChainsDistinct(List<Chain> list)
        {
            var otvet = new List<Chain>();
            foreach (var t in list)
            {
                if (otvet.Any(x => x.Str.Equals(t.Str)))
                {
                    continue;
                }

                otvet.Add(t);
            }

            return otvet;
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label12.Text = "Статус грамматик";
            isSuccessfullyGrammaticBNF = false;
            isSuccessfullyGrammaticKS = false;
            BNF = null;
            textBox9.Text = null;
            textBox10.Text = null;
            textBox12.Text = null;
            textBox11.Text = null;
            dataGridView2.DataSource = null;
            dataGridView1.DataSource = null;
            textBox13.Text = null;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                string filename = openFileDialog1.FileName;
                string textFromFile = System.IO.File.ReadAllText(filename);

                KS = new Grammatic();
                KS.SetGrammaticByFile(textFromFile);

                textBox3.Text = KS.GetVTString();//Vt
                textBox4.Text = KS.GetVNString();//vn
                textBox5.Text = KS.Start;//start
                textBox6.Text = KS.Lambda;
                if (string.IsNullOrEmpty(KS.Lambda))
                {
                    throw new Exception("Введите лямбду");
                }
                textBox2.Text = KS.ToString();//regular

                textBox1.Text = "Грамматика успешно считана из файла и записана";
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
            label12.Text = "Статус грамматик";
            isSuccessfullyGrammaticBNF = false;
            if (!isSuccessfullyGrammaticKS)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! Запишите сначала КС-грамматику";
            }
            try
            {
                BNF = KS.ConvertToBNFGrammatik();

                textBox9.Text = BNF.ToString();
                textBox10.Text = BNF.GetVTString();
                textBox12.Text = BNF.GetVNString();
                textBox11.Text = BNF.Start;
                isSuccessfullyGrammaticBNF = true;
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + "Ошибка! " + ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var ch in ChainKS)
                {
                    if (!ChainBNF.Any(x => x.Str.Equals(ch.Str)))
                    {
                        textBox1.Text += Environment.NewLine + "БНФ не содержит цепочку " + ch.Str;
                        label12.Text = "Не идентичны";
                        return;
                    }
                }

                foreach (var ch in ChainBNF)
                {
                    if (!ChainKS.Any(x => x.Str.Equals(ch.Str)))
                    {
                        textBox1.Text += Environment.NewLine + "КС не содержит цепочку " + ch.Str;
                        label12.Text = "Не идентичны";
                        return;
                    }
                }

                textBox1.Text += Environment.NewLine + "Цепочки грамматик идентичны";
                label12.Text = "Идентичны";
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label12.Text = "Статус грамматик";
                var rowIndex = e.RowIndex;
                var value = dataGridView2[0, rowIndex].Value.ToString();
                if (rowIndex == ChainBNF.Count)
                {
                    ChainBNF.Add(new Chain() {Str = value});
                    BildDataGridBNF();
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    ChainBNF.RemoveAt(rowIndex);
                    BildDataGridBNF();
                    return;
                }


                ChainBNF[rowIndex].Str = dataGridView2[0, rowIndex].Value.ToString();
            }
            catch (Exception ex)
            {
                //textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label12.Text = "Статус грамматик";
                var rowIndex = e.RowIndex;
                var value = dataGridView1[0, rowIndex].Value.ToString();
                if (rowIndex == ChainKS.Count)
                {
                    ChainKS.Add(new Chain() {Str = value});
                    BildDataGridKS();
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    ChainKS.RemoveAt(rowIndex);
                    BildDataGridKS();
                    return;
                }


                ChainKS[rowIndex].Str = dataGridView1[0, rowIndex].Value.ToString();
                ChainKS[rowIndex].RegularsList = null;
            }
            catch (Exception ex)
            {
               // textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void BildDataGridKS()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn());
                foreach (var chain in ChainKS)
                {
                    DataRow row = table.NewRow();
                    row[0] = chain.Str;
                    table.Rows.Add(row);
                }

                dataGridView1.DataSource = table;

                textBox1.Text += Environment.NewLine + "Таблица КС цепочек изменена";
            }
            catch (Exception ex)
            {
                //textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void BildDataGridBNF()
        {
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn());
                foreach (var chain in ChainBNF)
                {
                    DataRow row = table.NewRow();
                    row[0] = chain.Str;
                    table.Rows.Add(row);
                }

                dataGridView2.DataSource = table;

                textBox1.Text += Environment.NewLine + "Таблица БНВ цепочек изменена";
            }
            catch (Exception ex)
            {
                //textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var rowIndex = e.RowIndex;
                if (ChainKS[rowIndex].RegularsList == null)
                {
                    if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
                    {
                        throw new Exception("Введите размерность цепочки!");
                    }

                    var a = int.Parse(textBox7.Text);
                    var b = int.Parse(textBox8.Text);

                    var list = CompRec2(a, b, KS);
                    ChainKS[rowIndex].RegularsList = list.Where(x => x.Str.Equals(ChainKS[rowIndex].Str))
                        .FirstOrDefault()?.RegularsList;
                }

                if (string.IsNullOrEmpty(ChainKS[rowIndex].GetRegularsListString()))
                {
                    textBox13.Text = "По заданным параметрам невозможно создать данную цепочку";
                }
                else
                {
                    textBox13.Text = "Цепочка " + ChainKS[rowIndex].Str + " грамматики KC строится: " +
                                     ChainKS[rowIndex].GetRegularsListString();
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var rowIndex = e.RowIndex;
                if (ChainBNF[rowIndex].RegularsList == null)
                {
                    if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
                    {
                        throw new Exception("Введите размерность цепочки!");
                    }

                    var a = int.Parse(textBox7.Text);
                    var b = int.Parse(textBox8.Text);

                    var list = CompRec2(a, b, KS);
                    ChainBNF[rowIndex].RegularsList = list.Where(x => x.Str.Equals(ChainBNF[rowIndex].Str))
                        .FirstOrDefault()?.RegularsList;
                }

                if (string.IsNullOrEmpty(ChainBNF[rowIndex].GetRegularsListString()))
                {
                    textBox13.Text = "По заданным параметрам невозможно создать данную цепочку";
                }
                else
                {
                    textBox13.Text = "Цепочка " + ChainBNF[rowIndex].Str + " грамматики БНФ строится: " +
                                     ChainBNF[rowIndex].GetRegularsListString();
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void скачатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                // получаем выбранный файл
                string filename = saveFileDialog1.FileName;

                var str = "";
                if (KS != null)
                {
                    str += "КС грамматика: " + Environment.NewLine;
                    str += "Алфавит: " + KS.GetVTString() + Environment.NewLine;
                    str += "Алфавит нетерминальный: " + KS.GetVNString() + Environment.NewLine;
                    str += "Лямбда: " + KS.Lambda + Environment.NewLine;
                    str += "Стартовый символ: " + KS.Start + Environment.NewLine;
                    str += "Грамматика: " + KS.ToString() + Environment.NewLine;
                    if (ChainKS != null)
                    {
                        str += "Цепочки: " + Environment.NewLine;
                        foreach (var ch in ChainKS)
                        {
                            str += ch.Str + Environment.NewLine;
                        }
                    }
                }

                if (BNF != null)
                {
                    str += "БНФ грамматика: " + Environment.NewLine;
                    str += "Алфавит: " + BNF.GetVTString() + Environment.NewLine;
                    str += "Алфавит нетерминальный: " + BNF.GetVNString() + Environment.NewLine;
                    str += "Лямбда: " + BNF.Lambda + Environment.NewLine;
                    str += "Стартовый символ: " + BNF.Start + Environment.NewLine;
                    str += "Грамматика: " + BNF.ToString() + Environment.NewLine;
                    if (ChainBNF != null)
                    {
                        str += "Цепочки: " + Environment.NewLine;
                        foreach (var ch in ChainBNF)
                        {
                            str += ch.Str + Environment.NewLine;
                        }
                    }
                }

                // сохраняем текст в файл
                System.IO.File.WriteAllText(filename, str);
                MessageBox.Show("Файл сохранен");
            }
            catch (Exception ex)
            {
                textBox1.Text += Environment.NewLine + ex.Message;
            }
        }

        private void форматФайлаДляЗагрузкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = "";
            str += "Алфавит терминальных символов через запятую без пробела" + Environment.NewLine;
            str += "Алфавит нетерминальных символов через запятую без пробела" + Environment.NewLine;
            str += "Начальный символ" + Environment.NewLine;
            str += "Лямбда" + Environment.NewLine;
            str += "Правила каждое с новой строки в формате: A->aB|aBa|bC" + Environment.NewLine;
            str += "Каждая буква алфавита не может быть более одного символа! Лямбда не может быть более одного символа! Начальный символ должен быть нетерминальным!" + Environment.NewLine;
            MessageBox.Show(str, "Формат файла для загрузки:");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}

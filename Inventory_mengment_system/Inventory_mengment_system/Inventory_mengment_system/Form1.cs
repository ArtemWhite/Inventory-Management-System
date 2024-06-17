using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Inventory_mengment_system
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        string ConStr;
        SqlCommand cmd;
        List<string> Item = new List<string>();
        List<string> Good = new List<string>();
        List<string> Red = new List<string>();
        List<string> CompOrd = new List<string>();
        List<string> NowOrder = new List<string>();
        int t = 0;
        public Form1()
        {
            InitializeComponent();
            btnConnect_Click(null, null);
            conn = new SqlConnection(ConStr);
            cmd = new SqlCommand("", conn);
            //btnConnect_Click(null, null);
            try
            {
                AddItems();
                OrderItems();
                loadOrders();
                CompOrders();
                SoldDownload();
               
            }
            catch (Exception)
            {

            }
        }
        void loadOrders()
        {
            conn.Open();
            NowOrder.Clear();
            cmd.CommandText = $"SELECT [NameOfOrder] FROM Orders";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
                cBoxNowOrders.Items.Add(sdr[0].ToString());
            foreach (var item in cBoxNowOrders.Items)
            {
                NowOrder.Add(Convert.ToString(item));
            }
            conn.Close();
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConStr = $"Data Source={tBoxAdress.Text}; Initial Catalog={tBoxDB.Text}; User ID={tBoxUser.Text}; Password={tBoxPass.Text}";
            //btnDisConnect_Click(sender, e);
        }
        private void btnDisConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ConStr = $"Data Source={tBoxAdress.Text}; Initial Catalog=master; User ID={tBoxUser.Text}; Password={tBoxPass.Text}";
                SqlConnection conn = new SqlConnection(ConStr);
                SqlCommand cmd = new SqlCommand($"SELECT [name] FROM sys.databases WHERE [name]='{tBoxDB.Text}'", conn);
                conn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                bool tableExist = sdr.Read();
                conn.Close();
                if (tableExist)
                {
                    btnDisConnect.Text = "✔Готово";
                    //btnConnect.Enabled = true;
                }
                else
                    if (MessageBox.Show($"Базы данных {tBoxDB.Text} нет, хотите создать?", "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    conn.Open();
                    cmd.CommandText = $"CREATE DATABASE {tBoxDB.Text}";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"USE {tBoxDB.Text}\r\n" +
                        $"CREATE TABLE Goods" +
                        $"(\r\n" +
                        $"Id INT PRIMARY KEY IDENTITY," +
                        $"[NameOfGood] NVARCHAR(MAX) NOT NULL," +
                        $"[Category] NVARCHAR(25) NOT NULL," +
                        $"[Sum] INT NOT NULL," +
                        $"[Min] INT NOT NULL," +
                        $"[Now] INT NOT NULL," +
                        $"[Max] INT NOT NULL" +
                        $")\r\n" +
                        $"CREATE TABLE SoldGoods" +
                        $"(\r\n" +
                        $"Id INT PRIMARY KEY IDENTITY," +
                        $"[NameOfGood] NVARCHAR (MAX) NOT NULL," +
                        $"[Category] NVARCHAR(25) NOT NULL," +
                        $"[Now] INT NOT NULL," +
                        $"[Sum] INT NOT NULL" +
                        $")\r\n" +
                        $"CREATE TABLE Orders" +
                        $"(\r\n" +
                        $"Id INT PRIMARY KEY IDENTITY," +
                        $"[NameOfOrder] NVARCHAR(MAX) UNIQUE NOT NULL," +
                        $"[Name] NVARCHAR (20) NOT NULL," +
                        $"[SurName] NVARCHAR (20) NOT NULL," +
                        $"Cont NVARCHAR(25) NOT NULL," +
                        $"[Sum] INT NOT NULL," +
                        $"Stat INT NOT NULL," +
                        $"[Goods] NVARCHAR NOT NULL," +
                        $"Goode NVARCHAR(MAX) NOT NULL" +
                        $")\r\n" +
                        $"CREATE TABLE CompOreders" +
                        $"(\r\n" +
                        $"Id INT PRIMARY KEY IDENTITY," +
                        $"[NameOfOrder] NVARCHAR(MAX) NOT NULL," +
                        $"[Name] NVARCHAR (20) NOT NULL," +
                        $"[SurName] NVARCHAR (20) NOT NULL," +
                        $"Cont NVARCHAR(25) NOT NULL," +
                        $"[Sum] INT NOT NULL," +
                        $"Stat INT NOT NULL," +
                        $"Goode NVARCHAR(MAX) NOT NULL" +
                        $")\r\n";
                    cmd.ExecuteNonQuery();

                    btnDisConnect.Text = "✔Готово";
                    //btnConnect.Enabled = true;
                    //conn.Close();
                }
            }
            catch (Exception)
            {

                btnDisConnect.Text = "❌Ошибка";
            }
            finally
            {
                conn.Close();
            }
        }
        private void btnAddGoods_Click(object sender, EventArgs e)
        {
            if (tBoxAddGoods.Text != "" && tBoxAddMin.Text != "" && tBoxAddMax.Text != "" && tBoxAddThigs.Text != "" && tBoxAddSum.Text != "")
            {
                if (Convert.ToInt32(tBoxAddThigs.Text) <= Convert.ToInt32(tBoxAddMax.Text) && Convert.ToInt32(tBoxAddMin.Text) < Convert.ToInt32(tBoxAddMax.Text))
                {

                    conn.Open();
                    cmd.CommandText = $"SELECT [NameOfGood],[Now] FROM Goods";
                    SqlDataReader sdre = cmd.ExecuteReader();
                    bool found = false;
                    int thing = 0;
                    while (sdre.Read())
                    {
                        String good = sdre[0].ToString();
                        if (tBoxAddGoods.Text == good)
                        {
                            found = true;
                            thing = Convert.ToInt32(sdre[1]);
                            break;
                        }
                    }
                    sdre.Close();
                    if (found)
                    {
                        cmd.CommandText = $"UPDATE Goods SET [Category] = '{cBoxAddGoods.SelectedItem}', [Sum] = '{tBoxAddSum.Text}', [Min] = '{tBoxAddMin.Text}', [Now] = '{Convert.ToInt32(tBoxAddThigs.Text) + thing}', [Max] = '{tBoxAddMax.Text}' WHERE [NameOfGood] = '{tBoxAddGoods.Text}'";

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = $"INSERT INTO Goods VALUES('{tBoxAddGoods.Text}','{cBoxAddGoods.SelectedItem}'," +
                            $"'{tBoxAddSum.Text}','{tBoxAddMin.Text}','{tBoxAddThigs.Text}','{tBoxAddMax.Text}')";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                    lBoxAddItem.Items.Clear();
                    conn.Open();
                    cmd.CommandText = $"SELECT [NameOfGood] FROM Goods";
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                        lBoxAddItem.Items.Add(sdr[0].ToString());
                    conn.Close();
                    AddItems();
                    OrderItems();



                }

            }
            else
            {
                MessageBox.Show("Не все поля заполнены!!!");
            }
        }
        void MakeGood ()
        {
            lBoxAddItem.Items.Clear();
            conn.Open();
            Red.Clear();
            cmd.CommandText = $"SELECT [NameOfGood] FROM Goods";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                lBoxAddItem.Items.Add(sdr[0].ToString());
            }
            foreach (string item in lBoxAddItem.Items)
            {
                Red.Add(item);
            }
            conn.Close();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            lBoxAddItem.Items.Clear();
            if (checkBoxAddItem.Checked)
            {
                lBoxAddItem.Visible = true;
                MakeGood();
            }
            else
                lBoxAddItem.Visible = false;
            

        }
        private void lBoxAddItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood],[Category],[Sum],[Min],[Now],[Max] FROM Goods WHERE '{lBoxAddItem.SelectedItem}' = [NameOfGood]";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                tBoxAddGoods.Text = sdr[0].ToString();
                cBoxAddGoods.Text = sdr[1].ToString();
                tBoxAddSum.Text = sdr[2].ToString();
                tBoxAddMin.Text = sdr[3].ToString();
                tBoxAddThigs.Text = sdr[4].ToString();
                tBoxAddMax.Text = sdr[5].ToString();
            }
            conn.Close();
        }
        private void cBoxRedaction_CheckedChanged(object sender, EventArgs e)
        {
            if (cBoxRedaction.Checked)
            {
                tBoxAdress.Enabled = true;
                tBoxDB.Enabled = true;
                tBoxUser.Enabled = true;
                tBoxPass.Enabled = true;
                btnConnect.Enabled = true;
            }
            else
            {
                tBoxAdress.Enabled = false;
                tBoxDB.Enabled = false;
                tBoxUser.Enabled = false;
                tBoxPass.Enabled = false;
                btnConnect.Enabled = false;
            }
        }
        void AddItems()
        {
            lBoxGoods.Items.Clear();
            conn.Open();
            Item.Clear();
            cmd.CommandText = $"SELECT [NameOfGood] FROM Goods";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                lBoxGoods.Items.Add(sdr[0].ToString());
                
            }
            foreach (string item in lBoxGoods.Items)
            {
                Item.Add(item);
            }
            conn.Close();
        }
        private void lBoxGoods_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood],[Category],[Sum],[Min],[Now],[Max] FROM Goods WHERE '{lBoxGoods.SelectedItem}' = [NameOfGood]";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                tBoxSerGoods.Text = sdr[0].ToString();
                cBoxGoods.Text = sdr[1].ToString();
                tBoxSumGoods.Text = Convert.ToString(Convert.ToInt32(sdr[2]) * Convert.ToInt32(sdr[4]));
                tBoxMinLevel.Text = sdr[3].ToString();
                tBoxCurLevel.Text = sdr[4].ToString();
                tBoxMaxLevel.Text = sdr[5].ToString();
                tBoxOneGood.Text = sdr[2].ToString();
                if (Convert.ToInt32(sdr[3].ToString()) >= Convert.ToInt32(sdr[4].ToString()))
                {
                    min.Visible = true;
                    min.ForeColor = Color.Red;
                    min.Text = "Необходимо пополнить товар!";
                }
                else
                {
                    min.Visible = false;
                }
            }
            conn.Close();
        }
        void OrderItems()
        {
            listBox1.Items.Clear();
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood], [Now] FROM Goods";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                listBox1.Items.Add(sdr[0].ToString());
            }
            conn.Close();

        }
        private void cBoxKorzina_CheckedChanged_1(object sender, EventArgs e)
        {

            if (cBoxKorzina.Checked)
            {
                lBoxKorzina.Visible = true;
                lBoxKcol.Visible = true;
            }
            else
            {
                lBoxKorzina.Visible = false;
                lBoxKcol.Visible = false;
            }

        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood],[Now],[Sum] FROM Goods WHERE '{listBox1.SelectedItem}' = [NameOfGood]";
            SqlDataReader sdr = cmd.ExecuteReader();

            while (sdr.Read())
            {

                if (Convert.ToInt32(tBoxColItems.Text) > Convert.ToInt32(sdr[1].ToString()) && listBox1.SelectedItem.ToString() == sdr[0].ToString())
                {
                    MessageBox.Show("Запрос превышает количество имеющихся вещей на складе", "Ошибка");
                    break;
                }
                if (listBox1.SelectedItem != null && lBoxKorzina.Items.Contains(listBox1.SelectedItem))
                {
                    break;
                }
                if (listBox1.SelectedItem != null)
                {
                    lBoxKorzina.Items.Add(listBox1.SelectedItem);
                    lBoxKcol.Items.Add(Convert.ToInt32(tBoxColItems.Text));
                }
                if (listBox1.SelectedItem.ToString() == sdr[0].ToString())
                {
                    t = t + Convert.ToInt32(tBoxColItems.Text) * Convert.ToInt32(sdr[2].ToString());
                    tBoxSum.Text = t.ToString();
                }
            }

            conn.Close();

        }
        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood],[Now],[Category] FROM Goods";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                if (Convert.ToString(listBox1.SelectedItem) == sdr[0].ToString())
                {
                    lNumber.Text = sdr[1].ToString();
                    comboBox1.Text = sdr[2].ToString();
                }

            }
            conn.Close();
        }
        private void lBoxKorzina_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"SELECT [Sum] FROM Goods WHERE '{lBoxKorzina.SelectedItem}' = [NameOfGood]";
            SqlDataReader sdr = cmd.ExecuteReader();
            int selectedIndex = lBoxKorzina.SelectedIndex;
            if (lBoxKorzina.SelectedItem != null)
            {
                object itemFromLBoxCol = lBoxKcol.Items[selectedIndex];

                while (sdr.Read())
                {
                    if (lBoxKorzina.SelectedItems != null)
                    {
                        t = t - Convert.ToInt32(itemFromLBoxCol) * Convert.ToInt32(sdr[0].ToString());
                        tBoxSum.Text = t.ToString();
                        lBoxKcol.Items.RemoveAt(lBoxKorzina.SelectedIndex);
                        lBoxKorzina.Items.RemoveAt(lBoxKorzina.SelectedIndex);
                    }
                    if (lBoxKorzina.Items.Count == 0)
                    {
                        t = 0;
                        tBoxSum.Text = t.ToString();
                    }
                }
            }
            conn.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (tBoxNameOfOrder.Text != "" && tBoxName.Text != "" && tBoxSurName.Text != "" && tBoxCont.Text != "" && lBoxKorzina.Items.Count > 0)
            {
                List<string> name = new List<string>();
                List<int> col = new List<int>();

                for (int j = 0; j < lBoxKorzina.Items.Count; j++)
                {
                    name.Add(lBoxKorzina.Items[j].ToString());
                    col.Add(Convert.ToInt32(lBoxKcol.Items[j]));
                }

                string namesAndCols = string.Join("|", name.Zip(col, (n, c) => $"{n},{c}"));
                //// В этой строке мы объединяем элементы из списков `name` и `col` 
                //// в одну строку, разделяя их символом `|`.
                //try
                //{
                cBoxNowOrders.Items.Clear();
                conn.Open();
                cmd.CommandText = $"INSERT INTO Orders VALUES('{tBoxNameOfOrder.Text}','{tBoxName.Text}','{tBoxSurName.Text}'," +
                                  $"'{tBoxCont.Text}','{tBoxSum.Text}',2,'{namesAndCols}')";
                cmd.ExecuteNonQuery();
                conn.Close();
                object lisboxKorzina;
                object liboxCol;
                conn.Open();
                for (int i = 0; i < lBoxKorzina.Items.Count; i++)
                {
                    lisboxKorzina = lBoxKorzina.Items[i];
                    liboxCol = lBoxKcol.Items[i];
                    cmd.CommandText = $"UPDATE Goods SET [Now] = [Now] - '{Convert.ToInt32(liboxCol)}' " +
                        $"WHERE [NameOfGood] = '{Convert.ToString(lisboxKorzina)}'";
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                conn.Open();
                cmd.CommandText = $"SELECT [NameOfOrder] FROM Orders";
                SqlDataReader sql = cmd.ExecuteReader();
                while (sql.Read())
                    cBoxNowOrders.Items.Add(sql.GetString(0));
                lBoxKorzina.Items.Clear();
                lBoxKcol.Items.Clear();
                conn.Close();
            }
            else
                MessageBox.Show("Не все поля заполнены!!!");
            //}
            //catch(Exception)
            //{
            //    MessageBox.Show("Не все поля заполненны!");
            //}

        }
        void SoldDownload()
        {
            conn.Open();
            Good.Clear();
            cmd.CommandText = $"SELECT [NameOfGood] FROM SoldGoods";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                lBoxSoldGoods.Items.Add(sdr[0].ToString());
            }
            conn.Close();
            foreach (string item in lBoxSoldGoods.Items)
            {
                Good.Add(item);
            }
        }
        void CompOrders()
        {
            conn.Open();
            CompOrd.Clear();
            cmd.CommandText = $"SELECT [NameOfOrder] FROM CompOreders";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                cBoxSerCompOrd.Items.Add(sdr.GetString(0));
            }
            foreach (var item in cBoxSerCompOrd.Items)
            {
                CompOrd.Add(Convert.ToString(item));
            }
            conn.Close();
        }
        private void cBoxNowOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            lBoxCurOrder.Items.Clear();
            try
            {
                conn.Open();
                cmd.CommandText = $"SELECT [NameOfOrder],[Name],[SurName],[Cont],[Sum],[Goode] " +
                    $"FROM Orders WHERE '{cBoxNowOrders.SelectedItem}' = [NameOfOrder]";
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    tBoxNameCur.Text = sdr[1].ToString();
                    tBoxSurNameCur.Text = sdr[2].ToString();
                    tBoxContCur.Text = sdr[3].ToString();
                    tBoxSumCur.Text = sdr[4].ToString();
                    string value = sdr.GetString(5);
                    string[] values = value.Split('|');
                    foreach (string val in values)
                    {
                        lBoxCurOrder.Items.Add(val.Replace("|", "\n"));
                    }
                }
                conn.Close();
            }
            catch (Exception) { }
        }
        private void btnCompOrder_Click(object sender, EventArgs e)
        {
            //lBoxCompOrders.Items.Clear();
            conn.Open();
            lBoxSoldGoods.Items.Clear();
            cmd.CommandText = $"INSERT INTO CompOreders ([NameOfOrder], [Name], " +
                $"[SurName], [Cont], [Sum], [Stat], [Goode]) VALUES ('{cBoxNowOrders.SelectedItem}', " +
                $"'{tBoxNameCur.Text}', '{tBoxSurNameCur.Text}', '{tBoxContCur.Text}', '{tBoxSumCur.Text}', 1, " +
                $"'{string.Join(",", lBoxCurOrder.Items.Cast<string>())}')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"DELETE FROM Orders WHERE [NameOfOrder]='{cBoxNowOrders.SelectedItem}'";
            cmd.ExecuteNonQuery();
            cBoxSerCompOrd.Items.Add(cBoxNowOrders.SelectedItem);
            cBoxNowOrders.Items.Remove(cBoxNowOrders.SelectedItem);
            cBoxNowOrders.Items.Clear();
            cBoxNowOrders.Text = "";
            tBoxSumCur.Text = "";
            tBoxNameCur.Text = "";
            tBoxSurNameCur.Text = "";
            tBoxContCur.Text = "";

            conn.Close();

            conn.Open();

            for (int i = 0; i < lBoxCurOrder.Items.Count; i++)
            {
                string item = lBoxCurOrder.Items[i].ToString();
                string[] values = item.Split(',');
                string name = values[0].Trim();
                int count = int.Parse(values[1].Trim());

                cmd.CommandText = $"SELECT [NameOfGood],[Now],[Sum],[Category] FROM SoldGoods";
                SqlDataReader sdr = cmd.ExecuteReader();
                bool found = false;
                int thing = 0;
                while (sdr.Read())
                {
                    String good = sdr[0].ToString();
                    if (name == good)
                    {
                        found = true;
                        thing = Convert.ToInt32(sdr[1]);
                        break;
                    }
                }
                sdr.Close();

                if (found)
                {
                    cmd.CommandText = $"UPDATE SoldGoods SET [NameOfGood] = '{name}', [Category] = [Category], [Now] = '{thing + count}', [Sum] = [Sum] WHERE [NameOfGood] = '{name}'";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = $"SELECT [Category], [Sum] FROM Goods WHERE [NameOfGood] = '{name}'";
                    sdr = cmd.ExecuteReader();
                    sdr.Read();
                    string category = sdr[0].ToString();
                    int sum = int.Parse(sdr[1].ToString());
                    sdr.Close();

                    cmd.CommandText = $"INSERT INTO SoldGoods VALUES ('{name}', '{category}', {count}, {sum})";
                    cmd.ExecuteNonQuery();
                }

            }
            cBoxSerCompOrd.Items.Clear();
            cmd.CommandText = $"SELECT [NameOfGood] FROM SoldGoods";
            SqlDataReader sdrT = cmd.ExecuteReader();
            while (sdrT.Read())
                lBoxSoldGoods.Items.Add(sdrT[0].ToString());

            conn.Close();
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfOrder] FROM [CompOreders]";
            SqlDataReader sdf = cmd.ExecuteReader();
            while (sdf.Read())
            {
                cBoxSerCompOrd.Items.Add(sdf.GetString(0));
            }
            conn.Close();
            lBoxCurOrder.Items.Clear();

        }
        private void lBoxSoldGoods_MouseClick(object sender, MouseEventArgs e)
        {

            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood],[Category],[Now],[Sum] FROM SoldGoods WHERE [NameOfGood] = '{lBoxSoldGoods.SelectedItem}'";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                cBoxDelGoods.Text = sdr[1].ToString();
                textBox1.Text = sdr[2].ToString();
                tBoxSumSoldGoods.Text = Convert.ToString(Convert.ToInt32(textBox1.Text) * Convert.ToInt32(sdr[3].ToString()));
            }
            conn.Close();
        }
        private void tBoxAddMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void tBoxAddMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void tBoxAddThigs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void tBoxAddSum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void tBoxColItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void cBoxSerCompOrd_SelectedIndexChanged(object sender, EventArgs e)
        {
            lBoxCompOrders.Items.Clear();
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfOrder],[Name],[SurName], [Cont],[Sum],[Goode] FROM [CompOreders] " +
                $"WHERE [NameOfOrder] = '{cBoxSerCompOrd.SelectedItem}'";
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                lBoxCompOrders.Items.Clear();
                tBoxSumComp.Text = sdr[4].ToString();
                tBoxNameComp.Text = sdr[1].ToString();
                tBoxSurnameComp.Text = sdr[2].ToString();
                tBoxContComp.Text = sdr[3].ToString();

                string itemsStr = sdr.GetString(5);
                if (!string.IsNullOrEmpty(itemsStr))
                {
                    string[] items = itemsStr.Split(',');
                    for (int i = 0; i < items.Length; i += 2)
                    {
                        string name = items[i].Trim();
                        int count = int.Parse(items[i + 1].Trim());
                        lBoxCompOrders.Items.Add($"{name}, {count}" + (i < items.Length - 2 ? "," : ""));
                    }
                }
            }
            sdr.Close();
            conn.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"DELETE FROM Goods WHERE [NameOfGood] = '{lBoxAddItem.SelectedItem}'";
            cmd.ExecuteNonQuery();
            conn.Close();
            lBoxAddItem.Items.Remove(lBoxAddItem.SelectedItem);
            lBoxGoods.Items.Remove(lBoxAddItem.SelectedItem);
        }
        private void btnDelSoldGoods_Click(object sender, EventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"DELETE FROM [SoldGoods] WHERE [NameOfGood] = '{lBoxSoldGoods.SelectedItem}'";
            cmd.ExecuteNonQuery();
            conn.Close();
            lBoxSoldGoods.Items.Remove(lBoxSoldGoods.SelectedItem);
        }
        private void btnDel_Click(object sender, EventArgs e)
        {
            conn.Open();
            cmd.CommandText = $"DELETE FROM [CompOreders] WHERE [NameOfOrder] = '{cBoxSerCompOrd.SelectedItem}'";
            cmd.ExecuteNonQuery();
            conn.Close();
            lBoxCompOrders.Items.Clear();
            cBoxSerCompOrd.Items.Remove(cBoxSerCompOrd.SelectedItem);
        }
        private void cBoxComp_CheckedChanged(object sender, EventArgs e)
        {
            if (cBoxComp.Checked)
            {
                cBoxSerCompOrd.Items.Clear();
                conn.Open();
                cmd.CommandText = $"SELECT [NameOfOrder] FROM [CompOreders] WHERE [Stat] = 1";
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    cBoxSerCompOrd.Items.Add(sdr.GetString(0));
                }
                conn.Close();
                cBoxNotComplete.Enabled = false;
                checkBox1.Enabled = false;
            }
            else
            {
                cBoxNotComplete.Enabled = true;
                checkBox1.Enabled = true;
            }


        }
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                cBoxSerCompOrd.Items.Clear();
                conn.Open();
                cmd.CommandText = $"SELECT [NameOfOrder] FROM [CompOreders]";
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    cBoxSerCompOrd.Items.Add(sdr.GetString(0));
                }
                conn.Close();
                cBoxNotComplete.Enabled = false;
                cBoxComp.Enabled = false;
            }
            else
            {
                cBoxNotComplete.Enabled = true;
                cBoxComp.Enabled = true;
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(tBoxAddMax.Text) >= Convert.ToInt32(tBoxAddThigs.Text))
            {
                conn.Open();
                cmd.CommandText = $"UPDATE Goods SET [Sum] = '{tBoxAddSum.Text}', " +
                    $"[Max] ='{tBoxAddMax.Text}',[Min] = '{tBoxAddMin.Text}',[Now] = '{tBoxAddThigs.Text}',[Category] = '{Convert.ToString(cBoxAddGoods.SelectedItem)}' " +
                    $"WHERE [NameOfGood] = '{tBoxAddGoods.Text}'";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            else
                MessageBox.Show("Склад будет переполнен!");
        }
        private void btnDelCurOrder_Click(object sender, EventArgs e)
        {
            conn.Open();
            lBoxSoldGoods.Items.Clear();
            cmd.CommandText = $"INSERT INTO CompOreders ([NameOfOrder], [Name], " +
                $"[SurName], [Cont], [Sum], [Stat], [Goode]) VALUES ('{cBoxNowOrders.SelectedItem}', " +
                $"'{tBoxNameCur.Text}', '{tBoxSurNameCur.Text}', '{tBoxContCur.Text}', '{tBoxSumCur.Text}', 0, " +
                $"'{string.Join(",", lBoxCurOrder.Items.Cast<string>())}')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"DELETE FROM Orders WHERE [NameOfOrder]='{cBoxNowOrders.SelectedItem}'";
            cmd.ExecuteNonQuery();
            cBoxSerCompOrd.Items.Add(cBoxNowOrders.SelectedItem);
            cBoxNowOrders.Items.Remove(cBoxNowOrders.SelectedItem);
            cBoxNowOrders.Items.Clear();
            cBoxNowOrders.Text = "";
            tBoxSumCur.Text = "";
            tBoxNameCur.Text = "";
            tBoxSurNameCur.Text = "";
            tBoxContCur.Text = "";

            conn.Close();

            conn.Open();

            for (int i = 0; i < lBoxCurOrder.Items.Count; i++)
            {
                string item = lBoxCurOrder.Items[i].ToString();
                string[] values = item.Split(',');
                string name = values[0].Trim();
                int count = int.Parse(values[1].Trim());

                cmd.CommandText = $"SELECT [NameOfGood],[Now],[Sum],[Category] FROM Goods";
                SqlDataReader sdr = cmd.ExecuteReader();
                bool found = false;
                int thing = 0;
                while (sdr.Read())
                {
                    String good = sdr[0].ToString();
                    if (name == good)
                    {
                        found = true;
                        thing = Convert.ToInt32(sdr[1]);
                        break;
                    }
                }
                sdr.Close();

                if (found)
                {
                    cmd.CommandText = $"UPDATE Goods SET [NameOfGood] = '{name}', [Category] = [Category], [Now] = '{thing + count}', [Sum] = [Sum] WHERE [NameOfGood] = '{name}'";
                    cmd.ExecuteNonQuery();
                }

            }
            lBoxAddItem.Items.Clear();
            lBoxGoods.Items.Clear();
            conn.Close();
            conn.Open();
            cmd.CommandText = $"SELECT [NameOfGood] FROM [Goods]";
            SqlDataReader sdf = cmd.ExecuteReader();
            while (sdf.Read())
            {
                lBoxAddItem.Items.Add(sdf.GetString(0));
                lBoxGoods.Items.Add(sdf.GetString(0));
            }
            conn.Close();
            lBoxCurOrder.Items.Clear();
        }
        private void cBoxNotComplete_CheckedChanged(object sender, EventArgs e)
        {
            if (cBoxNotComplete.Checked)
            {
                cBoxSerCompOrd.Items.Clear();
                conn.Open();
                cmd.CommandText = $"SELECT [NameOfOrder] FROM [CompOreders] WHERE [Stat] = 0";
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    cBoxSerCompOrd.Items.Add(sdr.GetString(0));
                }
                conn.Close();
                checkBox1.Enabled = false;
                cBoxComp.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
                cBoxComp.Enabled = true;
            }
        }
        private void tBoxSerGoods_TextChanged(object sender, EventArgs e)
        {
            lBoxGoods.BeginUpdate();
            lBoxGoods.Items.Clear();
            string filter = tBoxSerGoods.Text;
            if (Item != null)
            {
                lBoxGoods.Items.Clear();
                var filteredItems = Item.Where(item => item.Contains(filter)).ToArray();
                lBoxGoods.Items.AddRange(filteredItems);
            }
            lBoxGoods.EndUpdate();
        }
        private void tBoxSerSoldGoods_TextChanged(object sender, EventArgs e)
        {
            lBoxSoldGoods.BeginUpdate();
            lBoxSoldGoods.Items.Clear();
            string filter = tBoxSerSoldGoods.Text;
            if (Good != null)
            {
                lBoxSoldGoods.Items.Clear();
                var filteredItems = Good.Where(item => item.Contains(filter)).ToArray();
                lBoxSoldGoods.Items.AddRange(filteredItems);
            }
            lBoxSoldGoods.EndUpdate();
        }
        private void tBoxAddGoods_TextChanged(object sender, EventArgs e)
        {
            lBoxAddItem.BeginUpdate();
            lBoxAddItem.Items.Clear();
            string filter = tBoxAddGoods.Text;
            if (Red != null)
            {
                lBoxAddItem.Items.Clear();
                var filteredItems = Red.Where(item => item.Contains(filter)).ToArray();
                lBoxAddItem.Items.AddRange(filteredItems);
            }
            lBoxAddItem.EndUpdate();
        }

        private void cBoxSerCompOrd_TextChanged(object sender, EventArgs e)
        {
            cBoxSerCompOrd.BeginUpdate(); //Система поиска, работает не корректо
            string filter = cBoxSerCompOrd.Text;
            cBoxSerCompOrd.Items.Clear();
            foreach (var item in CompOrd)
            {
                if (item.Contains(filter))
                {
                    cBoxSerCompOrd.Items.Add(item);
                }
            }
            cBoxSerCompOrd.DroppedDown = true;
            cBoxSerCompOrd.Select(cBoxSerCompOrd.Text.Length, 0);
            cBoxSerCompOrd.EndUpdate();
        }

        private void cBoxNowOrders_TextChanged(object sender, EventArgs e)
        {
            cBoxNowOrders.BeginUpdate(); // Система поиска работает не корректо
            string filter = cBoxNowOrders.Text;
            cBoxNowOrders.Items.Clear();
            foreach (var item in NowOrder)
            {
                if (item.Contains(filter))
                {
                    cBoxNowOrders.Items.Add(item);
                }
            }
            cBoxNowOrders.DroppedDown = true;
            cBoxNowOrders.Select(cBoxNowOrders.Text.Length, 0);
            cBoxNowOrders.EndUpdate();
            cBoxNowOrders.Text = Convert.ToString(cBoxNowOrders.SelectedItem);
        }

    }
}

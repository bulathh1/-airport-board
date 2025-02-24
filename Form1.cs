using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.Common;

namespace kysrovaya
{
    public partial class Form1 : Form
    {
        private DataSet ds = new DataSet();
        public Form1()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            if (File.Exists(@"C:\kyrsovoyfile\aeroflot.txt"))
            {
                try
                {
                    using (StreamReader rd = new StreamReader(@"C:\kyrsovoyfile\aeroflot.txt"))
                    {
                        // Проверяем, существует ли таблица с таким именем
                        if (!ds.Tables.Contains("Aeroflotinfo"))
                        {
                            ds.Tables.Add("Aeroflotinfo");
                            string header = rd.ReadLine();
                            string[] headers = header.Split(',');
                            foreach (string column in headers)
                            {
                                // Добавляем столбцы
                                ds.Tables["Aeroflotinfo"].Columns.Add(column);
                            }
                        }
                        else
                        {
                            // Если таблица уже существует, очищаем её перед загрузкой новых данных
                            ds.Tables["Aeroflotinfo"].Clear();
                            // Пропускаем строку заголовков, так как столбцы уже добавлены
                            rd.ReadLine();
                        }

                        string row;
                        while ((row = rd.ReadLine()) != null)
                        {
                            string[] rvalue = row.Split(',');
                            ds.Tables["Aeroflotinfo"].Rows.Add(rvalue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при чтении файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл данных не найден.");
            }
        }

        private void LoadDataButton_Click(object sender, EventArgs e)
        {
            LoadData();
            dataGridView1.DataSource = ds.Tables["Aeroflotinfo"];
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SearchFlight(string searchQuery, string columnName)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                MessageBox.Show("Пожалуйста, введите данные для поиска.");
                return;
            }

            var foundRows = ds.Tables["Aeroflotinfo"].AsEnumerable()
                .Where(row => row[columnName].ToString().Trim().Equals(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (foundRows.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var row in foundRows)
                {
                    sb.AppendLine($"Пункт назначения: {row["Destination"]}, Номер рейса: {row["FlightNumber"]}, Тип самолета: {row["AircraftType"]}");
                }
                MessageBox.Show(sb.ToString());
            }
            else
            {
                MessageBox.Show("Рейсы не найдены.");
            }
        }

        public void SortData(string columnName, bool ascending = true)
        {
            DataTable table = ds.Tables["Aeroflotinfo"];
            for (int i = 1; i < table.Rows.Count; i++)
            {
                DataRow currentRow = table.NewRow();
                currentRow.ItemArray = table.Rows[i].ItemArray;
                int j = i - 1;

                while (j >= 0 && CompareRows(table.Rows[j], currentRow, columnName, ascending))
                {
                    table.Rows[j + 1].ItemArray = table.Rows[j].ItemArray;
                    j--;
                }
                table.Rows[j + 1].ItemArray = currentRow.ItemArray;
            }
        }

        public bool CompareRows(DataRow row1, DataRow row2, string columnName, bool ascending)
        {

            int comparison = string.Compare(row1[columnName].ToString(), row2[columnName].ToString());
            return ascending ? comparison > 0 : comparison < 0;
        }
        private void SortButton_Click(object sender, EventArgs e)
        {
            
            if (comboBoxSort.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите критерий сортировки.");
                return;
            }
            string columnName = comboBoxSort.SelectedItem.ToString();
            bool ascending = radioButtonAscending.Checked;
            SortData(columnName, ascending);
            dataGridView1.DataSource = ds.Tables["Aeroflotinfo"];
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {

            // Проверяем, есть ли у DataGridView привязанный источник данных
            if (dataGridView1.DataSource != null)
            {
                // Очищаем таблицу данных, которая является источником данных для DataGridView
                DataTable dt = ds.Tables["Aeroflotinfo"];
                dt.Clear();
            }
            else
            {
                // Если источник данных не задан, то очищаем строки напрямую
                dataGridView1.Rows.Clear();
            }
        }
        // Обработчик события для кнопки добавления данных
        private void addButton_Click_1(object sender, EventArgs e)
        {
            // Создаем новый объект Flight с данными из TextBox
            Flight newFlight = new Flight(textBoxDestination.Text, textBoxFlightNumber.Text, textBoxAircraftType.Text);
            // Проверяем, существует ли таблица, и если нет, создаем ее
            if (!ds.Tables.Contains("Aeroflotinfo"))
            {
                DataTable table = new DataTable("Aeroflotinfo");
                table.Columns.Add("Destination");
                table.Columns.Add("FlightNumber");
                table.Columns.Add("AircraftType");
                ds.Tables.Add(table);
            }

            // Добавляем новый объект Flight в DataTable
            DataRow newRow = ds.Tables["Aeroflotinfo"].NewRow();
            newRow["Destination"] = newFlight.Destination;
            newRow["FlightNumber"] = newFlight.FlightNumber;
            newRow["AircraftType"] = newFlight.AircraftType;
            ds.Tables["Aeroflotinfo"].Rows.Add(newRow);

            // Обновляем источник данных для DataGridView
            dataGridView1.DataSource = ds.Tables["Aeroflotinfo"];
        }

        private void SearchNumButton_Click(object sender, EventArgs e)
        {
            string searchQuery = searchTextBox.Text.Trim();
            SearchFlight(searchQuery, "FlightNumber");
        }

        private void SearchDestButton_Click(object sender, EventArgs e)
        {
            string searchQuery = searchTextBox2.Text.Trim();
            SearchFlight(searchQuery, "Destination");
        }
    }
}
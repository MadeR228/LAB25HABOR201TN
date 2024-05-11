using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public class DataAddedEventArgs : EventArgs
        {
            public string Code {  get; }
            public string SportName { get; }
            public string SportView { get; }

            public DataAddedEventArgs(string code, string sportName, string sportView)
            {
                Code = code;
                SportName = sportName;
                SportView = sportView;
            }
        }

        public event EventHandler <DataAddedEventArgs> DataAdded;

        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем данные из текстовых полей
            string sportName = textBox1.Text;
            string sportView = textBox2.Text;

            // Проверяем, что поля не пустые
            if (string.IsNullOrEmpty(sportName) || string.IsNullOrEmpty(sportView))
            {
                MessageBox.Show("Всі поля повинні бути заповнені");
                return;
            }

            // Генерируем код из базы данных
            string code = GenerateCodeFromDatabase();

            // Добавляем данные в базу данных
            AddDataToDatabase(code, sportName, sportView);

            // Вызываем событие DataAdded, передавая данные
            DataAdded?.Invoke(this, new DataAddedEventArgs(code, sportName, sportView));

            // Закрываем форму
            this.Close();
        }

        // Метод для генерации кода из базы данных
        private string GenerateCodeFromDatabase()
        {
            string code = ""; // Инициализируем код

            string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=sport.accdb";
            using (OleDbConnection dbConnection = new OleDbConnection(connectionString))
            {
                try
                {
                    dbConnection.Open();
                    string query = "SELECT MAX(Code) FROM register"; // Запрос для получения максимального кода
                    OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
                    object result = dbCommand.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        int maxCode = Convert.ToInt32(result);
                        code = (maxCode + 1).ToString(); // Увеличиваем максимальный код на 1
                    }
                    else
                    {
                        code = "1"; // Если база данных пуста, начинаем с кода 1
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }

            return code;
        }

        // Метод для добавления данных в базу данных
        private void AddDataToDatabase(string code, string sportName, string sportView)
        {
            string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=sport.accdb";
            using (OleDbConnection dbConnection = new OleDbConnection(connectionString))
            {
                try
                {
                    dbConnection.Open();
                    string query = "INSERT INTO register (Code, sportName, sportView) VALUES (@Code, @sportName, @sportView)";
                    OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
                    dbCommand.Parameters.AddWithValue("@Code", code);
                    dbCommand.Parameters.AddWithValue("@sportName", sportName);
                    dbCommand.Parameters.AddWithValue("@sportView", sportView);
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при додаванні даних в базу даних: " + ex.Message);
                }
            }
        }
    }
}

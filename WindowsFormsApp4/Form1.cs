using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static WindowsFormsApp4.Form2;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=sport.accdb";
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            string query = "SELECT * FROM register";
            OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
            OleDbDataReader dbReader = dbCommand.ExecuteReader();

            if (dbReader.HasRows == false)
            {
                MessageBox.Show("");
            }
            else
            {
                while (dbReader.Read())
                {
                    dataGridView1.Rows.Add(dbReader["Code"], dbReader["sportName"], dbReader["sportView"]);
                }
            }
            dbReader.Close();
            dbConnection.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchValue = tbSearch.Text.Trim(); 

            if (string.IsNullOrEmpty(searchValue))
            {
                MessageBox.Show("Введіть значення для пошуку");
                return;
            }

            string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=sport.accdb";
            using (OleDbConnection dbConnection = new OleDbConnection(connectionString))
            {
                try
                {
                    dbConnection.Open();
                    string query = "SELECT * FROM register WHERE sportView LIKE @searchValue";
                    OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
                    dbCommand.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%"); 

                    using (OleDbDataReader dbReader = dbCommand.ExecuteReader())
                    {
                        dataGridView1.Rows.Clear(); 
                        if (dbReader.HasRows)
                        {
                            while (dbReader.Read())
                            {
                                dataGridView1.Rows.Add(dbReader["Code"], dbReader["sportName"], dbReader["sportView"]);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Нічого не знайдено");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка: " + ex.Message);
                }
            }
        }

        private void tbSearch_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.DataAdded += Form2_DataAdded;

            form2.ShowDialog();
        }

        private void Form2_DataAdded(object sender, DataAddedEventArgs e)
        {
            dataGridView1.Rows.Add(e.Code, e.SportName, e.SportView);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string codeToDelete = dataGridView1.SelectedRows[0].Cells["Code"].Value.ToString();

                DeleteDataFromDatabase(codeToDelete);

                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
        }

        private void DeleteDataFromDatabase(string code)
        {
            string connectionString = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=sport.accdb";
            using (OleDbConnection dbConnection = new OleDbConnection(connectionString))
            {
                try
                {
                    dbConnection.Open();
                    string query = "DELETE FROM register WHERE Code = @Code";
                    OleDbCommand dbCommand = new OleDbCommand(query, dbConnection);
                    dbCommand.Parameters.AddWithValue("@Code", code);
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при видаленні даних з бази даних: " + ex.Message);
                }
            }
        }
    }
}

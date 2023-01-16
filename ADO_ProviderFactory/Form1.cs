using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Configuration;

namespace ADO_ProviderFactory
{
    public partial class Form1 : Form
    {
        DbConnection conn=null;
        DbProviderFactory fact=null;
        string providerName="";

        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        /// <summary>
        /// Читаем список зарегистрированных поставщиков данных
        /// возвращаем список в виде таблицы
        /// отображаем список в dataGridView1
        /// значения из колонки InvariantName созданной таблицы
        /// заносим в comboBox1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable t = DbProviderFactories.GetFactoryClasses();
            dataGridView1.DataSource = t;

            comboBox1.Items.Clear();
            
            foreach (DataRow dr in t.Rows)
            {
                comboBox1.Items.Add(dr["InvariantName"]);
            }
        }
        /// <summary>
        /// по выбранному в comboBox1 значению с помощью метода GetFactory()
        /// создаем фабрику для вібранного поставщика
        /// с помощью метода GetConnectionStringByProvider()
        /// получаем из App.config строку подключения к БД
        /// отображаем строку подключения в textBox1 
        /// и сохраняем в глобальной строке providerName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fact = DbProviderFactories.GetFactory(comboBox1.SelectedItem.ToString());
            conn = fact.CreateConnection();
            providerName = GetConnectionStringByProvider(comboBox1.SelectedItem.ToString());
            textBox1.Text = providerName;
            
        }

        /// <summary>
        /// читаем из App.config строку подключения с значением providerName,
        /// совпадающим с параметром providerName
        /// возвращаем найденную строку подключения или null
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        static string GetConnectionStringByProvider(string providerName)
        {
            string returnValue = null;

            // читаем все строки подключения из App.config
            ConnectionStringSettingsCollection settings =
                ConfigurationManager.ConnectionStrings;

            // ищем и возвращаем строку подключения для providerName
            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.ProviderName == providerName)
                    {
                        returnValue = cs.ConnectionString;
                        break;
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// имея в свом распоряжении фабрику для выбранного поставщика
        /// выполняем стандартные действия с БД
        /// для демонстрации работоспособности кода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            conn.ConnectionString = textBox1.Text;

            // создаем адаптер из фабрики
            DbDataAdapter adapter = fact.CreateDataAdapter();
            adapter.SelectCommand = conn.CreateCommand();
            adapter.SelectCommand.CommandText = textBox2.Text.ToString();
            // выполняем запрос select из адаптера
            DataTable table = new DataTable();
            adapter.Fill(table);
            // выводим результаты запроса
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = table;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 5)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeneratorDataForBase
{
    public partial class AuthorizationForm : Form
    {
        public static SqlConnection connection;
        public static SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();

        public AuthorizationForm()
        {
            InitializeComponent();
           

            sqlCSB.DataSource = @"DESKTOP-8VRV8GQ\SQLEXPRESS";
            sqlCSB.InitialCatalog = "ModellerBase";
            sqlCSB.Pooling = true;
            //sqlCSB.UserID = "dataBaseUser";
            //sqlCSB.Password = "1234";



        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            try
            {
                sqlCSB.UserID = login.Text;
                sqlCSB.Password = password.Text;
                connection = new SqlConnection(sqlCSB.ConnectionString);
                connection.Open();
                MessageBox.Show("Успешно!!!");
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль!!!");
            }

            GeneratorData generator = new GeneratorData();
            generator.Show();
            this.Hide();
        }

        public static SqlConnection GetSqlConnection() {
            return connection;
        }

        public static void CloseSqlConnection()
        {
            connection.Close();
        }
    }
}

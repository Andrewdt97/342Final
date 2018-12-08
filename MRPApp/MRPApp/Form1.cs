using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRPApp
{
    public partial class Form1 : Form
    {
        List<KeyValuePair<int, String>> partsList;

        public Form1()
        {
            InitializeComponent();
            String connectionString = "Data Source=localhost;" +
                "Initial Catalog=MRP;Integrated Security=true ";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("sp_queryParts", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();

            this.partsList = new List<KeyValuePair<int, String>>();

            while (reader.Read())
            {
                partsList.Add(new KeyValuePair<int, String>(int.Parse(reader["partID"].ToString()), reader["partName"].ToString()));
            }
            conn.Close();
            filterProductList();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void filterTB_TextChanged(object sender, EventArgs e)
        {
            filterProductList();
        }

        private void filterProductList()
        {
            productList.BeginUpdate();
            productList.Items.Clear();

            if (!string.IsNullOrEmpty(filterTB.Text))
            {
                foreach (KeyValuePair<int, String> kvp in partsList)
                {
                    if (kvp.Value.Contains(filterTB.Text))
                    {
                        productList.Items.Add(kvp.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, String> kvp in partsList)
                {
                    productList.Items.Add(kvp.Value);
                }
            }

            productList.EndUpdate();
        }
    }
}

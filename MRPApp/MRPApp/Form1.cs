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
        List<String> selectedList;
        bool pendingAdd;
        SqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            String connectionString = "Data Source=localhost;" +
                "Initial Catalog=MRP;Integrated Security=true ";
            conn = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("sp_queryParts", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();

            this.partsList = new List<KeyValuePair<int, String>>();

            while (reader.Read())
            {
                partsList.Add(new KeyValuePair<int, String>(int.Parse(reader["partID"].ToString()), reader["partName"].ToString().ToLower()));
            }
            conn.Close();

            selectedList = new List<String>();

            filterProductList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            errorLabel.Text = string.Empty;
            bool success = checkValidEntries();

            if (success)
            {
                insertVendor();
            }
            else
            {
                errorLabel.ForeColor = Color.Red;
                errorLabel.Text = "Please make sure all fields are populated and at least one product is selected";
            }
        }

        private bool checkValidEntries()
        {
            return !(
                string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) ||
                string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(comboBox1.Text) ||
                selectedList.Count == 0
             );
        }

        private void insertVendor()
        {
            insertVendorInfo();

            insertVendorProducts();

            errorLabel.ForeColor = Color.Green;
            errorLabel.Text = "Successfully added vendor!";

            cleanUp();
            
        }

        private void insertVendorInfo()
        {
            SqlCommand command = new SqlCommand("sp_AddVendor", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(DbCreateVCharInputParam(
                "name", 40, textBox1.Text));
            command.Parameters.Add(DbCreateVCharInputParam(
                "contactName", 40, textBox2.Text));
            command.Parameters.Add(DbCreateVCharInputParam(
                "phoneNum", 40, textBox3.Text));
            command.Parameters.Add(DbCreateVCharInputParam(
                "paymentAddress", 200, textBox4.Text));
            command.Parameters.Add(DbCreateVCharInputParam(
                "rating", 2, comboBox1.Text));

            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();
        }

        private void insertVendorProducts()
        {
            int vendorId;
            SqlCommand command = new SqlCommand("sp_GetVendorId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(DbCreateVCharInputParam("name", 40, textBox1.Text));
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            vendorId = int.Parse(reader["VendorId"].ToString());
            conn.Close();

            for (int i = 0; i < selectedList.Count; i++)
            {
                int partId = partsList.Find(kvp => kvp.Value == selectedList[i]).Key;

                command = new SqlCommand("sp_AddVendorPart", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(DbCreateIntInputParam("vendorId", vendorId));
                command.Parameters.Add(DbCreateIntInputParam("partId", partId));
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        private SqlParameter DbCreateVCharInputParam(string param_name,
                        int p_size, string in_value)
        {
            SqlParameter rParam = new SqlParameter();
            rParam.ParameterName = param_name;
            rParam.SqlDbType = SqlDbType.VarChar;
            rParam.Size = p_size;
            rParam.Direction = ParameterDirection.Input;
            rParam.Value = in_value;

            return rParam;
        }

        private SqlParameter DbCreateIntInputParam(string param_name, int in_value)
        {
            SqlParameter rParam = new SqlParameter();
            rParam.ParameterName = param_name;
            rParam.SqlDbType = SqlDbType.Int;
            rParam.Direction = ParameterDirection.Input;
            rParam.Value = in_value;

            return rParam;
        }

        private void cleanUp()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            comboBox1.Text = string.Empty;

            productList.SelectedIndices.Clear();

            selectedList.Clear();
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
                    if (kvp.Value.Contains(filterTB.Text.ToLower()))
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

            for (int i = 0; i < selectedList.Count; i++)
            {
                if (productList.Items.Contains(selectedList[i]))
                {
                    pendingAdd = true;
                    productList.SelectedIndices.Add(productList.Items.IndexOf(selectedList[i]));
                    pendingAdd = false;
                }
            }

            productList.EndUpdate();
        }

        private void productList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool inSelectedList, inSelectedItems;
            for (int i = 0; i < productList.Items.Count; i ++)
            {
                inSelectedList = selectedList.IndexOf(productList.Items[i].ToString()) != -1;
                inSelectedItems = productList.SelectedItems.IndexOf(productList.Items[i].ToString()) != -1;

                if (inSelectedItems && !inSelectedList)
                {
                    selectedList.Add(productList.Items[i].ToString());
                } else if (!inSelectedItems && inSelectedList && !pendingAdd)
                {
                    selectedList.Remove(productList.Items[i].ToString());
                }
            }
        }
    }
}

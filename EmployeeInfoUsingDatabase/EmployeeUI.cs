using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeInfoUsingDatabase
{
    public partial class EmployeeUI : Form
    {
        public EmployeeUI()
        {
            InitializeComponent();
            deleteButton.Visible = false;
        }
        
        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        
        private bool isNotUnique = false;
        private bool updateMode = false;
        private int employeeID;

        private void saveButton_Click(object sender, EventArgs e)
        {

            Employee anEmployee = new Employee();
            anEmployee.name = nameTextBox.Text;
            anEmployee.address = addressTextBox.Text;
            anEmployee.email = emailTextBox.Text;
            anEmployee.salary = float.Parse(salaryTextBox.Text);


            isNotUnique = CheckEmail(anEmployee.email);

            if (isNotUnique)
            {
                MessageBox.Show("Email Already Exists.");
            }
            else
            {
                if (updateMode)
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = "UPDATE Employee_Info SET Name='" + anEmployee.name + "',Address='" +
                                   anEmployee.address + "',Email='"+anEmployee.email+"',Salary='" + anEmployee.salary + "' WHERE EmployeeID='" + employeeID + "' ";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowseffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowseffected > 0)
                    {
                        MessageBox.Show("Data Updated Succssfully");
                        GetListViewData();
                    }
                }
                else
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = "INSERT INTO Employee_Info VALUES('" + anEmployee.name + "','" +
                                   anEmployee.address +
                                   "','" +
                                   anEmployee.email + "','" + anEmployee.salary + "')";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Succesfully saved.");
                        GetListViewData(); 
                    }
                     
                }
                nameTextBox.Clear();
                addressTextBox.Clear();
                emailTextBox.Clear();
                salaryTextBox.Clear();
            }
        }

        public bool CheckEmail(string email)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT Email FROM Employee_Info WHERE Email = '"+email+"'";

            SqlCommand command = new SqlCommand(query, connection);

            bool unique = false;
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                unique = true;
            }
            return unique;
        }

        private void EmployeeUI_Load(object sender, EventArgs e)
        {
           
            GetListViewData();
        }

        private void GetListViewData()
        {
            employeeListView.Items.Clear();
            SqlConnection connection = new SqlConnection(connectionString);

            List<Employee> employees = new List<Employee>();
            
            string query = "SELECT * FROM Employee_Info";

            SqlCommand command = new SqlCommand(query, connection);


            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Employee anEmployee = new Employee();

                anEmployee.id = Convert.ToInt32(reader["EmployeeID"].ToString());
                anEmployee.name = reader["Name"].ToString();
                anEmployee.address = reader["Address"].ToString();
                anEmployee.email = reader["Email"].ToString();
                anEmployee.salary = float.Parse(reader["Salary"].ToString());

                employees.Add(anEmployee);
            }
            connection.Close();

            foreach (var employee in employees)
            {
                ListViewItem item = new ListViewItem();

                item.Text = employee.id.ToString();
                item.SubItems.Add(employee.name);
                item.SubItems.Add(employee.address);
                item.SubItems.Add(employee.email);
                item.SubItems.Add(employee.salary.ToString());
                employeeListView.Items.Add(item);
            }
        }
        

        private void employeeListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            employeeID = int.Parse(employeeListView.SelectedItems[0].Text);

            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT Name,Address,Email,Salary FROM Employee_Info WHERE EmployeeID='" + employeeID + "'";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                nameTextBox.Text = reader["Name"].ToString();
                addressTextBox.Text = reader["Address"].ToString();
                emailTextBox.Text = reader["Email"].ToString();
                salaryTextBox.Text = reader["Salary"].ToString();


            }

            connection.Close();

            updateMode = true;
            saveButton.Text = "Update";
            deleteButton.Visible = true;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you want delete this employee?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "DELETE FROM Employee_Info WHERE employeeID='" + employeeID + "'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                int rowsEffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsEffected > 0)
                {
                    MessageBox.Show("Deleted Succesfully.");
                    GetListViewData();
                }
            }
        }
       
    }
}

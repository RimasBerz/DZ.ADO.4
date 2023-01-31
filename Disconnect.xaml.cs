using ADO1.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for Disconnect.xaml
    /// </summary>
    public partial class Disconnect : Window
    {
        public ObservableCollection<Entities.Department> Departments { get; set; }

        public ObservableCollection<Entities.Product> Products { get; set; }

        public ObservableCollection<Entities.Managers> Managers { get; set; }
        public Disconnect()
        {
            InitializeComponent();
            DataContext = this;
            using SqlConnection connection = new(App.ConnectinString);
            try
            {
                connection.Open();
                #region Departments
                Departments = new();
                using SqlCommand cmd = new("SELECT Id,Name FROM Departments", connection);
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Departments.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                #endregion
                #region Products
                Products = new();
                using SqlCommand cmd2 = new("SELECT Id,Name,Price FROM Products", connection);
                {
                    using var reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        Products.Add(new()
                        {
                            Id = reader2.GetGuid(0),
                            Name = reader2.GetString(1),
                            Price = reader2.GetDouble(2)
                        });
                    }
                }
                #endregion
                #region Managers
                Managers = new();
                using SqlCommand cmd3 = new("SELECT Id,Name,Surname,Secname FROM Managers", connection);
                {
                    using var reader3 = cmd3.ExecuteReader();
                    while (reader3.Read())
                    {
                        Managers.Add(new()
                        {
                            Id = reader3.GetGuid(0),
                            Name = reader3.GetString(1),
                            Surname = reader3.GetString(2),
                            Secname = reader3.GetString(3)
                        });
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void ListVievItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Department department)
                {
                    MessageBox.Show(department.ToShortString());
                }
            }

        }
        private void ListVievItem_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Product product)
                {
                    MessageBox.Show(product.ToShortString());
                }
            }
        }
        private void ListVievItem_MouseDoubleClick_2(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Managers manager)
                {
                    MessageBox.Show(manager.ToShortString());
                }
            }
        }
    }
}

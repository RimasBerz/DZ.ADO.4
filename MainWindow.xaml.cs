using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Data;
using ADO1.Entities;

namespace ADO1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection _connection;
        private List<Entities.Department>? _departments;
        private List<Entities.Product>? _products;
        private List<Entities.Managers>? _managers;
        public MainWindow()
        {
            InitializeComponent();
            //String connectinString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin\source\repos\ADO1\ADO1\Database1.mdf;Integrated Security=True";
            _connection = new(App.ConnectinString);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                MonitorConnection.Content = "Установлено";
                MonitorConnection.Foreground = Brushes.Green;
            }
            catch (SqlException ex)
            {
                MonitorConnection.Content = "Закрыто";
                MonitorConnection.Foreground = Brushes.Red;
                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowManagersCount();
            ShowProductsCount();
            ShowSalesCount();
            ShowDeparmentsOrm();
            ShowDailyStatistics();
            //ShowDailyStatistics2();
            ShowProductsOrm();
            ShowmanagersOrm();
            ShowNameOrmD();
        }
        #region Show Monitor

        /// <summary>
        /// Выводит в таблицу-монитор количество отделов (департаментов) из БД
        /// </summary>
        private void ShowDepartmentsCount()
        {
            String sql = "SELECT COUNT(*) FROM Departments";
            // SqlCommand объект для передачи команд (запросов) к БД.
            // Требует закрытия, поэтому using
            using var cmd = new SqlCommand(sql, _connection);
            // создание объекта не исполняет команду, для этого есть методы ExecuteXxxx
            MonitorDepartments.Content =
                Convert.ToString(
                    cmd.ExecuteScalar()   // выполняет команду и возвращает "верхний-левый" результат
                );
        }

        /// <summary>
        /// Выводит в таблицу-монитор количество Товаров из БД
        /// </summary>
        private void ShowProductsCount()
        {
            String sql = "SELECT COUNT(*) FROM Products";
            using var cmd = new SqlCommand(sql, _connection);
            MonitorProducts.Content = Convert.ToString(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Выводит в таблицу-монитор количество Сотрудников (Менеджеров) из БД
        /// </summary>
        private void ShowManagersCount()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Managers", _connection);
            MonitorManagers.Content = Convert.ToString(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Выводит в таблицу-монитор количество Продаж (чеков) из БД
        /// </summary>
        private void ShowSalesCount()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Sales", _connection);
            MonitorSales.Content = Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion


        /// <summary>
        /// Заполняет блок "Статистика за день"
        /// </summary>
        private void ShowDailyStatistics()
        {
            SqlCommand cmd = new()
            {
                Connection = _connection
            };
            // В БД информация за 2022 год, поэтому формируем дату с текущим днем и месяцем, но за 2022 год
            String date = $"2022-{DateTime.Now.Month}-{DateTime.Now.Day}";

            // Всего продаж (чеков)
            cmd.CommandText = $"SELECT COUNT(*) FROM Sales S WHERE CAST( S.Moment AS DATE ) = '{date}'";
            StatTotalSales.Content = Convert.ToString(cmd.ExecuteScalar());

            // Всего продаж (товаров, штук)
            cmd.CommandText = $"SELECT SUM(S.Cnt) FROM Sales S WHERE CAST( S.Moment AS DATE ) = '{date}'";
            StatTotalProducts.Content = Convert.ToString(cmd.ExecuteScalar());

            // Всего продаж (грн, деньги)
            cmd.CommandText = $"SELECT ROUND( SUM( S.Cnt * P.Price ), 2 ) FROM Sales S JOIN Products P ON S.Id_product = P.Id WHERE CAST( S.Moment AS DATE ) = '{date}'";
            StatTotalMoney.Content = Convert.ToString(cmd.ExecuteScalar());
            // File.ReadAllText("")

            cmd.Dispose();
        }
        //private void ShowDailyStatistics2()
        //{
        //    SqlCommand cmd = new()
        //    {
        //        Connection = _connection
        //    };
        //    String date = $"2022-{DateTime.Now.Month}-{DateTime.Now.Day}";

        //    cmd.CommandText = $"SELECT ROUND( COUNT( S.Cnt), 2 ),M.Name,M.Surname ((FROM Sales S JOIN Products P ON S.Id_product = P.Id)JOIN Managers M ON S.Id_Manager = M.Id) WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalManagereD.Content = Convert.ToString(cmd.ExecuteScalar());

        //    // Не очень разобрался с самим запросом,понял,что надо подключить все таблици,по скольку за продажи отвечают менеджеры,которые привязаны к департаменту
        //    cmd.CommandText = $"SELECT ROUND ( SUM( S.Cnt * P.Price ), 2 ) (((FROM Sales S JOIN Products P ON S.Id_product = P.Id) JOIN Managers M ON S.Id_manager = M.Id) JOIN Departments D ON M.Id_main_dep = D.Id) WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalDepartmentD.Content = Convert.ToString(cmd.ExecuteScalar());

        //    cmd.CommandText = $"SELECT ROUND( COUNT( S.Cnt ), 2 ),S.Cnt FROM Sales S WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalProductsD.Content = Convert.ToString(cmd.ExecuteScalar());

        //    cmd.Dispose();
        //}

        private void ShowDepartments()
        {
            using SqlCommand cmd = new("SELECT * FROM Departments", _connection);
            SqlDataReader reader = cmd.ExecuteReader();
            DepartmentCell.Text = "";
            while (reader.Read())
            {
                Guid id = reader.GetGuid("id");
                String name = (String)reader[1];
                DepartmentCell.Text += $"{id} {name} \n";
            }
            reader.Dispose();
        }
        private void ShowDeparmentsOrm()
        {
            if (_departments is null)
            {
                using SqlCommand cmd = new("SELECT D.Id,D.Name FROM DEPARTMENTS D", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _departments = new();
                    while (reader.Read())
                    {
                        _departments.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                DepartmentCell.Text = "";
                foreach (var depertment in _departments)
                {
                    DepartmentCell.Text += DepartmentCell.Name;
                }
            }
            DepartmentCell.Text = "";
            foreach (var department in _departments)
            {
                DepartmentCell.Text += department.ToShortString() + "\n";
            }
        }

        private void ShowProductsOrm()
        {
            if (_products is null)
            {
                using SqlCommand cmd = new("SELECT P.Id,P.Name,P.Price FROM Products P", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _products = new();
                    while (reader.Read())
                    {
                        _products.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                DepartmentCell.Text = "";
                foreach (var department in _products)
                {
                    DepartmentCell.Text += DepartmentCell.Name;
                }
            }
            ProductsCell.Text = "";
            foreach (var product in _products)
            {
                ProductsCell.Text += product.ToShortString() + "\n";
            }
        }

        private void ShowmanagersOrm()
        {
            if (_managers is null)
            {
                using SqlCommand cmd = new("SELECT M.Id,M.Surname,M.Name,M.Secname FROM Managers M", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _managers = new();
                    while (reader.Read())
                    {
                        _managers.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                ManagersCell.Text = "";
                foreach (var managers in _managers)
                {
                    ManagersCell.Text += ManagersCell.Name;
                }
            }
            ManagersCell.Text = "";
            foreach (var product in _managers)
            {
                ManagersCell.Text += product.ToShortString() + "\n";
            }
        }

        private void ShowNameOrmD()
        {
            if (_products is null)
            {
                using SqlCommand cmd = new("SELECT S.Id,S.Cnt,S.Cnt,ROUND( SUM( S.Cnt * P.Price ), 2 ) FROM Sales S JOIN Products P ON S.Id_product = P.Id WHERE CAST( S.Moment AS DATE ) = '{date}'", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _products = new();
                    while (reader.Read())
                    {
                        _products.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                ProdName.Text = "";
                foreach (var department in _products)
                {
                    ProdName.Text += ProdName.Name;
                }
            }
            ProdName.Text = "";
            foreach (var department in _products)
            {
                ProdName.Text += department.ToShortString() + "\n";
            }
        }

        private void Window_Closed(object sender,EventArgs e)
        {
           if(_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
    

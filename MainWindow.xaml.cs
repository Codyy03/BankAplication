using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
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

namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
       
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LoadData();
        }

        private void backToRegistracion_Click(object sender, RoutedEventArgs e)
        {
            Rejestracion rejestracionWiondw = new Rejestracion();
            MainWindow loginWindow = new MainWindow();
          
            this.Close();
            rejestracionWiondw.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            rejestracionWiondw.Closed += (s, args) => Application.Current.Shutdown();
            rejestracionWiondw.Show();

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; // Ignorowanie wprowadzonego tekstu, jeśli nie jest literą
            }
        }

        private void LoadData()
        {
            DatabaseManager databaseManager = new DatabaseManager(connectionString);
            string query = "SELECT imie FROM pracownicy"; // Zmień na swoje zapytanie
                                                          //  DataTable data = databaseManager.ExecuteQuery(query);
            label.Content = databaseManager.ExecuteQueryToStringList(query)[0];




        }
    }
}

using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            // ustaw okno na środku ekranu
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
        }

        private void backToRegistracion_Click(object sender, RoutedEventArgs e)
        {
            Rejestracion rejestracionWiondw = new Rejestracion();
            MainWindow loginWindow = new MainWindow();

            this.Close();
            rejestracionWiondw.WindowStartupLocation = WindowStartupLocation.CenterScreen;

          //  rejestracionWiondw.Closed += (s, args) => Application.Current.Shutdown();
            rejestracionWiondw.Show();

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; // Ignorowanie wprowadzonego tekstu, jeśli nie jest literą
            }
        }

        private void LoginAttempt()
        {
      
            DatabaseManager databaseManager = new DatabaseManager(connectionString);

            string login = "SELECT login FROM klienci Where login = " + "'"+loginField.Text+"'";
            string password = "SELECT haslo FROM klienci Where haslo = " + "'" + PasswordField.Password + "'";
            // jezeli wartości wpisane przez użytkownika są w bazie danych przejdz do głównego okna aplikacji
            if (databaseManager.CheckIfValueExistInDataBase(login) && databaseManager.CheckIfValueExistInDataBase(password))
                label.Content = "Tak";
            else label.Content = "ne";



        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // próba logowania
            LoginAttempt();
        }

    }
}

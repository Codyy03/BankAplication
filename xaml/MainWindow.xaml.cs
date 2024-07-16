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
    ///  ta klasa reprezentuje wszystkie funkcjolaności sceny 'logowanie'
    public partial class MainWindow : Window
    {
        private string currentUserName="";
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public MainWindow()
        {
            InitializeComponent();
            // ustaw okno na środku ekranu
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           
        }

        // zmiena scene na rejestracje
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
            string login = "SELECT login FROM klienci Where login = " + "'"+loginField.Text+"' AND haslo = " + "'" + PasswordField.Password + "'";
           
            // jezeli wartości wpisane przez użytkownika są w bazie danych przejdz do głównego okna aplikacji
            if (databaseManager.CheckIfValueExistInDataBase(login))
            {
                currentUserName = databaseManager.ReturnSingleQueryValue<string>("SELECT imie FROM klienci Where login = " + "'" + loginField.Text + "'");
                Bank bank = new Bank(currentUserName, loginField.Text);
                this.Close();
                bank.Show();
            }
            else LoginError.Text = "Login lub hasło są nieprawidłowe.";



        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // próba logowania
            LoginAttempt();
        }

        public string GetCurrentUserName()
        {
            return currentUserName;
        }
    }
}

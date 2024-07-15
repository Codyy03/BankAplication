using System;
using System.Collections.Generic;
using System.Configuration;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy Rejestracion.xaml
    /// </summary>
    public partial class Rejestracion : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Rejestracion()
        {
            InitializeComponent();
        }

        private void RegistracionName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; // Ignorowanie wprowadzonego tekstu, jeśli nie jest literą
            }
        }

     
        private void backToLogin_Click(object sender, RoutedEventArgs e)
        {
            ChangeSceneToLogin();
        }

        private void RejestracionAttempt_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager databaseManager = new DatabaseManager(connectionString);
            string login = "SELECT login FROM klienci Where login = " + "'" + RegistracionUserName.Text + "'";


            // zabezpieczenia przed niepoprawnymi danymi wprowadzonymi przez użytkownika.
            if (StringIsNull(RegistracionName.Text))
            {
                RegistracionError.Text = "Imie nie może być puste";
                return;
            }

            if (StringIsNull(RegistracionLastName.Text))
            {
                RegistracionError.Text = "Nazwisko nie może być puste";
                return;
            }

            if (StringIsNull(RegistracionUserName.Text))
            {
                RegistracionError.Text = "Login nie może być pusty";
                return;
            }

            if (StringIsNull(RegistracionPassword.Password))
            {
                RegistracionError.Text = "Hasło nie może być puste";
                return;
            }

            if (databaseManager.CheckIfValueExistInDataBase(login))
            {
                RegistracionError.Text = "Login jest zajety";
                DisableText();
                return;
            }

            if (RegistracionPassword.Password != RegistracionRepetdPassword.Password)
            {
                RegistracionError.Text = "Hasła nie są takie same";
                DisableText();
                return;
            }
            DateTime currentDate = DateTime.Today;
            // wyslij dane do bazy danych jezeli są poprawne
            string newUser = $@"INSERT INTO klienci (imie, nazwisko,login,haslo) VALUES('{RegistracionName.Text}','{RegistracionLastName.Text}','{RegistracionUserName.Text}','{RegistracionPassword.Password}') ";
            string newUserAccount = $@"INSERT INTO konto (klient,saldo,data_zalozenia_konta) VALUES('{RegistracionUserName.Text}',0,'{currentDate}')";
            databaseManager.InsertData(newUser);
            databaseManager.InsertData(newUserAccount);
            ChangeSceneToLogin();
           
        }
        //sprawdz czy string jest pusty lub null
        bool StringIsNull(string text)
        {
            
            if (string.IsNullOrEmpty(text))
            {
                DisableText();
                return true;
            }

            return false;

        }

        async void DisableText()
        {
            await DisableErrorMessage();
        }
        //poczekaj 10 sekund do wyłączenia komunikatu błędu
        async Task DisableErrorMessage()
        {
            await Task.Delay(10000);
            RegistracionError.Text = "";
        }

        // zmienia otwarte okno na logowanie
        void ChangeSceneToLogin()
        {
            MainWindow mainWindow = new MainWindow();

            this.Close();
            mainWindow.Show();

        }
    }
}

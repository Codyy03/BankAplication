using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy Bank.xaml
    /// </summary>
    /// ta klasa reprezentuje wszystkie funkcjolaności sceny 'Bank'
    public partial class Bank : Window
    {
       
        DatabaseManager databaseManager = new DatabaseManager(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        string login;
        decimal accountId;
        double bankBalanceValue;
        public Bank(string currentUserName, string login)
        {
            this.login = login;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            // zapytanie SQL pobierające aktualne wartosci zalogowanego.
            string lastNameQuery = $@"SELECT nazwisko FROM klienci WHERE login = '{login}'";
            string createdAccountDate = $@"SELECT data_zalozenia_konta FROM konto WHERE klient = '{login}'";
            string accountIdQuery = $@"SELECT id_klienta FROM konto WHERE klient = '{login}'";
            // zapytanie pobierające saldo konta
            string bankBalance = $@"SELECT saldo FROM konto WHERE klient= '{login}'";
            accountId = databaseManager.ReturnSingleQueryValue<decimal>(accountIdQuery);

            // wyswietlenie danych użytkownika
            NameInBankScene.Text = currentUserName + " " + databaseManager.ReturnSingleQueryValue<string>(lastNameQuery);
            UserNameInBankScene.Text = login;
            AccountDateInBankScene.Text = "Data założenia konta: " + databaseManager.ReturnSingleQueryValue<DateTime>(createdAccountDate);

            bankBalanceValue = databaseManager.ReturnSingleQueryValue<double>(bankBalance);
            BankBalance.Text = bankBalanceValue.ToString(); // wyświetla aktualne saldo konta

            DisplayCurrentAccountValues();

            // ustawiwa maksumum slidera na stan konta
            PayoutsSlider.Maximum = bankBalanceValue;
        }

        void UpdateBankAccount(double value)
        {
            bankBalanceValue += value;
            BankBalance.Text = bankBalanceValue.ToString();
            PayoutsSlider.Maximum = bankBalanceValue;
        }
        void DisplayCurrentAccountValues()
        {
            // Inicjalizacja listy przechowującej transakcje
            List<string> transactions = new List<string>();
            List<DateTime> transactionDates = new List<DateTime>();
           
            transactions.Add("Historia tranzakcji:");

            // Zapytanie SQL łączące wpłaty i wypłaty
            string transactionsQuery = $@"
            SELECT wplata.data_wplaty AS data, wplata.kwota AS kwota, 'wplata' AS typ
            FROM konto
            INNER JOIN wplata ON konto.id_klienta = wplata.id_konta
            WHERE konto.klient = '{login}'
            UNION ALL
            SELECT wyplata.data_wyplaty AS data, -wyplata.kwota AS kwota, 'wyplata' AS typ
            FROM konto
            INNER JOIN wyplata ON konto.id_klienta = wyplata.id_konta
            WHERE konto.klient = '{login}'
            ORDER BY data DESC";
            
            // Pobieranie danych z bazy
            var transactionsData = databaseManager.ExecuteQuery(transactionsQuery);

            foreach (DataRow row in transactionsData.Rows)
            {
                DateTime date = Convert.ToDateTime(row["data"]); // konwrtuje zawartość na date
                string amount = row["kwota"].ToString(); // konwrtuje zawartość kwote
                string type = row["typ"].ToString(); // sprawdza czy to wpłata czy wypłata i konwertuje

                string formattedTransaction = (type == "wplata" ? "+" : "") + amount; // jezeli rząd ma typ 'wplata' dodaje + na początku
                transactionDates.Add(date); // dodaje przekonwertowaną date do listy
                transactions.Add($"{date.ToString("dd-MM-yyyy")}: {formattedTransaction}"); // formatuje date dzien-miesiac-rok i łączy z kwotą która
            }



            // Wyświetlanie w ListBox
            MoneyList.ItemsSource = transactions;

            

        }
        // wylogowanie
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();
            this.Close();
        }

        // usuwa konto 
        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            // zapytania usuwające dane
            string deleteAccoutnQuery = $@"DELETE FROM konto WHERE klient = '{login}'";
            string deleteCustomerQuery =  $@"DELETE FROM klienci WHERE login = '{login}'";

            // wywołanie zapytań
            databaseManager.InsertData(deleteAccoutnQuery);
            databaseManager.InsertData(deleteCustomerQuery);

            // zmiana sceny na logowanie
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();  
            this.Close();



        }
        // ustawia wartosc do wyplacenia
        private void PayoutsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PayoutsValue != null) // Sprawdzenie, czy kontrolka PayoutsValue jest zainicjalizowana
            {
                int payoutAmount = (int)e.NewValue; // Konwersja wartości suwaka na int
                PayoutsValue.Text = payoutAmount.ToString(); // Aktualizacja tekstu
              
            }
        }

        // wyplac pieniadze
        private void PayoutMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            if (PayoutsSlider.Value == 0) // jezeli wartosc sildera jest 0 nie wykonuj funkcji
                return;

            // inincjalizacja zmiennych
            double payoutAmount = (int)PayoutsSlider.Value;
            DateTime todayDate = DateTime.Today;
            // zapytanie tworzące wypłate
            string userDepositMoneyQuery = $@"INSERT INTO wyplata (id_konta,kwota,data_wyplaty) VALUES({accountId},{payoutAmount},'{todayDate:yyyy-MM-dd}')";

            databaseManager.InsertData(userDepositMoneyQuery);
            
            // aktualizacja danych 
            DisplayCurrentAccountValues();
            UpdateBankAccount(-payoutAmount);
            string updateBankBalance = $@"UPDATE konto SET saldo = {bankBalanceValue} WHERE klient = '{login}'";
            databaseManager.InsertData(updateBankBalance);

        }
        // wplac pieniadze 
        private void DepositMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            if (DepositSlider.Value == 0) // jezeli wartosc sildera jest 0 nie wykonuj funkcji
                return;

            // inincjalizacja zmiennych
            double depositAmount = (int)DepositSlider.Value;
            DateTime todayDate = DateTime.Today;

            // zapytanie tworzące wpłate
            string userDepositMoneyQuery = $@"INSERT INTO wplata (id_konta,kwota,data_wplaty) VALUES({accountId},{depositAmount},'{todayDate:yyyy-MM-dd}')";
            databaseManager.InsertData(userDepositMoneyQuery);

            // aktualizacja danych 
            DisplayCurrentAccountValues();
            UpdateBankAccount(depositAmount);
            string updateBankBalance = $@"UPDATE konto SET saldo = {bankBalanceValue} WHERE klient = '{login}'";
            databaseManager.InsertData(updateBankBalance);
        }

        private void DepositSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DepositSlider != null) // Sprawdzenie, czy kontrolka PayoutsValue jest zainicjalizowana
            {
                int depositAmount = (int)e.NewValue; // Konwersja wartości suwaka na int
                DepositValue.Text = depositAmount.ToString(); // Aktualizacja tekstu

            }
        }
    }
}

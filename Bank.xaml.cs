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
    public partial class Bank : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Bank(string currentUserName, string login)
        {
            // Inicjalizacja listy przechowującej transakcje
            List<string> transactions = new List<string>();
            List<DateTime> transactionDates = new List<DateTime>();
            transactions.Add("Historia tranzakcji");
            InitializeComponent();

            NameInBankScene.Text = currentUserName;
            DatabaseManager databaseManager = new DatabaseManager(connectionString);

            // zapytanie SQL pobierające aktualne saldo.
            string bankBalance = $@"SELECT saldo FROM konto WHERE klient= '{login}'";

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
            ORDER BY data";

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

            BankBalance.Text = databaseManager.ReturnSingleQueryValue(bankBalance); // wyświetla aktualne saldo konta

            // Wyświetlanie w ListBox
            MoneyList.ItemsSource = transactions;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Npgsql;
using NpgsqlTypes;

namespace Projekt
{
    internal class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // zwraca całą tabelke danych z zapytania
        public DataTable ExecuteQuery(string query)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // zwracaj liste dowolnej zmiennej z bazy danych 
        public List<T> ExecuteQueryToList<T>(string query)
        {
            List<T> list = new List<T>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add((T)reader[0]);
                        }
                    }
                }
            }

            return list;
        }

        // sprawdza czy wartosc z zapytania istnieje
        public bool CheckIfValueExistInDataBase(string query)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Jeśli jest jakiś wiersz, to wartość istnieje
                        return reader.HasRows;
                      
                    }
                }
            }
        }
        // zwraca pojedyńczą wartość z zapytania
        public string ReturnSingleQueryValue(string query)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader[0].ToString();
                        }
                        else
                        {
                            // Brak wyników, zwróć null lub odpowiednią wartość domyślną
                            return null;
                        }
                    }
                }
            }
        }

        // pozwala zadawać zapytania do bazy danych nie zwracające nic. Np update, insert into
        public void InsertData(string query)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}


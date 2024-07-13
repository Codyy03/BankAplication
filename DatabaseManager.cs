﻿using System;
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

        public List<string> ExecuteQueryToStringList(string query)
        {

            List<string> list = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader[0].ToString());
                        }
                    }
                }
            }

            return list;

        }

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Telegram.Bot.Types;

namespace Bot1
{
    class Database
    {
        private static NpgsqlConnection connection;
        private const string CONNECTION_STRING = "Server=217.28.223.128;port=36700;User Id=admin;Password=admin;Database=Repetitors_bot;";

        public static void Connect()
        {
            connection = new NpgsqlConnection(CONNECTION_STRING);
            connection.OpenAsync();
            Console.WriteLine("Подключение к базе данных успешно установлено.");
        }

        public static async Task Add(Message message, string Full_Name, string class_st, string telephone_number_st, string description_st, string tg_name_st)
        {
            string CommandText = "call add_student_procedure (name_st, class_st, description_st, telephone_number_st, tg_name_st text);";
            using (var cmd = new NpgsqlCommand(CommandText, connection))
            {
                cmd.Parameters.AddWithValue("name_st", Full_Name);
                cmd.Parameters.AddWithValue("class_st", class_st);
                cmd.Parameters.AddWithValue("description_st", description_st);
                cmd.Parameters.AddWithValue("telephone_number_st", telephone_number_st);
                cmd.Parameters.AddWithValue("tg_name_st", tg_name_st);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Telegram.Bot.Types;

namespace Bot1
{
    class Database
    {
        static public string connectionString = "Server=217.28.223.128;port=36700;User Id=admin;Password=admin;Database=Repetitors_bot";

        public static async Task AddStudent(Dictionary<long, StudentInfo> studentInfo, Message message)
        {
            string CommandText = "call add_student_procedure (@name_st, @class_st, @description_st, @telephone_number_st, @tg_name_st, @chat_id_st);";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(CommandText, connection))
                {
                    cmd.Parameters.AddWithValue("name_st", studentInfo[message.Chat.Id].Name + " " + studentInfo[message.Chat.Id].LastName);
                    cmd.Parameters.AddWithValue("class_st", studentInfo[message.Chat.Id].StudentClass);
                    cmd.Parameters.AddWithValue("description_st", studentInfo[message.Chat.Id].Description);
                    cmd.Parameters.AddWithValue("telephone_number_st", studentInfo[message.Chat.Id].PhoneNumber);
                    cmd.Parameters.AddWithValue("tg_name_st", studentInfo[message.Chat.Id].TgName);
                    cmd.Parameters.AddWithValue("chat_id_st", studentInfo[message.Chat.Id].ChatId);

                    cmd.ExecuteNonQuery();
                }
            }
            return;
        }

        public static bool CheckUser(Message message)
        {
            var chat_id = message.Chat.Id;
            string sql = "SELECT * FROM students WHERE chat_id = @chat_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@chat_id", chat_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (reader.HasRows) 
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}

            

                 
          

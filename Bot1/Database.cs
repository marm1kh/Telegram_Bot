using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Telegram.Bot;
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

        public static async Task AddTeacher(Dictionary<long, TeacherInfo> teacherInfo, Message message)
        {
            string CommandText = "call add_teacher_procedure (@name_te, @subject_te, @description_te, @fix_time_te, @price_te, @tg_name_te, @chat_id_te);";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(CommandText, connection))
                {
                    cmd.Parameters.AddWithValue("name_te", teacherInfo[message.Chat.Id].Name + " " + teacherInfo[message.Chat.Id].LastName);
                    cmd.Parameters.AddWithValue("subject_te", teacherInfo[message.Chat.Id].Subject);
                    cmd.Parameters.AddWithValue("description_te", teacherInfo[message.Chat.Id].Description);
                    cmd.Parameters.AddWithValue("fix_time_te", TimeSpan.Parse(teacherInfo[message.Chat.Id].FixTime));
                    cmd.Parameters.AddWithValue("price_te", teacherInfo[message.Chat.Id].Price);
                    cmd.Parameters.AddWithValue("tg_name_te", teacherInfo[message.Chat.Id].TgName);
                    cmd.Parameters.AddWithValue("chat_id_te", teacherInfo[message.Chat.Id].ChatId);

                    cmd.ExecuteNonQuery();
                }
            } 
            return;
        }

        public static string CheckUser(ITelegramBotClient botClient, Message message)
        {
            var chat_id = message.Chat.Id;
            string sql = @"SELECT 'students' AS students, chat_id FROM students WHERE chat_id = @chat_id
                           UNION ALL
                           SELECT 'teachers' AS teachers, chat_id FROM teachers WHERE chat_id = @chat_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("chat_id", chat_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        return tableName;
                    }
                    return null;
                }
            }
        }
    }
}

            

                 
          

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;



namespace Bot1
{
    internal class Bot1
    {

        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient("6635411143:AAGgN4ZPdhcjcd0Mw27_XCjSVQxaHAtwyw8");
            botClient.StartReceiving(Update, Error);
            Database.Connect();
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }
        }

        async static Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(
                    new[]
                    {
                        new KeyboardButton[] {"Регистрация", "Авторизация" },
                        new KeyboardButton[] {"Информация о проекте", "Поделиться ботом" }
                    })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуйте, {message.Chat.Username}! Выберите соотвествующую кнопку:", replyMarkup: replyKeyboard);
                return;
            };

            if (message.Text == "Регистрация")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваше имя: ");
                message.Chat.FirstName = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите вашу фамилию: ");
                message.Chat.LastName = message.Text;
                string Full_Name = message.Chat.FirstName + " " + message.Chat.LastName;
                await botClient.SendTextMessageAsync(message.Chat.Id, "В каком классе вы обучаетесь: ");
                string class_st = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваш номер телефона: ");
                string telephone_number_st = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Расскажите немного о себе: ");
                string description_st = message.Text;
                string tg_name_st = message.Chat.Username;
                await Database.Add(message, Full_Name, class_st, telephone_number_st, description_st, tg_name_st);
            };

            if (message.Text == "Информация о проекте")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Бот \"Репетиторы здесь!\" был создан для поиска репетитора по всей стране.\n Очное и дистанционное образование уверенно использется в нашей жизни, поэтому Мы собираем преподавателей из разных сфер в одном месте.\n\n 🔥 Удобный поиск репетиторов с различными форматами обучения. \n 🔥 Помощь в написании контрольных и самостоятельных работ. \n 🔥 Проверенные репетиторы со всей страны.");
                return;
            };

            if (message.Text == "Поделиться ботом")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Мы будем рады, если Вы поделитесь нашим ботом с друзьями!\n\nСсылка на Бота: @Repetitors_here_bot");
                return;
            };
        }

    async static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            
        }

        
        
    }

}
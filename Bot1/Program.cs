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
using System.Runtime.Remoting.Messaging;



namespace Bot1
{
    internal class Bot1
    {
        private static Dictionary<long, State> userState;
        private static Dictionary<long, StudentInfo> studentInfo;
        static void Main(string[] args)
        {
            userState = new Dictionary<long, State>();
            studentInfo = new Dictionary<long, StudentInfo>();
            var botClient = new TelegramBotClient("6635411143:AAGgN4ZPdhcjcd0Mw27_XCjSVQxaHAtwyw8");
            botClient.StartReceiving(Update, Error);
            Database.Connect();
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            Message message = new Message();
            if (update?.Message?.Text == null)
            {
                userState[message.Chat.Id] = State.WaitingStart;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите команду /start");
                return;
            }

            //if (userState.ContainsKey(message.Chat.Id))
            //{
            //    userState[message.Chat.Id] = State.WaitingStart;
            //    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите команду /start");
            //    return;
            //}

            if (message.Text == "/start" && userState[message.Chat.Id] == State.WaitingStart) 
                {
                    await HandleMessage(botClient, update, message);
                }

            if (message.Text == "Регистрация" && userState[message.Chat.Id] == State.WaitingButton)
            {
                studentInfo[message.Chat.Id] = new StudentInfo();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваше имя: ");
                userState[message.Chat.Id] = State.WaitingName;
            }

            if (userState[message.Chat.Id] == State.WaitingName)
            {
                message.Chat.FirstName = message.Text;
                userState[message.Chat.Id] = State.WaitingLastName;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Введите вашу фамилию: ");
            }

            if (message.Text == "Информация о проекте")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Бот \"Репетиторы здесь!\" был создан для поиска репетитора по всей стране.\n Очное и дистанционное образование уверенно использется в нашей жизни, поэтому Мы собираем преподавателей из разных сфер в одном месте.\n\n 🔥 Удобный поиск репетиторов с различными форматами обучения. \n 🔥 Помощь в написании контрольных и самостоятельных работ. \n 🔥 Проверенные репетиторы со всей страны.");
                return;
            }

            if (message.Text == "Поделиться ботом")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Мы будем рады, если Вы поделитесь нашим ботом с друзьями!\n\nСсылка на Бота: @Repetitors_here_bot");
                return;
            }
        }

        async static Task HandleMessage(ITelegramBotClient botClient, Update update, Message message)
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
                userState[update.Message.Chat.Id] = State.WaitingButton;
                return;
            };
        }

    async static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            
        }

        
        
    }

    class StudentInfo
    {
        public string Name;
        public string LastName;
        public int StudentClass;
        public string PhoneNumber;
        public string Description;
    }

    public enum State
    {
        WaitingStart,
        WaitingButton,
        WaitingName,
        WaitingLastName,
        WaitingStudentClass,
        WaitingPhoneNumber,
        WaitingDescription,
    }

    

}
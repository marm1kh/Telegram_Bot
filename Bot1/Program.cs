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
            var botClient = new TelegramBotClient("6635411143:AAGgN4ZPdhcjcd0Mw27_XCjSVQxaHAtwyw8");
            userState = new Dictionary<long, State>();
            studentInfo = new Dictionary<long, StudentInfo>();

            botClient.StartReceiving(Update, Error);
            Database.Connect();
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text != null)
            {

                if (!userState.ContainsKey(message.Chat.Id))
                {
                    userState[message.Chat.Id] = State.WaitingStart;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите команду /start");
                    return;
                }

                if (message.Text == "/start" && userState[message.Chat.Id] == State.WaitingStart) // Команда /start
                {
                    await HandleStart(botClient, update, message);
                }

                if (userState[message.Chat.Id] == State.WaitingName) // Запрос имени пользователя
                {
                    studentInfo[message.Chat.Id].Name = message.Text;
                    userState[message.Chat.Id] = State.WaitingLastName;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите вашу фамилию: ");
                    return;
                }

                if (userState[message.Chat.Id] == State.WaitingLastName) // Запрос фамилии пользователя
                {
                    studentInfo[message.Chat.Id].LastName = message.Text;
                    userState[message.Chat.Id] = State.WaitingStudentClass;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваш класс: ");
                    return;
                }

                if (userState[message.Chat.Id] == State.WaitingStudentClass) // Запрос класса пользователя
                {
                    studentInfo[message.Chat.Id].StudentClass = int.Parse(message.Text);
                    userState[message.Chat.Id] = State.WaitingPhoneNumber;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваш номер телефона: ");
                    return;
                }

                if(userState[message.Chat.Id] == State.WaitingPhoneNumber) // Запрос номера телефона пользователя
                {
                    studentInfo[message.Chat.Id].PhoneNumber = message.Text;
                    userState[message.Chat.Id] = State.WaitingDescription;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Расскажите немного о себе: ");
                    return;
                }

                if (userState[message.Chat.Id] == State.WaitingDescription) // Запрос информации о пользователи
                {
                    studentInfo[message.Chat.Id].Description = message.Text;
                    userState[message.Chat.Id] = State.WaitingButton;
                    return;
                }

                if (message.Text == "Регистрация" && userState[message.Chat.Id] == State.WaitingButton) // Регистрация
                {
                    studentInfo[message.Chat.Id] = new StudentInfo();
                    studentInfo[message.Chat.Id].ChatId = message.Chat.Id;
                    studentInfo[message.Chat.Id].TgName = message.Chat.Username;
                    await StudentTeacher(botClient, update, message);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите ваше имя: ");
                    userState[message.Chat.Id] = State.WaitingName;
                    return;
                }
                
                if (message.Text == "Информация о проекте") // Информация о проекте
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Бот \"Репетиторы здесь!\" был создан для поиска репетитора по всей стране.\n Очное и дистанционное образование уверенно использется в нашей жизни, поэтому Мы собираем преподавателей из разных сфер в одном месте.\n\n 🔥 Удобный поиск репетиторов с различными форматами обучения. \n 🔥 Помощь в написании контрольных и самостоятельных работ. \n 🔥 Проверенные репетиторы со всей страны.");
                    return;
                }

                if (message.Text == "Поделиться ботом") // Поделиться ботом
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Мы будем рады, если Вы поделитесь нашим ботом с друзьями!\n\nСсылка на Бота: @Repetitors_here_bot");
                    return;
                }

            }

        }

        async static Task HandleStart(ITelegramBotClient botClient, Update update, Message message) // Кнопки при вводе команды /start
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
            }
        }

        async static Task StudentTeacher(ITelegramBotClient botClient, Update update, Message message) // Кнопки при нажатии Регистрация
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                    new[]
                    {
                        new KeyboardButton[] {"Ученик", "Преподаватель" },
                    })
            {
                ResizeKeyboard = true
            };
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Выберите соотвествующую кнопку:", replyMarkup: replyKeyboard);
            userState[update.Message.Chat.Id] = State.WaitingButton;
            return;
        }

    async static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            
        }
    }

    class StudentInfo
    {
        public long ChatId;
        public string TgName;
        public string Name;
        public string LastName;
        public int StudentClass;
        public string PhoneNumber;
        public string Description;
    }

    class TeacherInfo
    {
        public long ChatId;
        public string TgName;
        public string Name;
        public string LastName;
        public string Subject;
        public string Description;
        public string FixTime;
        public string Price;
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
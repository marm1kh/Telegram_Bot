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

        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient("6635411143:AAGgN4ZPdhcjcd0Mw27_XCjSVQxaHAtwyw8");
            userState = new Dictionary<long, State>();    

            botClient.StartReceiving(Update, Error);
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
               
                await HandleStart(botClient, update, message);
                await StudentTeacher(botClient, update, message);
                await Student.RegistrationStudent(botClient, update, userState);
                await Student.SendInformationStudent(botClient, update, userState);
                await Teacher.RegistrationTeacher(botClient, update, userState);
                await Teacher.SendInformationTeacher(botClient, update, userState);
                
                if (message.Text == "Информация о проекте" && userState[message.Chat.Id] == State.WaitingButton) // Информация о проекте
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Бот \"Репетиторы здесь!\" был создан для поиска репетитора по всей стране.\n Очное и дистанционное образование уверенно использется в нашей жизни, поэтому Мы собираем преподавателей из разных сфер в одном месте.\n\n 🔥 Удобный поиск репетиторов с различными форматами обучения. \n 🔥 Помощь в написании контрольных и самостоятельных работ. \n 🔥 Проверенные репетиторы со всей страны.");
                    return;
                }

                if (message.Text == "Поделиться ботом" && userState[message.Chat.Id] == State.WaitingButton) // Поделиться ботом
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Мы будем рады, если Вы поделитесь нашим ботом с друзьями!\n\nСсылка на Бота: @Repetitors_here_bot");
                    return;
                }

            }

        }

        async static Task HandleStart(ITelegramBotClient botClient, Update update, Message message) // Кнопки при вводе команды /start
        {
            if (message.Text == "/start" && userState[message.Chat.Id] == State.WaitingStart)
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
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуйте, {message.Chat.Username}! Выберите соответствующую кнопку:", replyMarkup: replyKeyboard);
                userState[update.Message.Chat.Id] = State.WaitingButton;
                return;
            }
        }

        async static Task StudentTeacher(ITelegramBotClient botClient, Update update, Message message) // Кнопки при нажатии Регистрация
        {
            if (message.Text != "Регистрация" || userState[message.Chat.Id] != State.WaitingButton)
            {
                return;   
            }

            var tableName = Database.CheckUser(botClient, message);

            if (tableName is null)
            {
                var replyKeyboard = new ReplyKeyboardMarkup(
                    new[]
                    {
                        new KeyboardButton[] {"Ученик", "Преподаватель" },
                    })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите соответствующую кнопку:", replyMarkup: replyKeyboard);
                userState[update.Message.Chat.Id] = State.WaitingButton;
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Вы уже зарегистрированы как {tableName}"); 
                userState[update.Message.Chat.Id] = State.WaitingButton;
                return;
            }
        }

    async static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            
        }
    }

    public enum State
    {
        WaitingStart,
        WaitingButton,
        WaitingNameStudent,
        WaitingNameTeacher,
        WaitingLastNameStudent,
        WaitingLastNameTeacher,
        WaitingStudentClass,
        WaitingPhoneNumber,
        WaitingDescriptionStudent,
        WaitingDescriptionTeacher,
        WaitingDataBaseStudent,
        WaitingDataBaseTeacher,
        WaitingSubject,
        WaitingFixTime,
        WaitingPrice
    }
}
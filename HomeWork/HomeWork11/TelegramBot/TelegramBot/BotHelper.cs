using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Dto;
using TelegramBot.Entities;

namespace TelegramBot
{
    internal static class BotHelper
    {
        public static readonly string BASE_DIR = Path.Combine("D:", "TgBot");
        public static ReplyKeyboardMarkup GetKeyboardButtons(bool userRegistered)
        {
            if (userRegistered)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "/addtask", "/showalltasks", "/showtasks", "/report" },
            })
                {
                    ResizeKeyboard = true
                };

                return replyKeyboardMarkup;

            }
            else
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "/start" },
            })
                {
                    ResizeKeyboard = true
                };

                return replyKeyboardMarkup;
            }
        }

        public static ReplyKeyboardMarkup GetKeyboardCancel()
        {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "/cancel" },
            })
                {
                    ResizeKeyboard = true
                };

                return replyKeyboardMarkup;
        }
        public static void ValidateString(string? str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var item in str)
                {
                    if (!char.IsWhiteSpace(item))
                    {
                        return;
                    }
                }
            }
            throw new ArgumentException("Передаваемый параметр пуст или содержит одни пробелы");
        }
    }
}

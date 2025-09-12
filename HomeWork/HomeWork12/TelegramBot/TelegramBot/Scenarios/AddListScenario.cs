using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entities;
using TelegramBot.Services;
using static TelegramBot.BotHelper;

namespace TelegramBot.Scenarios
{
    internal class AddListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, ct);
                    context.CurrentStep = "Name";
                    context.Data.Add("User", toDoUser);
                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Введите название списка:", cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Name":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    await _toDoListService.Add(toDoUser, update.Message.Text, ct);
                    await bot.SendMessage(update.Message.Chat.Id, "Список добавлен", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                    return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет case для шага {context.CurrentStep}");
            }
        }
    }
}

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
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(update.Message.From.Id, ct);
                    context.CurrentStep = "Name";
                    context.Data.Add("User", toDoUser);
                    await bot.SendMessage(update.Message.Chat, "Введите название задачи:", cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Name":
                    context.Data.Add(context.CurrentStep, update.Message.Text);  //Date -> Название задачи
                    context.CurrentStep = "Date";
                    await bot.SendMessage(update.Message.Chat, "Введите срок выполнения:", cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Date":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    toDoItemName = (string)context.Data.GetValueOrDefault("Name");
                    if (DateTime.TryParse(update.Message.Text, out DateTime toDoDate))
                    {
                        await _toDoService.AddAsync(toDoUser, toDoItemName, toDoDate, null, ct); 
                        await bot.SendMessage(update.Message.Chat, "Задача добавлена", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                        return ScenarioResult.Completed;
                    }
                    else
                    {
                        await bot.SendMessage(update.Message.Chat, "Ошибка в дате. Введите срок выполнения еще раз:", cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                        return ScenarioResult.Transition;
                    }

                default:
                    throw new NotSupportedException($"Нет case для шага {context.CurrentStep}");
            }
            

        }
    }
}

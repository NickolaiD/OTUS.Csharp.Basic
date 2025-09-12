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
using TelegramBot.Dto;
using TelegramBot.Entities;
using TelegramBot.Services;
using static TelegramBot.BotHelper;

namespace TelegramBot.Scenarios
{
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoListService _toDoListService;
        public AddTaskScenario(IUserService userService, IToDoService toDoService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            DateTime toDoDate;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, ct);
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
                    if (DateTime.TryParse(update.Message.Text, out toDoDate))
                    {
                        context.Data.Add(context.CurrentStep, toDoDate);
                        context.CurrentStep = "List";

                        var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, ct);
                        var listButtons = new List<InlineKeyboardButton>();

                        listButtons.Add(InlineKeyboardButton.WithCallbackData("📌Без списка", $"selectlist|null"));
                        foreach (var list in toDoLists)
                        {
                            listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"selectlist|{list.Id}"));
                        }

                        var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                        {
                        listButtons.ToArray()
                    });

                        await bot.SendMessage(update.Message.Chat.Id, "Выберите список:", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
                    }
                    else
                    {
                        await bot.SendMessage(update.Message.Chat, "Ошибка в дате. Введите срок выполнения еще раз:", cancellationToken: ct, replyMarkup: GetKeyboardCancel());
                    }
                    return ScenarioResult.Transition;
                case "List":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    toDoItemName = (string)context.Data.GetValueOrDefault("Name");
                    toDoDate = (DateTime)context.Data.GetValueOrDefault("Date");

                    var callback = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    if (callback.ToDoListId == null)
                    {
                        await _toDoService.AddAsync(toDoUser, toDoItemName, toDoDate, null, ct);
                    }
                    else
                    {
                        var toDoList = await _toDoListService.Get(callback.ToDoListId.GetValueOrDefault(), ct);
                        await _toDoService.AddAsync(toDoUser, toDoItemName, toDoDate, toDoList, ct);
                    }


                        await bot.SendMessage(update.CallbackQuery.Message.Chat, "Задача добавлена", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                    return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет case для шага {context.CurrentStep}");
            }
            

        }
    }
}

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
    internal class DeleteListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoService _toDoService;
        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.DeleteList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            InlineKeyboardMarkup replyKeyboardMarkup;
            ToDoListCallbackDto? toDoListCallback = null;
            CallbackDto? callback = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(update.CallbackQuery.From.Id, ct);
                    context.CurrentStep = "Approve";
                    context.Data.Add("User", toDoUser);
                    
                    var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, ct);
                    var listButtons = new List<InlineKeyboardButton>();
                    foreach (var list in toDoLists)
                    {
                        listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"deletelist|{list.Id}"));
                    }

                    replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                    {
                        listButtons.ToArray()
                    });

                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Выберите список:", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
                    return ScenarioResult.Transition;
                case "Approve":
                    //toDoUser = await _userService.GetUserAsync(update.Message.From.Id, ct);
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");

                    toDoListCallback = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    var toDoList = await _toDoListService.Get(toDoListCallback.ToDoListId, ct);

                    context.CurrentStep = "Delete";
                    context.Data.Add("toDoList", toDoList);

                    replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                            InlineKeyboardButton.WithCallbackData("❌Нет", "no")
                        }
                    });

                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, $"Подтверждаете удаление списка { toDoList.Name} и всех его задач?", cancellationToken: ct, replyMarkup: replyKeyboardMarkup);
                    
                    //await _toDoListService.Add(toDoUser, update.Message.Text, ct);
                    return ScenarioResult.Transition;
                case "Delete":
                    callback = CallbackDto.FromString(update.CallbackQuery.Data);
                    if (callback.Action == "yes")
                    {
                        toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                        toDoList = (ToDoList?)context.Data.GetValueOrDefault("toDoList");

                        var userTasks = await _toDoService.GetByUserIdAndListAsync(toDoUser.UserId, toDoList.Id, ct);
                        foreach(var task in userTasks)
                        {
                            await _toDoService.DeleteAsync(task.Id, ct);
                        }

                        await _toDoListService.Delete(toDoList.Id, ct);
                    }
                    else if (callback.Action == "no")
                    {
                        await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Удаление отменено", cancellationToken: ct, replyMarkup: GetKeyboardButtons(true));
                    }
                        return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет case для шага {context.CurrentStep}");
            }
        }
    }
}

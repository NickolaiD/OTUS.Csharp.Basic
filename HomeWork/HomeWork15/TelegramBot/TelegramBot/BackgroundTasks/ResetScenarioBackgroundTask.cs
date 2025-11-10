using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Helpers;
using TelegramBot.Scenarios;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace TelegramBot.BackgroundTasks
{
    public class ResetScenarioBackgroundTask : BackgroundTask
    {
        private readonly TimeSpan _resetScenarioTimeout;
        private readonly IScenarioContextRepository _scenarioRepository;
        private readonly ITelegramBotClient _bot;
        public ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository scenarioRepository, ITelegramBotClient bot) : base(TimeSpan.FromHours(1), nameof(ResetScenarioBackgroundTask))
        {
            _resetScenarioTimeout = resetScenarioTimeout;
            _scenarioRepository = scenarioRepository;
            _bot = bot;
        }

        protected override async Task Execute(CancellationToken ct)
        {
            var scenarioContexts = await _scenarioRepository.GetContexts(ct);
            foreach (var scenarioContext in scenarioContexts)
            {
                if ((DateTime.Now - scenarioContext.CreatedAt) > _resetScenarioTimeout)
                {
                    await _bot.SendMessage(scenarioContext.ChatId, $"Сценарий отменен, так как не поступил ответ в течение {_resetScenarioTimeout}", cancellationToken: ct, replyMarkup: BotHelper.GetKeyboardButtons(true));
                    await _scenarioRepository.ResetContext(scenarioContext.UserId, ct);
                }
            }
        }
    }
}

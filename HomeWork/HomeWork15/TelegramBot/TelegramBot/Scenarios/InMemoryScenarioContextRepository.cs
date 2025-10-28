using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Scenarios
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private ConcurrentDictionary<long, ScenarioContext> _scenarioContexts;

        public InMemoryScenarioContextRepository()
        {
            _scenarioContexts = new ConcurrentDictionary<long, ScenarioContext>();
        }

        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            ScenarioContext? result = null;
            await Task.Run(() => 
            {
                 if (_scenarioContexts.TryGetValue(userId, out ScenarioContext? value))
                    result = value;
            }
            ); 
            return result;
        }

        public async Task ResetContext(long userId, CancellationToken ct)
        {
            await Task.Run(() => _scenarioContexts.TryRemove(userId, out ScenarioContext? value));
        }

        public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            await Task.Run(() => _scenarioContexts.GetOrAdd(userId, context));
        }
        public async Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken ct)
        {
            var result = await Task.Run(() => _scenarioContexts.Select(x => x.Value).ToList());
            return result;
        }
    }
}

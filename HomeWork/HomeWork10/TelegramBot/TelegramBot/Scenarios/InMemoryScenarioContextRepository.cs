using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Scenarios
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private Dictionary<long, ScenarioContext> _scenarioContexts;

        public InMemoryScenarioContextRepository()
        {
            _scenarioContexts = new Dictionary<long, ScenarioContext>();
        }

        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            ScenarioContext? result = null;
            await Task.Run(() => result = _scenarioContexts.GetValueOrDefault(userId));
            return result;
        }

        public async Task ResetContext(long userId, CancellationToken ct)
        {
            await Task.Run(() => _scenarioContexts.Remove(userId));
        }

        public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            await Task.Run(() => _scenarioContexts.Add(userId, context));
        }
    }
}

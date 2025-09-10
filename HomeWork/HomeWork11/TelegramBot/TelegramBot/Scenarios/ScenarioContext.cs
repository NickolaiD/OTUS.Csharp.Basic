using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Scenarios
{
    public class ScenarioContext
    {

        public long UserId { get; }
        public ScenarioType CurrentScenario { get; }
        public string? CurrentStep { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public ScenarioContext(ScenarioType scenario, long userId)
        {
            UserId = userId;
            CurrentScenario = scenario;
            Data = new Dictionary<string, object>();
        }
    }
}

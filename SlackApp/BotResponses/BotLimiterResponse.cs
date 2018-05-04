using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackAPI.WebSocketMessages;
using SlackAPIService;

namespace SlackApp.BotResponses
{
    public class BotLimiterResponse : AbstractSocketResponse
    {
        public List<BotCooldownEntry> BotCooldowns = new List<BotCooldownEntry>();

        protected BotLimiterResponse(ISlackClient client) : base(client)
        {
        }

        public override void MessageReceiver(NewMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public class BotCooldownEntry
    {
        string Botname { get; set; }
        string Channel { get; set; }
        DateTime CooldownEnd { get; set; }
        TimeSpan CoolDown { get; set; }
    }
}

using SlackAPI.WebSocketMessages;
using SlackAPIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public class RegexResponse : AbstractSocketResponse
    {
        public List<RegexResponseEntry> RegexResponses { get; set; }

        public RegexResponse(ISlackClient client) : base(client)
        {
        }

        public override void MessageReceiver(NewMessage message)
        {
            foreach(var r in RegexResponses)
            {
                if (Regex.IsMatch(r.Regex, message.text))
                {
                    this.Client.RespondToMessage(message, r.Response, ephemeral: r.Ephemeral);
                }
            }
        }
    }

    public class RegexResponseEntry
    {
        public string Regex { get; set; }
        public string Response { get; set; }
        public bool Ephemeral { get; set; }
    }
}

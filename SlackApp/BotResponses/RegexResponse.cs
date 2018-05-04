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
        public List<RegexResponseEntry> RegexResponses { get; set; } = new List<RegexResponseEntry>();

        public RegexResponse(ISlackClient client) : base(client)
        {
            RegexResponses.Add(new RegexResponseEntry(@"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)+","No mom jokes allowed",true));
            RegexResponses.Add(new RegexResponseEntry(@"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)", "Stop being crude", true));
            RegexResponses.Add(new RegexResponseEntry(@"(schl...)|(fourth leg)|(fifth leg)", "We really don't care how tiny it is", true));
        }

        public override void MessageReceiver(NewMessage message)
        {
            foreach(var r in RegexResponses)
            {
                if (!message.channel.StartsWith('D') && Regex.IsMatch(message.text, r.Regex, RegexOptions.IgnoreCase))
                {
                    if (Regex.IsMatch(r.Regex, message.text))
                    {
                        this.Client.RespondToMessage(message, r.Response, ephemeral: r.Ephemeral);
                    }
                }
            }
        }
    }

    public class RegexResponseEntry
    {
        public RegexResponseEntry(string regex, string response, bool ephemeral)
        {
            Regex = regex;
            Response = response;
            Ephemeral = ephemeral;
        }

        public string Regex { get; set; }
        public string Response { get; set; }
        public bool Ephemeral { get; set; }
    }
}

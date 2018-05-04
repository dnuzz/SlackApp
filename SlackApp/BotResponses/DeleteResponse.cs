using SlackAPI.WebSocketMessages;
using SlackAPIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public class DeleteResponse : AbstractSocketResponse
    {
        public List<string> DeleteRegexs { get; set; }

        public DeleteResponse(ISlackClient client) : base(client)
        {
            DeleteRegexs = new List<string>();
            DeleteRegexs.Add(@"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)+");
            DeleteRegexs.Add(@"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)");
            DeleteRegexs.Add(@"(schl...)|(fourth leg)|(fifth leg)");

        }

        public override void MessageReceiver(NewMessage message)
        {
            foreach(var regex in DeleteRegexs)
            {
                if (!message.channel.StartsWith('D') && Regex.IsMatch(message.text, regex, RegexOptions.IgnoreCase))
                {
                    Client.DeleteMessage(message);
                }
            }
        }
    }
}

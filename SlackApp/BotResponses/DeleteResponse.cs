using Amazon.DynamoDBv2;
using SlackAPI.WebSocketMessages;
using SlackAPIService;
using SlackApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public class DeleteResponse : AbstractSocketResponse
    {
        public List<DeleteResponseEntry> DeleteRegexs { get; set; }

        public DeleteResponse(ISlackClient client) : base(client)
        {
            DeleteRegexs = new List<DeleteResponseEntry>();
            DeleteRegexs.Add(new DeleteResponseEntry(@"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)+"));
            DeleteRegexs.Add(new DeleteResponseEntry(@"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)"));
            DeleteRegexs.Add(new DeleteResponseEntry(@"(schl...)|(fourth leg)|(fifth leg)"));

        }

        public override void MessageReceiver(NewMessage message)
        {
            foreach(var regex in DeleteRegexs)
            {
                if (!message.channel.StartsWith('D') && Regex.IsMatch(message.text, regex.Regex, RegexOptions.IgnoreCase))
                {
                    Client.DeleteMessage(message);
                }
            }
        }

        public override void ReloadResponseTriggers()
        {
            throw new NotImplementedException();
        }

        public override void SaveResponseTrigger<T>( T value)
        {
            throw new NotImplementedException();
        }
    }
}

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
        }

        public override void MessageReceiver(NewMessage message)
        {
            foreach(var regex in DeleteRegexs)
            {
                if (Regex.IsMatch(message.text, regex)) ;
            }
            Client.DeleteMessage(message);
        }
    }
}

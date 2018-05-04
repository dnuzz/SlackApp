using SlackAPI.WebSocketMessages;
using SlackAPIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public abstract class AbstractSocketResponse
    {
        protected ISlackClient Client { get; private set; }

        protected AbstractSocketResponse(ISlackClient client) {
            Client = client;
            Client.SubscribeToMessage(MessageReceiver);
        }

        public abstract void MessageReceiver(NewMessage message);
    }
}

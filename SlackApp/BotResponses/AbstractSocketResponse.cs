using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.RDS;
using SlackAPI.WebSocketMessages;
using SlackAPIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public abstract class AbstractSocketResponse : IMessageReceiver
    {
        protected ISlackClient Client { get; private set; }

        protected AbstractSocketResponse(ISlackClient client) {
            Client = client;
            Client.SubscribeToMessage(MessageReceiver);
        }

        public abstract void MessageReceiver(NewMessage message);

        public abstract void ReloadResponseTriggers();

        public abstract void SaveResponseTrigger<T>( T value);
    }
}

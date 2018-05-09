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
        protected IAmazonDynamoDB DynamoDB { get; private set; }
        protected IAmazonRDS AwsRDS { get; private set; }

        protected AbstractSocketResponse(ISlackClient client,IAmazonDynamoDB dynamoDB = null, IAmazonRDS awsRDS = null) {
            Client = client;
            Client.SubscribeToMessage(MessageReceiver);
            DynamoDB = dynamoDB;
            AwsRDS = awsRDS;
        }

        public abstract void MessageReceiver(NewMessage message);

        public abstract void ReloadResponseTriggers();

        public abstract void SaveResponseTrigger<T>(string key, T value);
    }
}

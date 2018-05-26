using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    interface IMessageReceiver
    {
        void MessageReceiver(NewMessage message);
    }
}

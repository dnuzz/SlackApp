using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public class ModerationResponse : RegexResponse
    {
        Action<NewMessage> moderationAction;
    }
}

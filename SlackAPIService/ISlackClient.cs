using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPIService
{
    public interface ISlackClient
    {
        SlackClient GetSlackClient();

        SlackSocketClient GetSlackSocketClient();

        void SubscribeToMessage(Action<NewMessage> action);

        bool DeleteMessage(NewMessage message);

        void RespondToMessage(NewMessage original, string text, bool ephemeral = false, string botName = null, string parse = null, bool linkNames = false, Attachment[] attachments = null, bool unfurl_links = false, string icon_url = null, string icon_emoji = null, bool as_user = false);
    }
}

using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPIService
{
    [RequestPath("chat.postEphemeral")]
    public class PostEphemeralResponse : Response
    {
        public string ts;
        public string channel;
        public Message message;

        public class Message
        {
            public string text;
            public string user;
            public string username;
            public string type;
            public string subtype;
            public string ts;
        }
    }
}

using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPIService
{
    [RequestPath("chat.postEphemeral")]
    public class PostEphemeralResponse : Response
    {
        public string message_ts;
        public string channel;
    }
}

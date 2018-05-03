using SlackAPIService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.BotResponses
{
    public abstract class AppActionHandler
    {
        protected ISlackClient Client { get; private set; }

        protected AppActionHandler(ISlackClient client) {
            Client = client;
        }
    }
}

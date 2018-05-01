using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlackAPI;
using SlackAPIService;

namespace SlackApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Bot")]
    public class BotController : Controller
    {
        private SlackSocketClient _socketclient;
        private SlackClient _client;

        public BotController(ISlackClient slackClientWrapper)
        {
            this._socketclient = slackClientWrapper.GetSlackSocketClient();
            this._client = slackClientWrapper.GetSlackClient();
        }
    }
}
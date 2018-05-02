using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPIService
{
    public interface ISlackClient
    {
        SlackClient GetSlackClient();
        SlackSocketClient GetSlackSocketClient();
    }
}

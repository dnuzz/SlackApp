using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPIService
{
    public interface ISlackSocketClient
    {
        SlackSocketClient GetSlackSocketClient();
    }
}

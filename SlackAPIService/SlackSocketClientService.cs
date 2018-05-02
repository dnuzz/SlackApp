using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SlackAPIService
{
    public class SlackSocketClientService : ISlackSocketClient
    {
        private SlackSocketClient _socketclient;

        public SlackSocketClientService(string socketauthtoken)
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);

            SlackSocketClient socketclient = new SlackSocketClient(socketauthtoken);
            socketclient.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                if (!connected.ok) { Console.WriteLine(connected.error); }
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected SocketClient to Slack");
            });
            clientReady.Wait();

            this._socketclient = socketclient;
        }

        public SlackSocketClient GetSlackSocketClient()
        {
            return _socketclient;
        }
    }
}

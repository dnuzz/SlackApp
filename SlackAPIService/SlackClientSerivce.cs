using System;
using System.Threading;
using SlackAPI;

namespace SlackAPIService
{
    public class SlackClientService : ISlackClient
    {
        private SlackClient _client;
        private SlackSocketClient _socketclient;

        public SlackClientService(string authtoken,string socketauthtoken = null)
        {
            if (socketauthtoken == null) { socketauthtoken = authtoken; }

                ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            SlackClient client = new SlackClient(authtoken);
            client.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected Client to Slack");
            });
            clientReady.Wait();

            SlackSocketClient socketclient = new SlackSocketClient(socketauthtoken);
            client.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected SocketClient to Slack");
            });
            clientReady.Wait();

            this._socketclient = socketclient;
            this._client = client;
        }

        public SlackClient GetSlackClient()
        {
            return _client;
        }

        public SlackSocketClient GetSlackSocketClient()
        {
            return _socketclient;
        }
    }
}

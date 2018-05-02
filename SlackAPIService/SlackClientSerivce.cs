using System;
using System.Threading;
using SlackAPI;

namespace SlackAPIService
{
    public class SlackClientService : ISlackClient
    {
        private SlackClient _client;

        public SlackClientService(string authtoken)
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            SlackClient client = new SlackClient(authtoken);
            client.Connect((connected) => {
                // This is called once the client has emitted the RTM start command
                if (!connected.ok) { Console.WriteLine(connected.error); }
                clientReady.Set();
            }, () => {
                Console.WriteLine("Connected Client to Slack");
            });
            clientReady.Wait();

            this._client = client;
        }

        public SlackClient GetSlackClient()
        {
            return _client;
        }
    }
}

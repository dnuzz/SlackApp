using System;
using System.Collections.Generic;
using System.Threading;
using SlackAPI;
using SlackAPI.WebSocketMessages;
using Newtonsoft.Json;

namespace SlackAPIService
{
    public class SlackClientService : ISlackClient
    {
        private SlackClient _client;
        private SlackSocketClient _socketclient;

        public SlackClientService(string authtoken,string socketauthtoken)
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            SlackClient client = new SlackClient(authtoken);
            client.Connect((connected) => {
                    if (!connected.ok) { Console.WriteLine(connected.error); }
                    else { Console.WriteLine("Connected Client to Slack"); }
                    clientReady.Set();
                }, () => {});
            clientReady.Wait();

            this._client = client;

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

        public bool DeleteMessage(NewMessage message)
        {
            bool success = false;
            _client.DeleteMessage((x) => { success = x.ok; }, message.channel, message.ts);
            return success;
        }

        public SlackClient GetSlackClient()
        {
            return _client;
        }

        public SlackSocketClient GetSlackSocketClient()
        {
            return _socketclient;
        }

        public void RespondToMessage(NewMessage original, string text, bool ephemeral = false, string botName = null, string parse = null, bool linkNames = false, Attachment[] attachments = null, bool unfurl_links = false, string icon_url = null, string icon_emoji = null, bool as_user = false, string thread_ts = null)
        {
            if (ephemeral)
            {
                try
                {
                    _socketclient.PostEphemeralMessage(null, original.channel, text, original.user, parse, linkNames, attachments, as_user, thread_ts);
                    return;
                }
                catch (Exception e) { }

                List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();

                parameters.Add(new Tuple<string, string>("channel", original.channel));
                parameters.Add(new Tuple<string, string>("text", text));
                parameters.Add(new Tuple<string, string>("user", original.user));

                if (!string.IsNullOrEmpty(parse))
                    parameters.Add(new Tuple<string, string>("parse", parse));

                if (linkNames)
                    parameters.Add(new Tuple<string, string>("link_names", "1"));

                if (attachments != null && attachments.Length > 0)
                    parameters.Add(new Tuple<string, string>("attachments",
                        JsonConvert.SerializeObject(attachments, Formatting.None,
                                new JsonSerializerSettings // Shouldn't include a not set property
                            {
                                    NullValueHandling = NullValueHandling.Ignore
                                })));

                if (unfurl_links)
                    parameters.Add(new Tuple<string, string>("unfurl_links", "1"));

                parameters.Add(new Tuple<string, string>("as_user", as_user.ToString()));

                _client.APIRequestWithToken<PostEphemeralResponse>(null,parameters.ToArray());
            }
            else
            {
                _socketclient.PostMessage(null, original.channel, text, botName, parse, linkNames, attachments, unfurl_links, icon_url, icon_emoji, as_user, thread_ts);
            }
        }

        public void SubscribeToMessage(Action<NewMessage> action)
        {
            _socketclient.OnMessageReceived += action;
        }
    }
}

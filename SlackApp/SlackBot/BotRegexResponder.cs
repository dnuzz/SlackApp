using SlackAPI;
using SlackAPI.WebSocketMessages;
using SlackApp.SlackBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackApp.Controllers
{
    public class BotRegexResponder
    {
        public static Dictionary<string, Action<NewMessage,string>> methodMap = new Dictionary<string, Action<NewMessage,string>>();
        public static List<RegexResponse> regexResponses = new List<RegexResponse>();
        private SlackSocketClient client;
        private SlackClient normalClient;
        private List<Tuple<string, string, DateTime>> botCoolDowns = new List<Tuple<string, string, DateTime>>();
        private DateTime botCooldownEnd = DateTime.UtcNow;

        public BotRegexResponder(SlackSocketClient client,SlackClient normalClient)
        {
            this.normalClient = normalClient;
            this.client = client;
            client.OnMessageReceived += (message) => { this.Receiver(message); };
            client.OnMessageReceived += (message) => { Console.WriteLine(message.text); }; //Remove this later
            methodMap.Add(@"([+-]?(\d+\.?\d+?)+)\s?°?[Cc]", (m,s) => { TemperatureConversion(m,s); });
            methodMap.Add(@"([+-]?(\d+\.?\d?)+)\s?([kK]m)|([kK]ilometers)", (m,s) => { DistanceConversion(m,s); });
            regexResponses.Add(new RegexResponse { Regex = @"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)+", Response = "stop that", DeleteMessage = true });
            regexResponses.Add(new RegexResponse { Regex = @"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)", Response = "stop that", DeleteMessage = true });
        }

        public static void AddRegexResponse(RegexResponse regexResp)
        {
            regexResponses.Add(regexResp);
        }

        public void Receiver(NewMessage message)
        {
            if (message.subtype == "bot_message")
            {
                var bot_channel = botCoolDowns
                    .Where((x) => { return x.Item1 == message.user && x.Item2 == message.channel; })
                    .DefaultIfEmpty(new Tuple<string, string, DateTime>(String.Empty, string.Empty, DateTime.MinValue))
                    .FirstOrDefault();

                if (bot_channel.Item1 == message.user && bot_channel.Item2 == message.channel && bot_channel.Item3 > DateTime.UtcNow)
                {
                    normalClient.DeleteMessage((m) =>
                        {
                            if (!m.ok)
                            {
                                Console.WriteLine(m.error);
                            }
                        },
                        message.channel,
                        message.ts);
                    return;
                } else
                {
                    botCoolDowns.Remove(bot_channel);
                    botCoolDowns.Add(new Tuple<string, string, DateTime>(message.user, message.channel, DateTime.UtcNow.AddSeconds(180.0)));
                }
            }

            foreach (var m in methodMap)
            {
                if (Regex.IsMatch(message.text,m.Key))
                {
                    try
                    {
                        m.Value.Invoke(message,m.Key);
                    }
                    catch (Exception e) { }
                }
            }
            foreach (var r in regexResponses)
            {
                if (Regex.IsMatch(message.text, r.Regex,RegexOptions.IgnoreCase))
                {
                    client.PostMessage(null, message.channel, r.Response);
                    if( r.DeleteMessage)
                    {
                        normalClient.DeleteMessage((m) => {
                            if (!m.ok)
                            {
                                Console.WriteLine(m.error);
                            } },
                            message.channel,
                            message.ts);
                    }
                }
            }
        }

        private void TemperatureConversion(NewMessage message, string regex)
        {
            var matching = Regex.Match(message.text, regex);
            var groups = new List<string>();
            foreach(Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) * 9.0 / 5.0 + 32} in farenheit";

            client.PostMessage(null, message.channel, replacement);
        }

        private void DistanceConversion(NewMessage message,string regex)
        {
            var matching = Regex.Match(message.text, regex);
            var groups = new List<string>();
            foreach (Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) / 0.621371} in miles";

            client.PostMessage(null, message.channel, replacement);
        }
    }
}

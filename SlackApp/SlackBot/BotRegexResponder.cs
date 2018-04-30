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

        public BotRegexResponder(SlackSocketClient client)
        {
            this.client = client;
            client.OnMessageReceived += (message) => { this.Receiver(message); };
            client.OnMessageReceived += (message) => { Console.WriteLine(message.text); }; //Remove this later
            methodMap.Add(@"([+-]?(\d+\.?\d+?)+)\s?°?[Cc]", (m,s) => { TemperatureConversion(m,s); });
            methodMap.Add(@"([+-]?(\d+\.?\d?)+)\s?([kK]m)|([kK]ilometers)", (m,s) => { DistanceConversion(m,s); });
        }

        public static void AddRegexResponse(RegexResponse regexResp)
        {
            regexResponses.Add(regexResp);
        }

        public void Receiver(NewMessage message)
        {
            foreach(var m in methodMap)
            {
                if (Regex.IsMatch(message.text,m.Key) && message.subtype != "bot_message")
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
                if (Regex.IsMatch(message.text, r.Regex) && message.subtype != "bot_message")
                {
                    client.PostMessage(null, message.channel, r.Response);
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

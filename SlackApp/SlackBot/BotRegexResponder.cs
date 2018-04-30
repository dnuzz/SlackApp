using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackApp.Controllers
{
    public class BotRegexResponder
    {
        public Dictionary<string, Action<NewMessage>> methodMap = new Dictionary<string, Action<NewMessage>>();
        private SlackSocketClient client;

        public BotRegexResponder(SlackSocketClient client)
        {
            this.client = client;
            client.OnMessageReceived += (message) => { this.Receiver(message); };
            client.OnMessageReceived += (message) => { Console.WriteLine(message.text); };
            methodMap.Add("([+-]?\\d+(\\.\\d+)*)\\s?°?([Cc]|(Celcius))", (m) => { TemperatureConversion(m); });
        }

        public void Receiver(NewMessage message)
        {
            foreach(var m in methodMap)
            {
                if (Regex.IsMatch(message.text,m.Key) && message.subtype != "bot_message")
                {
                    m.Value.Invoke(message);
                }
            }
        }

        private void TemperatureConversion(NewMessage message)
        {
            var matching = Regex.Match(message.text, "([+-]?\\d+(\\.\\d+)*)\\s?°[Cc]");
            var groups = new List<string>();
            foreach(Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) * 9.0 / 5.0 + 32} in Farenheit";

            var responseText = String.Format("That is {0} in Farenheit", Regex.Replace(message.text, "([+-]?\\d+(\\.\\d+)*)\\s?°[Cc]", delegate (Match match)
            {
                double temp = Double.Parse(match.Groups[1].ToString());
                return (temp * 9.0 / 5.0 + 32).ToString();
            }));
            client.PostMessage(null, message.channel, replacement);
        }
    }
}

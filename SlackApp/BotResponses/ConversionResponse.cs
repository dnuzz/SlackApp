using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackAPI.WebSocketMessages;
using SlackAPIService;

namespace SlackApp.BotResponses
{
    public class ConversionResponse : AbstractSocketResponse
    {
        public ConversionResponse(ISlackClient client) : base(client)
        {
        }

        public override void MessageReceiver(NewMessage message)
        {
            this.TemperatureConversion(message, @"([+-]?(\d+\.?\d+?)+)\s?°?(c\s)|(celcius)");
            this.DistanceConversion(message, @"([+-]?(\d+\.?\d?)+)\s?(km)|(kilometers)");
            this.MassConversion(message, @"([+-]?(\d+\.?\d?)+)\s?(kg)|(kilograms)");
        }

        private void TemperatureConversion(NewMessage message, string regex)
        {
            var matching = Regex.Match(message.text, regex, RegexOptions.IgnoreCase);
            var groups = new List<string>();
            foreach (Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) * 9.0 / 5.0 + 32} in farenheit";

            Client.RespondToMessage(null, replacement);
        }

        private void DistanceConversion(NewMessage message, string regex)
        {
            var matching = Regex.Match(message.text, regex, RegexOptions.IgnoreCase);
            var groups = new List<string>();
            foreach (Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) * 0.621371} in miles";

            Client.RespondToMessage(null, replacement);
        }

        private void MassConversion(NewMessage message, string regex)
        {
            var matching = Regex.Match(message.text, regex, RegexOptions.IgnoreCase);
            var groups = new List<string>();
            foreach (Group g in matching.Groups)
            {
                groups.Add(g.Value);
            }

            var replacement = $"That is {Double.Parse(groups[1]) / 2.2046} in pounds";

            Client.RespondToMessage(null, replacement);
        }
    }
}

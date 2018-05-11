using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.RDS;
using SlackAPI.WebSocketMessages;
using SlackAPIService;

namespace SlackApp.BotResponses
{
    public class ConversionResponse : AbstractSocketResponse
    {

        public ConversionResponse(ISlackClient client, IAmazonRDS rdsDB) : base(client, null, rdsDB)
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
            var number = RegexFilter(message, regex);
            if (String.IsNullOrEmpty(number)) { return; }

            var replacement = $"That is {Double.Parse(number) * 9.0 / 5.0 + 32} in farenheit";

            Client.RespondToMessage(message, replacement);
        }

        private void DistanceConversion(NewMessage message, string regex)
        {
            var number = RegexFilter(message, regex);
            if (String.IsNullOrEmpty(number)) { return; }

            var replacement = $"That is {Double.Parse(number) * 0.621371} in miles";

            Client.RespondToMessage(message, replacement);
        }

        private void MassConversion(NewMessage message, string regex)
        {
            var number = RegexFilter(message, regex);
            if (String.IsNullOrEmpty(number)) { return; }

            var replacement = $"That is {Double.Parse(number) * 2.2046} in pounds";

            Client.RespondToMessage(message, replacement);
        }

        public override void ReloadResponseTriggers()
        {
            throw new NotImplementedException();
        }

        public override void SaveResponseTrigger<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        private string RegexFilter(NewMessage message, string regex)
        {
            try
            {
                if (!Regex.IsMatch(message.text, regex, RegexOptions.IgnoreCase))
                {
                    return null;
                }
                var matching = Regex.Match(message.text, regex, RegexOptions.IgnoreCase);
                var groups = new List<string>();
                foreach (Group g in matching.Groups)
                {
                    groups.Add(g.Value);
                }
                return groups[1];
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}

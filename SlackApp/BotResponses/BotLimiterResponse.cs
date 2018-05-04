using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackAPI.WebSocketMessages;
using SlackAPIService;

namespace SlackApp.BotResponses
{
    public class BotLimiterResponse : AbstractSocketResponse
    {
        public List<BotCooldownEntry> BotCooldowns = new List<BotCooldownEntry>();

        public BotLimiterResponse(ISlackClient client) : base(client)
        {
        }

        public override void MessageReceiver(NewMessage message)
        {
            if (message.subtype == "bot_message" && !message.channel.StartsWith('D'))
            {
                var bot_channel = BotCooldowns
                    .Where((x) => { return x.Botname == message.user && x.Channel == message.channel; })
                    .DefaultIfEmpty(new BotCooldownEntry(String.Empty, string.Empty, DateTime.MinValue,TimeSpan.MinValue))
                    .FirstOrDefault();

                if (bot_channel.Botname == message.user && bot_channel.Channel == message.channel && bot_channel.CooldownEnd > DateTime.UtcNow)
                {
                    Client.DeleteMessage(message);
                }
                else
                {
                    BotCooldowns.Remove(bot_channel);
                    BotCooldowns.Add(new BotCooldownEntry(message.user, message.channel, DateTime.UtcNow + bot_channel.CoolDown, bot_channel.CoolDown));
                }
            }
        }
    }

    public class BotCooldownEntry
    {
        public BotCooldownEntry(string botname, string channel, DateTime cooldownEnd, TimeSpan coolDown)
        {
            Botname = botname;
            Channel = channel;
            CooldownEnd = cooldownEnd;
            CoolDown = coolDown;
        }

        public string Botname { get; set; }
        public string Channel { get; set; }
        public DateTime CooldownEnd { get; set; }
        public TimeSpan CoolDown { get; set; }
    }
}

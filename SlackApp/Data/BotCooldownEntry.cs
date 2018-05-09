using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Data
{
    public class BotCooldownEntry : AttributeValue
    {
        public static TimeSpan DefaultCooldown = new TimeSpan(0, 2, 0);

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

        public override bool Equals(object obj)
        {
            var entry = obj as BotCooldownEntry;
            return entry != null &&
                   Botname == entry.Botname &&
                   Channel == entry.Channel;
        }

        public override int GetHashCode()
        {
            var hashCode = 157315657;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Botname);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Channel);
            return hashCode;
        }
    }
}

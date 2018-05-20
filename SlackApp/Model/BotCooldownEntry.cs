using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Model
{
    public class BotCooldownEntry
    {
        public static TimeSpan DefaultCooldown = new TimeSpan(0, 2, 0);

        public BotCooldownEntry() { }

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

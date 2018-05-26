using SlackApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppResponseContext context)
        {
            context.Database.EnsureCreated();

            if (context.regexResponses.Any())
            {
                return;   // DB has been seeded
            }

            var cooldowns = new BotCooldownEntry[]
            {
                new BotCooldownEntry(String.Empty, string.Empty, DateTime.MinValue, BotCooldownEntry.DefaultCooldown)
            };
            foreach (var s in cooldowns)
            {
                context.botCooldowns.Add(s);
            }
            context.SaveChanges();

            var deletes = new DeleteResponseEntry[]
            {
                new DeleteResponseEntry(@"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)+"),
                new DeleteResponseEntry(@"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)"),
                new DeleteResponseEntry(@"(schl...)|(fourth leg)|(fifth leg)")
            };
            foreach (var d in deletes)
            {
                context.deleteResponses.Add(d);
            }
            context.SaveChanges();

            var regresponse = new RegexResponseEntry[]
            {
                new RegexResponseEntry(@"(ur|y..r|'s)\s*(m.m|m..h.r|m.t.rnal)","No mom jokes allowed",true),
                new RegexResponseEntry(@"(m.m|m..h.r|m.t.rnal)'?s (box|face|butt|ass|cunt)", "Stop being crude", true),
                new RegexResponseEntry(@"(schl...)|(fourth leg)|(fifth leg)", "We really don't care how tiny it is", true)
            };
            foreach (var r in regresponse)
            {
                context.regexResponses.Add(r);
            }
            context.SaveChanges();
        }
    }
}

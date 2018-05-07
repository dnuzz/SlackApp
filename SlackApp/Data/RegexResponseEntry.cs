using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Data
{
    public class RegexResponseEntry : AttributeValue
    {
        public RegexResponseEntry(string regex, string response, bool ephemeral)
        {
            Regex = regex;
            Response = response;
            Ephemeral = ephemeral;
        }

        public string Regex { get; set; }
        public string Response { get; set; }
        public bool Ephemeral { get; set; }

        public override bool Equals(object obj)
        {
            var entry = obj as RegexResponseEntry;
            return entry != null &&
                   Regex == entry.Regex &&
                   Ephemeral == entry.Ephemeral;
        }

        public override int GetHashCode()
        {
            var hashCode = 432919396;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Regex);
            hashCode = hashCode * -1521134295 + Ephemeral.GetHashCode();
            return hashCode;
        }
    }
}

using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Model
{
    public class RegexResponseEntry
    {
        public RegexResponseEntry() { }

        public RegexResponseEntry(string regex, string response, bool ephemeral)
        {
            Regex = regex;
            Response = response;
            Ephemeral = ephemeral;
        }
        
        public string Regex { get; set; }
        public string Response { get; set; }
        public bool Ephemeral { get; set; }
    }
}

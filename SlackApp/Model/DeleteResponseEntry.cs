using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SlackApp.Model
{
    public class DeleteResponseEntry
    {
        public DeleteResponseEntry()
        {
        }

        public DeleteResponseEntry(string regex)
        {
            Regex = regex;
        }

        public string Regex {get; set;}
    }
}

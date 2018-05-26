using Microsoft.EntityFrameworkCore;
using SlackApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackApp.Data
{
    public class AppResponseContext : DbContext
    {
        public AppResponseContext(DbContextOptions<AppResponseContext> options) : base(options)
        {

        }

        public AppResponseContext() : base()
        {

        }

        public DbSet<BotCooldownEntry> botCooldowns { get; set; }
        public DbSet<DeleteResponseEntry> deleteResponses { get; set; }
        public DbSet<RegexResponseEntry> regexResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotCooldownEntry>()
                .HasKey(c => new { c.Botname, c.Channel });
            modelBuilder.Entity<DeleteResponseEntry>()
                .HasKey(c => new { c.Regex });
            modelBuilder.Entity<RegexResponseEntry>()
                .HasKey(c => new { c.Regex });
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using ProgrammingClub.Models;
using ProgrammingClub.Models.CreateModels;
using System.Collections.Generic;

namespace ProgrammingClub.DataContext
{
    public class ProgrammingClubDataContext : DbContext
    {
        public ProgrammingClubDataContext(DbContextOptions<ProgrammingClubDataContext> options) : base(options) { }
      
        public DbSet<Member> Members { get; set; }
        public DbSet<CreateMember> CreateMembers { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Moderator> Moderators{ get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventType { get; set; } 
        public DbSet<PricingModel> PricingModels { get; set; }
        public DbSet<EventsParticipant> EventsParticipants { get; set; }

        public DbSet<Dropout> Dropouts { get; set; }
    }
}
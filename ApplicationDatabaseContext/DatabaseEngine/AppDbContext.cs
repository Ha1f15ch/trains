﻿using DatabaseEngine.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEngine
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<NewsChannel> NewsChannels { get; set; }
        public DbSet<NewsChannelsPosts> NewsChannelsPosts { get; set; }
        public DbSet<NewsChannelsSubscribers> NewsChannelsSubscribers { get; set; }
    }
}

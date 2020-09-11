using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SWENG894.Models;

namespace SWENG894.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FriendRequest>()
                .HasKey(t => new { t.RequestedById, t.RequestedForId });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(a => a.RequestedBy)
                .WithMany(b => b.SentFriendRequests)
                .HasForeignKey(c => c.RequestedById);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(a => a.RequestedFor)
                .WithMany(b => b.ReceievedFriendRequests)
                .HasForeignKey(c => c.RequestedForId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Workout> Workouts { get; set; }
    }
}

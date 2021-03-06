using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;
using SWENG894.Models;
using SWENG894.ViewModels;

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

            // FriendRequest setup
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

            // Messages setup
            modelBuilder.Entity<Message>()
                .HasOne(a => a.SentBy)
                .WithMany(b => b.SentMessages)
                .HasForeignKey(c => c.SentById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(a => a.SentTo)
                .WithMany(b => b.ReceievedMessages)
                .HasForeignKey(c => c.SentToId)
                .OnDelete(DeleteBehavior.NoAction);

            // WorkoutFavorites setup
            modelBuilder.Entity<WorkoutFavorite>()
                .HasKey(t => new { t.UserId, t.WorkoutId });

            // Challenges setup
            modelBuilder.Entity<Challenge>()
                .HasOne(c => c.Contender)
                .WithMany()
                .HasForeignKey(d => d.ContenderId)
                .OnDelete(DeleteBehavior.NoAction);


            //workout results

            //modelBuilder.Entity<WorkoutResults>().HasKey(t => new { t.UserId, t.WorkoutId });



        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<WorkoutResult> WorkoutResults { get; set; }
        public DbSet<NewsFeed> NewsFeed { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<WorkoutFavorite> WorkoutFavorites { get; set; }

        public DbSet<SWENG894.Models.Ranking> Ranking { get; set; }

    }
}

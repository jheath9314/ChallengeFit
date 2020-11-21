using Microsoft.EntityFrameworkCore;
using SWENG894.Data;
using SWENG894.Data.Repository;
using SWENG894.Data.Repository.IRepository;
using SWENG894.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace SWENG894.Test.Data.RepositoryTest
{
    public class RankingRepositoryTest
    {
        private readonly ApplicationDbContext _context;
        private readonly IRankingRepository _cut;

        public RankingRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "DbInMemoryRanking");

            var dbContextOptions = builder.Options;
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _cut = new RankingRepository(_context);
        }

        [Fact]
        public async void RankingRepositoryUpdateTest()
        {
            var newRanking = new SWENG894.Models.Ranking();
            string datetime = DateTime.UtcNow.ToString();
            newRanking.Id = 50;
            newRanking.BronzeValue = 0;
            newRanking.SilverValue = 1;
            newRanking.GoldValue = 2;
            newRanking.PlatinumValue = 3;
            newRanking.DiamondValue = 4;
            newRanking.Timestamp = datetime;

            await _cut.AddAsync(newRanking);
            await _context.SaveChangesAsync();

            var ranking = _cut.GetRankings();
            Assert.Equal(50, ranking.Id);
            Assert.Equal(0, ranking.BronzeValue);
            Assert.Equal(1, ranking.SilverValue);
            Assert.Equal(2, ranking.GoldValue);
            Assert.Equal(3, ranking.PlatinumValue);
            Assert.Equal(4, ranking.DiamondValue);
            Assert.Equal(datetime, ranking.Timestamp);

            ranking.DiamondValue = 1500;
            _cut.UpdateAsync(ranking);

            Thread.Sleep(100);

            ranking = _cut.GetRankings();

            Assert.Equal(50, ranking.Id);
            Assert.Equal(0, ranking.BronzeValue);
            Assert.Equal(1, ranking.SilverValue);
            Assert.Equal(2, ranking.GoldValue);
            Assert.Equal(3, ranking.PlatinumValue);
            Assert.Equal(1500, ranking.DiamondValue);
            Assert.Equal(datetime, ranking.Timestamp);
        }
    }
}

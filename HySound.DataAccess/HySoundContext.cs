using HySound.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.DataAccess
{
    public class HySoundContext : IdentityDbContext<IdentityUser>
    {
        public HySoundContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<TrackStatistic> TrackStatistics { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Playlist>()
                .HasOne(u=>u.User)
                .WithMany(p=>p.Playlists)
                .HasForeignKey(u=>u.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PlaylistTrack>()
           .HasKey(f => new { f.PlaylistId, f.TrackId });

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(p=>p.Playlist)
                .WithMany(pt=>pt.PlaylistTracks)
                .HasForeignKey(p=>p.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(t => t.Track)
                .WithMany(t => t.PlaylistTracks)
                .HasForeignKey(t => t.TrackId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Follower>()
           .HasKey(f => new { f.FollowedId, f.FollowingId });

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowedUser)
                .WithMany(f => f.Following)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowingUser)
                .WithMany(f => f.Followers)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.User)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                 .HasOne(g => g.Genre)
                 .WithMany(t => t.Tracks)
                 .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Track)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TrackId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<TrackStatistic>()
                .HasOne(ts => ts.Track)
                .WithOne(t => t.TrackStatistic)
                .HasForeignKey<TrackStatistic>(ts => ts.TrackId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Comment>()
                .HasOne(u => u.User)
                .WithMany(c => c.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.User)
                .WithMany(a => a.Albums)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrackStatistic>()
                .Property(ts => ts.Plays)
                .HasDefaultValue(0);

            modelBuilder.Entity<Like>()
                .HasOne(x => x.Track)
                .WithMany(l => l.Likes)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
              .HasOne(x => x.User)
              .WithMany(l => l.Likes)
              .HasForeignKey(x => x.UserId)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(c => c.UserIdentity)
                .WithOne()
                .HasForeignKey<User>(c => c.UserIdentityId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public async Task Seed()
        {
            if (!Genres.Any())
            {
                Genres.AddRange(
                    new Genre { Name = "Pop"},
                    new Genre { Name = "Rock" },
                    new Genre { Name = "Hip-Hop / Rap" },
                    new Genre { Name = "Electronic / Dance (EDN)" },
                    new Genre { Name = "R&B / Soul" },
                    new Genre { Name = "Country" },
                    new Genre { Name = "Jazz" },
                    new Genre { Name = "Classical" },
                    new Genre { Name = "Reggae" },
                    new Genre { Name = "Blues" },
                    new Genre { Name = "Folk" },
                    new Genre { Name = "Latin" },
                    new Genre { Name = "Metal" },
                    new Genre { Name = "Punk" },
                    new Genre { Name = "Indie / Alternative" },
                    new Genre { Name = "Trap" },
                    new Genre { Name = "Dubstep" },
                    new Genre { Name = "Techno" },
                    new Genre { Name = "K-Pop" },
                    new Genre { Name = "Chillwave" },
                    new Genre { Name = "Lo-fi" },
                    new Genre { Name = "Balkan Beats" }
                );
                await SaveChangesAsync();
            }
        }
    }
}

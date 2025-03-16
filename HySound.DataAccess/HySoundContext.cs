using HySound.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HySound.DataAccess
{
    public class HySoundContext : IdentityDbContext<IdentityUser>
    {
        IServiceProvider serviceProvider;

        public HySoundContext(DbContextOptions options,
            IServiceProvider _serviceProvider) : base(options)
        {
            serviceProvider = _serviceProvider;
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Followed> Followers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<ArtistRequest> ArtistRequests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArtistRequest>()
                .HasOne(u => u.User)
                .WithMany(ar => ar.ArtistRequests)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ArtistRequest>()
                .HasOne(u => u.Admin)
                .WithMany()
                .HasForeignKey(u => u.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Playlist>()
                .HasOne(u => u.User)
                .WithMany(p => p.Playlists)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PlaylistTrack>()
           .HasKey(f => new { f.PlaylistId, f.TrackId });

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(p => p.Playlist)
                .WithMany(pt => pt.PlaylistTracks)
                .HasForeignKey(p => p.PlaylistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(t => t.Track)
                .WithMany(t => t.PlaylistTracks)
                .HasForeignKey(t => t.TrackId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Followed>()
           .HasKey(f => new { f.FollowedId, f.FollowedById });

            modelBuilder.Entity<Followed>()
                .HasOne(x => x.FollowedByUser)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.FollowedById)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Followed>()
                .HasOne(x => x.FollowedUser)
                .WithMany(x => x.FollowedBy)
                .HasForeignKey(x => x.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Like>()
                .HasOne(x => x.Track)
                .WithMany(l => l.Likes)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
                .HasOne(x => x.Album)
                .WithMany(l => l.Likes)
                .HasForeignKey(x => x.AlbumId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
                .HasOne(x => x.Playlist)
                .WithMany(l => l.Likes)
                .HasForeignKey(x => x.PlaylistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
              .HasOne(x => x.User)
              .WithMany(l => l.Likes)
              .HasForeignKey(x => x.UserId)
              .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(x => x.UserIdentity)
                .WithOne()
                .HasForeignKey<User>(x => x.UserIdentityId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public async Task Seed()
        {
            if (!Genres.Any())
            {
                Genres.AddRange(
                    new Genre { Name = "Pop" },
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


            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminEmail = "admin@admin.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new IdentityUser { UserName = "admin@admin.com", Email = adminEmail };

                var result = await userManager.CreateAsync(user, "Admin123!");

                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(user, "Admin");
                    User user1 = new User()
                    {
                        Username = "Admin",
                        Email = "admin@admin.com",
                        Password = "Admin123!",
                        UserIdentityId = user.Id,
                        UserIdentity = user
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();
                }

            }

        }
    }



}

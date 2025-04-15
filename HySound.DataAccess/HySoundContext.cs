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
                .OnDelete(DeleteBehavior.Cascade);

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
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Followed>()
                .HasOne(x => x.FollowedUser)
                .WithMany(x => x.FollowedBy)
                .HasForeignKey(x => x.FollowedId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.User)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                 .HasOne(g => g.Genre)
                 .WithMany(t => t.Tracks)
                 .OnDelete(DeleteBehavior.NoAction);


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
                .OnDelete(DeleteBehavior.NoAction);

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


            modelBuilder.Entity<User>()
                .HasMany(x => x.Likes)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .HasOne(x => x.UserIdentity)
                .WithOne()
                .HasForeignKey<User>(x => x.UserIdentityId)
                .OnDelete(DeleteBehavior.NoAction);
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
                var userA = new IdentityUser { UserName = "admin@admin.com", Email = adminEmail };

                var resultA = await userManager.CreateAsync(userA, "Admin123!");

                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                if (!await roleManager.RoleExistsAsync("Artist"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Artist"));
                }


                if (resultA.Succeeded)
                {

                    await userManager.AddToRoleAsync(userA, "Admin");
                    User user1 = new User()
                    {
                        Username = "Admin",
                        Email = "admin@admin.com",
                        Password = "Admin123!",
                        UserIdentityId = userA.Id,
                        UserIdentity = userA
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();
                }

            }

            if (Users.Count() <= 1)
            {


                var user = new IdentityUser { UserName = "carti@abv.bg", Email = "carti@abv.bg" };
                var result = await userManager.CreateAsync(user, "Carti123!");


                var user2 = new IdentityUser { UserName = "yeat@abv.bg", Email = "yeat@abv.bg" };
                var result2 = await userManager.CreateAsync(user2, "Yeat123!");


                var user3 = new IdentityUser { UserName = "thug@abv.bg", Email = "thug@abv.bg" };
                var result3 = await userManager.CreateAsync(user3, "Thug123!");


                var user4 = new IdentityUser { UserName = "kencarson@abv.bg", Email = "kencarson@abv.bg" };
                var result4 = await userManager.CreateAsync(user4, "Carson123!");


                var user5 = new IdentityUser { UserName = "guns@abv.bg", Email = "guns@abv.bg" };
                var result5 = await userManager.CreateAsync(user5, "Guns123!");

                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync("Artist"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Artist"));
                }

                if (result.Succeeded)
                {

                    await userManager.AddToRoleAsync(user, "Artist");
                    User user1 = new User()
                    {
                        Username = "Playboi Carti",
                        Email = "carti@abv.bg",
                        Password = "Carti123!",
                        UserIdentityId = user.Id,
                        UserIdentity = user,
                        ProfilePicture= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744657955/rjs2vurjep5jii46caqa.jpg"
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();
                    Tracks.AddRange(
                    new Track() { Title = "Vamp Anthem", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744656988/HySound/Tracks/iebmvty3rpadugi7d5sa.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744656986/cnlfjdf9upzvf9vvjeak.jpg" },
                    new Track() { Title = "EvilJordan", IsYoutube = true, AudioUrl = "VcRc2DHHhoM", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744500537/ym7urjll9uxysp4xscwc.jpg" },
                    new Track() { Title = "FOMDJ", IsYoutube = true, AudioUrl = "HDTo1Pieha0", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744500537/ym7urjll9uxysp4xscwc.jpg" },
                    new Track() { Title = "Rather Lie", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744500643/HySound/Tracks/o3bzrzpprfzld6zlux1v.mp3", UserId = user1.Id, GenreId = 3, CoverImage = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744500537/ym7urjll9uxysp4xscwc.jpg" },
                    new Track() { Title = "ALL RED", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744500883/HySound/Tracks/xa0bktsqw6hh1qvhgslw.mp3", UserId = user1.Id, GenreId = 3, CoverImage = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744020245/gylvmkwakjscfnwf4brl.jpg" }

                    );
                }


     
                if (result2.Succeeded)
                {

                    await userManager.AddToRoleAsync(user2, "Artist");
                    User user1 = new User()
                    {
                        Username = "Yeat",
                        Email = "yeat@abv.bg",
                        Password = "Yeat123!",
                        UserIdentityId = user2.Id,
                        UserIdentity = user2,
                        ProfilePicture = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744657555/jtehrtcgeeuacdlc3duu.jpg"
                    };
                    Users.Add(user1);

                    await SaveChangesAsync();

                    Tracks.AddRange(
                          new Track() { Title = "ILUV", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744657378/HySound/Tracks/iukgt3uxupiqncl1fsiz.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744657374/f5moe7jbrokgzn5yjtqc.jpg" },
                          new Track() { Title = "Swerved It", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744657330/HySound/Tracks/zxpd4psx4j3d2veke39m.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744657327/ifmtwrz08of9neyo91le.jpg" }
                        );
                }

                if (result3.Succeeded)
                {

                    await userManager.AddToRoleAsync(user3, "Artist");
                    User user1 = new User()
                    {
                        Username = "Young Thug",
                        Email = "thug@abv.bg",
                        Password = "Thug123!",
                        UserIdentityId = user3.Id,
                        UserIdentity = user3,
                        ProfilePicture = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744501404/zmqzahfvewlku2nbvuh0.jpg"
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();

                    Tracks.AddRange(
                         new Track() { Title = "Lil Baby", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744501291/HySound/Tracks/uxsge5xgmdd9l5rzdxc8.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744501361/i1tmic3zim9brc3ibqtl.png" },
                         new Track() { Title = "Millions", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744501363/HySound/Tracks/bvnidbwigumniynzb3hv.mp3", UserId = user1.Id, GenreId = 3 , CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744501361/i1tmic3zim9brc3ibqtl.png" }
                    );
                }


                if (result4.Succeeded)
                {

                    await userManager.AddToRoleAsync(user4, "Artist");
                    User user1 = new User()
                    {
                        Username = "KenCarson",
                        Email = "kencarson@abv.bg",
                        Password = "Carson123!",
                        UserIdentityId = user4.Id,
                        UserIdentity = user4,
                        ProfilePicture = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744655904/a4am6dg88dpdeaguqe34.jpg"
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();

                    Tracks.AddRange(
                         new Track() { Title = "LiveLeak", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744655825/HySound/Tracks/yyr5ajsicgzfxb5kjs9s.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744655823/fnhfzsfz7axjecbn6bvh.jpg" },
                         new Track() { Title = "Diamnods", IsYoutube = false, AudioUrl = "https://res.cloudinary.com/ddcaeo5xq/raw/upload/v1744655907/HySound/Tracks/up9tgofq9aa7yvngxmoe.mp3", UserId = user1.Id, GenreId = 3, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744655823/fnhfzsfz7axjecbn6bvh.jpg" }
                    );
                }


                if (result5.Succeeded)
                {

                    await userManager.AddToRoleAsync(user5, "Artist");
                    User user1 = new User()
                    {
                        Username = "Guns N' Roses",
                        Email = "guns@abv.bg",
                        Password = "Guns123!",
                        UserIdentityId = user5.Id,
                        UserIdentity = user5,
                        ProfilePicture = "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744656008/lixyb5iipsyybjudsavz.jpg"
                    };

                    Users.Add(user1);
                    await SaveChangesAsync();

                    Tracks.AddRange(
                         new Track() { Title = "November Rain", IsYoutube = true, AudioUrl = "8SbUC-UaAxE", UserId = user1.Id, GenreId = 13, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744656008/lixyb5iipsyybjudsavz.jpg" },
                         new Track() { Title = "Don't Cry", IsYoutube = true, AudioUrl = "zRIbf6JqkNc", UserId = user1.Id, GenreId = 13, CoverImage= "https://res.cloudinary.com/ddcaeo5xq/image/upload/v1744656008/lixyb5iipsyybjudsavz.jpg" }
                    );
                }

                await SaveChangesAsync();
            }



        }
    }



}

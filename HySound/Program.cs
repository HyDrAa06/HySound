﻿using HySound.DataAccess;
using HySound.DataAccess.Repository.IRepository;
using HySound.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using HySound.Core.Service.IService;
using HySound.Core.Service;
using Microsoft.AspNetCore.Identity;
using CloudinaryDotNet;
using System.Text.Json.Serialization;
using System.Text.Json;
using HySound.Models.Models;

var builder = WebApplication.CreateBuilder(args);


MemoryStream stream = new MemoryStream();
IFormFile formFile = new FormFile(stream, 0, 100, "file", "test.mp3");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HySoundContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     b => b.MigrationsAssembly("HySound.DataAccess")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<HySoundContext>()
.AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IFollowerService, FollowerService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IArtistRequestService, ArtistRequestService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudinaryAccount = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);

var cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HySoundContext>();
    await dbContext.Seed();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

static async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User", "Artist" };

    foreach (var roleName in roleNames)
    {

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));

        }
    }
}

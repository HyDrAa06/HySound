using HySound.DataAccess;
using HySound.DataAccess.Repository.IRepository;
using HySound.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using HySound.Core.Service.IService;
using HySound.Core.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HySoundContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     b => b.MigrationsAssembly("HySound.DataAccess")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HySoundContext>();
    await dbContext.Seed();
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Track}/{action=AllTracks}/{id?}");

app.Run();

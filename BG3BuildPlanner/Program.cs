using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("BG3BuildDb"));

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);

    Console.WriteLine("=== BG3BuildPlanner: Sample LINQ Queries ===");

    var builds = context.Builds
        .WithDetails();

    Console.WriteLine();
    Console.WriteLine("Top builds by average rating (min 1 rating):");
    foreach (var row in builds.TopByAverageRating(take: 5, minRatings: 1).SelectRatingStats().ToList())
    {
        Console.WriteLine($"- {row.Title} | {row.CharacterName} | {row.Difficulty} | avg={row.AverageScore:0.00} ({row.RatingCount} ratings)");
    }

    Console.WriteLine();
    Console.WriteLine("All builds for character 'Astarion':");
    foreach (var build in builds.ForCharacter("Astarion").OrderByNewest().ToList())
    {
        var avg = build.Ratings.Select(r => (double?)r.Score).Average() ?? 0;
        Console.WriteLine($"- {build.Title} | {build.Difficulty} | avg={avg:0.00} ({build.Ratings.Count} ratings)");
    }

    Console.WriteLine();
    Console.WriteLine($"Tactician builds:");
    foreach (var build in builds.WithDifficulty(Difficulty.Tactician).OrderByNewest().ToList())
    {
        Console.WriteLine($"- {build.Title} | {build.Character?.Name}");
    }

    Console.WriteLine();
    Console.WriteLine("Search by title contains 'Blade':");
    foreach (var build in builds.SearchTitle("Blade").OrderByNewest().ToList())
    {
        Console.WriteLine($"- {build.Title} | {build.Character?.Name}");
    }

    Console.WriteLine();
    Console.WriteLine("Most popular (by rating count):");
    foreach (var row in builds.OrderByPopularity().SelectRatingStats().Take(5).ToList())
    {
        Console.WriteLine($"- {row.Title} | ratings={row.RatingCount} | avg={row.AverageScore:0.00}");
    }
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

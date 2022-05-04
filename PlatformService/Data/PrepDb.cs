using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
      }
    }

    private static void SeedData(AppDbContext context, bool isProduction)
    {
      if (isProduction)
      {
        Console.WriteLine("--> Applying Migrations...");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Couldn't run migrations: {ex.Message}");
        }
      }
      else
      {
        if (!context.Platforms.Any())
        {
          Console.WriteLine("--> Seeding data...");
          context.Platforms.AddRange(
            new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
            new Platform() { Name = "Deno", Publisher = "Deno land", Cost = "Free" },
            new Platform() { Name = "Nest.JS", Publisher = "Nest", Cost = "Free" },
            new Platform() { Name = "CockroachDb", Publisher = "Cockroach Labs", Cost = "Free" }
          );
          context.SaveChanges();
        }
        else
        {
          Console.WriteLine("--> We already have data");
        }
      }
    }
  }
}
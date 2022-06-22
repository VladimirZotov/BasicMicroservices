using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProd)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext appDbContext, bool isProd)
        {

            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    appDbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
                
            }

            if (!appDbContext.Platforms.Any())
            {
                appDbContext.Platforms.AddRange(new List<Platform>() {
                    new Platform() { Name= "DotNet", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name= "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name= "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" },
                });

                appDbContext.SaveChanges();
            }
        }
    }
}

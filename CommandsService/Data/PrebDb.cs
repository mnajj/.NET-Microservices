using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
  public static class PrebDb
  {
    public static void PrepPopulation(IApplicationBuilder appBuilder)
    {
      using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();
        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
      }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
      foreach (var plat in platforms)
      {
        if (!repo.ExternalPlatformExist(plat.ExternalID))
        {
          repo.CreatePlatform(plat);
        }
      }
    }
  }
}
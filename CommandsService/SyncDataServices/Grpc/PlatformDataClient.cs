using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
  public class PlatformDataClient : IPlatformDataClient
  {
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper)
    {
      _config = config;
      _mapper = mapper;
    }
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
      var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
      var client = new GrpcPlatform.GrpcPlatformClient(channel);
      var req = new GetAllRequest();

      try
      {
        var repy = client.GetAllPlatforms(req);
        return _mapper.Map<IEnumerable<Platform>>(repy.Platform);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"==> Couldn't call gRPC Server: {ex.Message}");
        return null;
      }
    }
  }
}
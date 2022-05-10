using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);
      switch (eventType)
      {
        case EventType.PlatformPublished:
          // TODO
          break;
        default:
          // TODO
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
      switch (eventType.Event)
      {
        case "Platform_Publish":
          return EventType.PlatformPublished;
        default:
          return EventType.Undetermined;
      }
    }

    private void AddPlatform(string platformPublishMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
        var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishMessage);
        try
        {
          var plat = _mapper.Map<Platform>(platformPublishDto);
          if (!repo.ExternalPlatformExist(plat.ExternalID))
          {
            repo.CreatePlatform(plat);
            repo.SaveChanges();
          }
          else
          {
            Console.WriteLine($"--> Platform already exist!");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Couldn't add platform to db: {ex.Message}");
        }
      }
    }
  }
}
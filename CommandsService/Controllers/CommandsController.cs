using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("api/c/platforms/{platformId}/[controller]")]
  public class CommandsController : ControllerBase
  {
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
      if (!_repository.PlatformExist(platformId))
      {
        return NotFound();
      }
      var commands = _repository.GetCommandsForPlatform(platformId);
      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
      if (!_repository.PlatformExist(platformId))
      {
        return NotFound();
      }
      var command = _repository.GetCommand(platformId, commandId);
      if (command is null)
      {
        return NotFound();
      }
      return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandDto)
    {
      if (!_repository.PlatformExist(platformId))
      {
        return NotFound();
      }
      Command command = _mapper.Map<Command>(commandDto);

      _repository.CreateCommand(platformId, command);
      _repository.SaveChanges();

      CommandReadDto commandReadDto = _mapper.Map<CommandReadDto>(command);
      return CreatedAtAction(
        nameof(GetCommandForPlatform),
        new { platformId = platformId, commandId = commandReadDto.Id },
        commandReadDto
      );
    }
  }
}
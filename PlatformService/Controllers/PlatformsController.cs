using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;

    public PlatformsController(IPlatformRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      var platforms = _repository.GetAllPlatforms();
      var platformsReadDto = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
      return Ok(platformsReadDto);
    }
  }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _platformRepo;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IMapper mapper, IPlatformRepo platformRepo, ICommandDataClient commandDataClient )
        {
            _mapper = mapper;
            _platformRepo = platformRepo;
            _commandDataClient = commandDataClient;
        }
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreateAsync(PlatformCreateDto platformCreateDto)
        {
            var platform = _mapper.Map<Platform>(platformCreateDto);
            _platformRepo.CreatePlatform(platform);
            _platformRepo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send sync: {ex.Message}");
            }

            //CreatedAtRoute returns 201 Created response and provide "location" header parameter added automatically
            return CreatedAtRoute(nameof(GetPlatformById), new { platformId = platformReadDto.Id}, platformReadDto);
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = _platformRepo.GetAllPlatforms();

            return Ok(_mapper.Map<List<PlatformReadDto>>(platforms));
        }
        [HttpGet("{platformId}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int platformId)
        {
            var platform = _platformRepo.GetPlatformById(platformId);
            if (platform == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }
    }
}

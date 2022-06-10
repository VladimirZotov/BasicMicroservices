using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _platformRepo;
        
        public PlatformsController(IMapper mapper, IPlatformRepo platformRepo)
        {
            _mapper = mapper;
            _platformRepo = platformRepo;
        }
        [HttpPost]
        public ActionResult<PlatformReadDto> Create(PlatformCreateDto platformCreateDto)
        {
            var platform = _mapper.Map<Platform>(platformCreateDto);
            _platformRepo.CreatePlatform(platform);
            _platformRepo.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);
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

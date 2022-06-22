using AutoMapper;
using CommandsService.Dto;
using CommandsService.Models;

namespace CommandsService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<Platform, PlatformReadDto>();
        }
    }
}

using AutoMapper;
using VillaAPI.Models;
using VillaAPI.Models.DTO;

namespace VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Villa, VillaDTO>().ReverseMap();

        CreateMap<Villa, VillaCreateDTO>().ReverseMap();
        CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

        CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();

        CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
        CreateMap<ApplicationUser, UserDTO>().ReverseMap();
    }
}
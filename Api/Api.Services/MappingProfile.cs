using AutoMapper;
using BoilerplateModel = Api.Common.Dtos.BoilerplateModel;

namespace Api.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
        => CreateMap<Models.BoilerplateModel, BoilerplateModel>().ReverseMap();
}
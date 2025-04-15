using AutoMapper;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.MappingProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, CategoryDTO>()
                .ReverseMap();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();
        }
    }
}

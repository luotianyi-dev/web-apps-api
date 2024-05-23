using AutoMapper;
using TianyiNetwork.Web.AppsApi.Models.Entities;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Models.Mappers
{
    public class CardMapper : Profile
    {
        public CardMapper()
        {
            CreateMap<CardApplication, CardEntity>()
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(
                        src => DateOnly.ParseExact(src.DateOfBirth, "yyyy-MM-dd")));
        }
    }
}

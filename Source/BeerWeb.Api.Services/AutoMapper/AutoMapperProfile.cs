
using AutoMapper;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;

namespace BeerWeb.Api.Services.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Bar, BarDto>().ReverseMap();
            CreateMap<Beer, BeerDto>().ReverseMap();
            CreateMap<Brewery, BreweryDto>().ReverseMap();
            CreateMap<BarBeer, BarBeerDto>().ReverseMap();
            CreateMap<BreweryBeer, BreweryBeerDto>().ReverseMap();
        }
    }
}

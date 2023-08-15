using BeerWeb.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BeerWeb.Api.Services.Interface
{
    public interface IBeerService
    {
        Task<BeerDto> AddBeer(BeerDto beerDto);
        Task UpdateBeer(int id, BeerDto beerDto);
        Task<IEnumerable<BeerDto>> GetAll();
        Task<BeerDto> GetById(int beerId);
        Task<IEnumerable<BeerDto>> GetBeer([FromQuery] double gtAlcoholByVolume, [FromQuery] double ltAlcoholByVolume);
    }
}

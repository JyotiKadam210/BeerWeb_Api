using BeerWeb.Api.Dto;

namespace BeerWeb.Api.Services.Interface
{
    public interface IBarBeersService
    {
        Task<IEnumerable<BarBeersDto>> GetAllBarsWithBeers();
        Task<IEnumerable<BarBeersDto>> GetBarbyIdWithAllBeers(int id);
        Task<BarBeerDto> AddBarBeer(BarBeerDto barBeer);
    }
}

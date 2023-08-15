using BeerWeb.Api.Dto;

namespace BeerWeb.Api.Services.Interface
{
    public interface IBreweryBeersService
    {
        Task<IEnumerable<BreweryBeersDto>> GetAllBreweriesWithBeers();
        Task<IEnumerable<BreweryBeersDto>> GetBrewerybyIdWithAllBeers(int id);
        Task<BreweryBeerDto> AddBreweryBeer(BreweryBeerDto breweryBeer);
    }
}

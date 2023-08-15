
using BeerWeb.Api.Dto;

namespace BeerWeb.Api.Services.Interface
{
    public interface IBreweryService
    {
        Task<BreweryDto> AddBrewery(BreweryDto breweryDto);
        Task UpdateBrewery(int id, BreweryDto breweryDto);
        Task<IEnumerable<BreweryDto>> GetAll();
        Task<BreweryDto> GetById(int breweryId);
    }
}

using BeerWeb.Api.Dto;

namespace BeerWeb.Api.Services.Interface
{ 
    public interface IBarService
    {
        Task<BarDto> AddBar(BarDto barDto);
        Task UpdateBar(int id, BarDto barDto);
        Task<IEnumerable<BarDto>> GetAll();
        Task<BarDto> GetById(int barId);
    }
}

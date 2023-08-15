using BeerWeb.Api.DataAccess.Interface.Generic;
using BeerWeb.Api.DataAccess.Model;

namespace BeerWeb.Api.DataAccess.Interface
{
    public interface IBeerRepository : IGenericRepository<Beer>
    {
        Task<IEnumerable<Beer>> GetBeerByAlcoholParameter(double gtAlcoholByVolume, double ltAlcoholByVolume);
    }
}
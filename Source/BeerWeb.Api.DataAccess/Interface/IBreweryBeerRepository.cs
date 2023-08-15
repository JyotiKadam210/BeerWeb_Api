using BeerWeb.Api.DataAccess.Interface.Generic;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;

namespace BeerWeb.Api.DataAccess.Interface
{
    public interface IBreweryBeerRepository: IBeerRelationalRepository<BreweryBeer, BreweryBeersDto>
    {
    }
}
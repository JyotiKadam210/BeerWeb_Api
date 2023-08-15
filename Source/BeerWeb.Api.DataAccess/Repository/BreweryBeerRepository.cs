using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using Microsoft.EntityFrameworkCore;

namespace BeerWeb.Api.DataAccess.Repository
{
    public class BreweryBeerRepository : IBreweryBeerRepository
    {
        private readonly BeerStoreDbContext dbContext;
        public BreweryBeerRepository(BeerStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Add brewery beer link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BreweryBeer> Add(BreweryBeer model)
        {
            var result = await dbContext.BreweryBeers.AddAsync(model);
            return result.Entity;
        }

        /// <summary>
        /// check brewery beer link is present or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return (dbContext.BarsBeers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Get all breweries with associated beers 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BreweryBeersDto>> GetAll()
        {
            var breweries = await dbContext.Breweries.ToListAsync();

            return GetBreweryWithBeers( breweries);
        }

        /// <summary>
        /// Get brewery with associated beers 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BreweryBeersDto>> GetById(int id)
        {
            var brewery = await dbContext.Breweries.Where(b => b.BreweryId == id).ToListAsync();

            return GetBreweryWithBeers(brewery);
        }        

        /// <summary>
        /// Get brewery beer collection
        /// </summary>
        /// <param name="breweryBeersDto"></param>
        /// <param name="breweries"></param>
        /// <returns></returns>
        private IEnumerable<BreweryBeersDto> GetBreweryWithBeers(List<Brewery> breweries)
        {
            var breweryBeersDto = new List<BreweryBeersDto>();

            breweryBeersDto.AddRange(from brewery in breweries
                                     let beerIDs = dbContext.BreweryBeers.Where(bb => bb.BreweryId == brewery.BreweryId).Select(beer => beer.BeerId).ToList()
                                     let beers = dbContext.Beers.Where(beer => beerIDs.Contains(beer.BeerId)).ToList()
                                     select new BreweryBeersDto
                                     {
                                         Brewery = new BreweryDto { BreweryId = brewery.BreweryId, Name = brewery.Name},
                                         Beers = beers.Select(beer => new BeerDto { BeerId = beer.BeerId, Name = beer.Name, PercentageAlcoholByVolume = beer.PercentageAlcoholByVolume }).ToList()
                                     });

            return breweryBeersDto.ToList();
        }
    }
}

using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.Dto;
using Microsoft.EntityFrameworkCore;

namespace BeerWeb.Api.DataAccess.Repository
{
    public class BarBeerRepository : IBarBeerRepository
    {
        private readonly BeerStoreDbContext dbContext;
        public BarBeerRepository(BeerStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Add new beer to bar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BarBeer> Add(BarBeer model)
        {
            var result = await dbContext.BarsBeers.AddAsync(model);
            return result.Entity;
        }

        /// <summary>
        /// Check beer bar link present
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return (dbContext.BarsBeers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Get all bars with associated beers
        /// </summary>
        /// <returns></returns>       
        public async Task<IEnumerable<BarBeersDto>> GetAll()
        {
           var bars = await dbContext.Bars.ToListAsync();

            return GetBarsWithBeers(bars);
        }

        /// <summary>
        /// Get bar by its id with all associated beers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BarBeersDto>> GetById(int id)
        {   
            var bars = await dbContext.Bars.Where(bar => bar.BarId == id).ToListAsync();

            return GetBarsWithBeers(bars);
        }

        /// <summary>
        /// Fetch bar and beers
        /// </summary>
        /// <param name="bars"></param>
        /// <returns></returns>
        private IEnumerable<BarBeersDto> GetBarsWithBeers(List<Bar> bars)
        {
            var barBeersDto = new List<BarBeersDto>();

            barBeersDto.AddRange(from bar in bars
                                 let beerIDs = dbContext.BarsBeers.Where(beerBar => beerBar.BarId == bar.BarId).Select(p => p.BeerId).ToList()
                                 let beers = dbContext.Beers.Where(beer => beerIDs.Contains(beer.BeerId)).ToList()
                                 select new BarBeersDto
                                 {
                                     Bar = new BarDto { BarId = bar.BarId, Name = bar.Name, Address = bar.Address },
                                     Beers = beers.Select(beer => new BeerDto { BeerId = beer.BeerId, Name = beer.Name, PercentageAlcoholByVolume = beer.PercentageAlcoholByVolume }).ToList()
                                 });

            return barBeersDto.ToList();
        }
    }
}

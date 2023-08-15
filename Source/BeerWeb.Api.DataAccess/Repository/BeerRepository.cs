using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.DataAccess.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BeerWeb.Api.DataAccess.Repository
{
    public class BeerRepository : GenericRepository<Beer>, IBeerRepository
    {
        private readonly BeerStoreDbContext dbContext;
        public BeerRepository(BeerStoreDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Get all beers with optional filtering query parameters for alcohol content (gtAlcoholByVolume = greater than, ltAlcoholByVolume = less than)
        /// </summary>
        /// <param name="gtAlcoholByVolume"></param>
        /// <param name="ltAlcoholByVolume"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Beer>> GetBeerByAlcoholParameter(Double gtAlcoholByVolume, double ltAlcoholByVolume)
        {
            var query = dbContext.Beers.AsQueryable(); ;
            if (gtAlcoholByVolume >= 0)
                query = query.Where(t => t.PercentageAlcoholByVolume >= gtAlcoholByVolume);
            if (ltAlcoholByVolume > 0)
                query = query.Where(p => p.PercentageAlcoholByVolume <= ltAlcoholByVolume);

            return await query.ToListAsync();
        }

    }
}
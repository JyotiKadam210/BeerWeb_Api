using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Repository;
using BreweryWeb.Api.DataAccess.Repository;

namespace BeerWeb.Api.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BeerStoreDbContext dbContext;

        private readonly IBeerRepository beerRepository;
        private readonly IBarRepository barRepository;
        private readonly IBreweryRepository breweryRepository;
        private readonly IBarBeerRepository barBeerRepository;
        private readonly IBreweryBeerRepository breweryBeerRepository;

        public UnitOfWork(BeerStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IBeerRepository BeerRepository => beerRepository ?? new BeerRepository(dbContext);
        public IBarRepository BarRepository => barRepository ?? new BarRepository(dbContext);
        public IBreweryRepository BreweryRepository => breweryRepository ?? new BreweryRepository(dbContext);
        public IBarBeerRepository BarBeerRepository => barBeerRepository ?? new BarBeerRepository(dbContext);
        public IBreweryBeerRepository BreweryBeerRepository => breweryBeerRepository ?? new BreweryBeerRepository(dbContext);

        public async Task SaveAsync()
        {
           await dbContext.SaveChangesAsync();
        }
    }
}
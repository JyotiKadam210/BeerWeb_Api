using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.DataAccess.Repository.GenericRepository;

namespace BreweryWeb.Api.DataAccess.Repository
{
    public class BreweryRepository : GenericRepository<Brewery>, IBreweryRepository
    {       
        public BreweryRepository(BeerStoreDbContext dbContext) : base(dbContext) { }
    }
}
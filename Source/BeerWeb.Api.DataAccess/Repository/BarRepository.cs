using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.Model;
using BeerWeb.Api.DataAccess.Repository.GenericRepository;

namespace BeerWeb.Api.DataAccess.Repository
{
    public class BarRepository : GenericRepository<Bar>, IBarRepository
    {       
        public BarRepository(BeerStoreDbContext dbContext) : base(dbContext) { }
    }
}

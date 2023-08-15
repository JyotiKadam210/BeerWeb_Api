using BeerWeb.Api.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace BeerWeb.Api.DataAccess.DatabaseContext
{   public partial class BeerStoreDbContext : DbContext
    {
        public BeerStoreDbContext(DbContextOptions<BeerStoreDbContext> options)  : base(options)
        {
        }

        public virtual DbSet<Bar> Bars { get; set; }
        public virtual DbSet<Beer> Beers { get; set; }
        public virtual DbSet<BarBeer> BarsBeers { get; set; }
        public virtual DbSet<Brewery> Breweries { get; set; }
        public virtual DbSet<BreweryBeer> BreweryBeers { get; set; }
    }
}

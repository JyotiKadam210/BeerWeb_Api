
namespace BeerWeb.Api.Dto
{
    public class BreweryBeersDto
    {
        public BreweryDto? Brewery { get; set; }
        public List<BeerDto>? Beers { get; set; }
    }

}

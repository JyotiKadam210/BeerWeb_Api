
namespace BeerWeb.Api.Dto
{
    public class BarBeersDto
    {
        public BarDto? Bar { get; set; }
        public List<BeerDto>? Beers { get; set; }
    }
}

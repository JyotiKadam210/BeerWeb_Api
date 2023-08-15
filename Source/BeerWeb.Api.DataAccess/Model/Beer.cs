
namespace BeerWeb.Api.DataAccess.Model
{
    public class Beer
    {
        public int BeerId { get; set; }

        public string Name { get; set; } = null!;

        public double PercentageAlcoholByVolume { get; set; }
    }
}

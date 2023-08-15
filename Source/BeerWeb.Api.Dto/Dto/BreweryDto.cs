
using System.ComponentModel.DataAnnotations;

namespace BeerWeb.Api.Dto
{
    public class BreweryDto
    {
        [Required]
        public int BreweryId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Brewery name is required")]
        public string Name { get; set; } = null!;
    }
}

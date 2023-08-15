
using System.ComponentModel.DataAnnotations;

namespace BeerWeb.Api.Dto
{
    public class BeerDto
    {
        [Required]
        public int BeerId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Beer name is required")]
        public string Name { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "PercentageAlcoholByVolume is required")]
        public double PercentageAlcoholByVolume { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace BeerWeb.Api.Dto
{
    public class BarDto
    {
        [Required]
        public int BarId { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage ="Bar name is required")]
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }
}

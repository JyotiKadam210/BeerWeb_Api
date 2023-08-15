namespace BeerWeb.Api.DataAccess.Model;

public partial class Bar
{
    public int BarId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }
    
}

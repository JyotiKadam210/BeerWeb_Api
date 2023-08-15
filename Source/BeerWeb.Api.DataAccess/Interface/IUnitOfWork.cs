namespace BeerWeb.Api.DataAccess.Interface
{
    public interface IUnitOfWork
    {
        IBeerRepository BeerRepository { get; }
        IBarRepository BarRepository { get; }
        IBreweryRepository BreweryRepository { get; }
        IBarBeerRepository BarBeerRepository { get; }
        IBreweryBeerRepository BreweryBeerRepository { get; }
        Task SaveAsync();
    }
}
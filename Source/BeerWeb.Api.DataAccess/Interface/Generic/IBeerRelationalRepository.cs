namespace BeerWeb.Api.DataAccess.Interface.Generic
{
    public interface IBeerRelationalRepository<TEntityMain, TEntityRelational>
    {
        Task<TEntityMain> Add(TEntityMain model);
        Task<IEnumerable<TEntityRelational>> GetById(int id);
        Task<IEnumerable<TEntityRelational>> GetAll();
        bool Exists(int id);
    }
}
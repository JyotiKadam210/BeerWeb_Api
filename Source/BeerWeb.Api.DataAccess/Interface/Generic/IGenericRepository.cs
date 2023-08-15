using System.Linq.Expressions;

namespace BeerWeb.Api.DataAccess.Interface.Generic
{
    public interface IGenericRepository<T>
    {
        Task<T> Add(T model);
        bool Update(int id, T model);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        bool Exists(Expression<Func<T, bool>> predicate);
    }
}
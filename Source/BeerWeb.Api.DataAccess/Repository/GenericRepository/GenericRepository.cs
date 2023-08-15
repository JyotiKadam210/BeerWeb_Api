using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeerWeb.Api.DataAccess.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BeerStoreDbContext dbContext;
        private readonly DbSet<T> entitySet;

        public GenericRepository(BeerStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
            entitySet = this.dbContext.Set<T>();
        }

        /// <summary>
        /// Add new entity to db
        /// </summary>
        /// <param name="model"></p.aram>
        /// <returns></returns>
        public async Task<T> Add(T model)
        {
            var result = await entitySet.AddAsync(model);
            return result.Entity;
        }

        /// <summary>
        /// check entity is prent or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return entitySet.Any(predicate);
    }

        /// <summary>
        /// Get all data
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll()
        {
            return await entitySet.ToListAsync();
        }

        /// <summary>
        /// Get entity by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetById(int id)
        {
            return await entitySet.FindAsync(id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(int id, T model)
        {
            entitySet.Update(model);
            return true;
        }
    }
}

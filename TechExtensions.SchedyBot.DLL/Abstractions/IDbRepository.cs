using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.DLL.Abstractions
{
    public interface IDbRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task CreateManyAsync(IEnumerable<TEntity> entity);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity entity);
        void DeleteMany(IEnumerable<TEntity> entites);
        void Delete(TEntity entity);
        Task SaveChangesAsync();
    }
}

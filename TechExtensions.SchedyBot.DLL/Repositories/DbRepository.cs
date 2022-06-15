using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TechExtensions.SchedyBot.DLL.Abstractions;
using TechExtensions.SchedyBot.DLL.Entities;

namespace TechExtensions.SchedyBot.DLL.Repositories
{
    /// <summary>
    /// Класс, который используется для работы с БД через EF Core
    /// </summary>
    public class DbRepository<TEntity> : IDbRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly SchedulyBotContext _dbContext;

        public DbRepository(SchedulyBotContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public void Update(TEntity entity)
        {
            SetUtc(entity);
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
        public void DeleteMany(IEnumerable<TEntity> entites)
        {
            _dbContext.Set<TEntity>().RemoveRange(entites);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateManyAsync(IEnumerable<TEntity> entity)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entity);
        }

        static void SetUtc(Object entity)
        {
            if (entity == null)
                return;

            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
                if (property?.GetValue(entity)?.GetType() == DateTime.Now.GetType())
                    property.SetValue(entity, DateTime.SpecifyKind((DateTime)property.GetValue(entity)!, DateTimeKind.Utc));
                else if (property?.GetValue(entity) is BaseEntity)
                    SetUtc(property.GetValue(entity)!);
                else if (property?.GetValue(entity) is IEnumerable<BaseEntity>)
                    foreach (var prop in property?.GetValue(entity) as IEnumerable<object>)
                        SetUtc(prop);
        }
    }

}

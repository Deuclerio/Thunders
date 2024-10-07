using Thunders.Domain.Base;
using Thunders.Domain.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thunders.Infra.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        public IDbConnection Connection { get; set; }
        protected IUnitOfWork Uow { get; set; }
        protected virtual Expression<Func<TEntity, object>>[]? DefaultIncludes => null;

        public Repository(IUnitOfWork uow)
        {
            Uow = uow;
            Connection = uow.Connection;
        }

        public virtual async Task Inactivate(Guid id)
        {
            var obj = Activator.CreateInstance<TEntity>();

            Uow.Context.Set<TEntity>().Update(obj);
            await Uow.Context.SaveChangesAsync();
        }

        public virtual async Task Inactivate(TEntity obj)
        {
            Uow.Context.Set<TEntity>().Update(obj);
            await Uow.Context.SaveChangesAsync();
        }

        protected virtual IQueryable<TEntity> Include(IQueryable<TEntity> dbSet, params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            return includeExpressions.Aggregate(dbSet, (current, expression) => current.Include(expression));
        }


        public virtual async Task<TEntity?> GetByIdActive(long id)
        {
            var query = Uow.Context.Set<TEntity>().AsQueryable();
            if (DefaultIncludes is not null)
                query = Include(query, DefaultIncludes);

            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public virtual async Task<TEntity> Insert(TEntity obj)
        {
            Uow.Context.Set<TEntity>().Add(obj);
            await Uow.Context.SaveChangesAsync();
            return obj;
        }

        public virtual async Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> obj)
        {
            await Uow.Context.Set<TEntity>().AddRangeAsync(obj);
            await Uow.Context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<TEntity> Update(TEntity obj)
        {
            Uow.Context.Set<TEntity>().Update(obj);
            await Uow.Context.SaveChangesAsync();
            return obj;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> obj)
        {
            Uow.Context.Set<TEntity>().UpdateRange(obj);

            await Uow.Context.SaveChangesAsync();
            return obj;
        }

        public virtual async Task Delete(Guid id)
        {
            var obj = Activator.CreateInstance<TEntity>();

            Uow.Context.Set<TEntity>().Remove(obj);
            await Uow.Context.SaveChangesAsync();
        }

        public virtual async Task DeleteRange(IEnumerable<TEntity> obj)
        {
            Uow.Context.Set<TEntity>().RemoveRange(obj);
            await Uow.Context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var query = Uow.Context.Set<TEntity>();

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            var query = Uow.Context.Set<TEntity>().Where(predicate);

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return await Uow.Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            var query = Uow.Context.Set<TEntity>().AnyAsync(predicate);

            return await query;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thunders.Domain.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Inactivate(TEntity obj);
        Task<TEntity> Insert(TEntity obj);
        Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> obj);
        Task<TEntity> Update(TEntity obj);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> obj);
        Task<TEntity?> GetByIdActive(long id);
        Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Any(Expression<Func<TEntity, bool>> predicate);
        Task DeleteRange(IEnumerable<TEntity> obj);
        Task Delete(Guid id);
    }
}

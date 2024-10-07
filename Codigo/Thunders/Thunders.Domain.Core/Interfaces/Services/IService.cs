using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunders.Domain.Core.Interfaces.Services
{
    public interface IService<TEntity> : IDisposable where TEntity : class
    {
        Task Inactivate(TEntity obj);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Insert(TEntity obj);
        Task<TEntity> Update(TEntity obj);
        void showError(string propertyType, string errorMenssage);
        Task<TEntity?> GetByIdActive(long id);
    }
}

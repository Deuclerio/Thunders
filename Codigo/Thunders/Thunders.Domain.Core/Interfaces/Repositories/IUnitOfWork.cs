using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace Thunders.Domain.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        bool ValidateEntity { get; set; }
        DbContext Context { get; set; }
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}

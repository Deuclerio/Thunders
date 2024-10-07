using Thunders.Domain.Core.Interfaces.Repositories;
using Thunders.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;

namespace Thunders.Infra.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        public Guid Id { get; set; }
        public bool ValidateEntity { get; set; }
        public DatabaseContext Context { get; set; } 
        public IDbConnection Connection { get; set; }
        public IDbTransaction? Transaction { get; set; }

        public UnitOfWork(DatabaseContext context) 
        {
            ValidateEntity = true;
            Context = context;
            Context.Database.OpenConnection();
            Connection = Context.Database.GetDbConnection();
        }

        public void BeginTransaction()
        {
            var transactionContext = Context.Database.BeginTransaction();
            Transaction = transactionContext.GetDbTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
        }

        public void Rollback()
        {
            Transaction?.Rollback();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Connection.State == ConnectionState.Open)
                        Connection.Close();

                    Transaction?.Dispose();
                    Connection.Dispose();
                    Context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        DbContext IUnitOfWork.Context
        {
            get { return Context; }
            set { Context = (DatabaseContext)value; }
        }
    }
}

using Thunders.Domain.Core.Interfaces.Repositories;
using Thunders.Domain.Core.Interfaces.Services;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thunders.Domain.Services.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        protected IRepository<TEntity> Repository { get; }
        protected AbstractValidator<TEntity>? Validator { get; set; }
        protected IUnitOfWork Context { get; set; }

        public Service(IUnitOfWork context, IRepository<TEntity> repository)
        {
            Repository = repository;
            Context = context;
        }      

        public virtual async Task Inactivate(TEntity obj)
        {
            if (Context.ValidateEntity)
                Validator?.Validate(obj, options =>
                {
                    options.ThrowOnFailures();
                    options.IncludeRuleSets("Delete");
                });

            await Repository.Inactivate(obj);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Repository.GetAll();
        }

        public virtual async Task<TEntity?> GetByIdActive(int id)
        {
            return await Repository.GetByIdActive(id);
        }

        public virtual async Task<TEntity?> GetByIdActive(long id)
        {
            return await Repository.GetByIdActive(id);
        }

        public virtual async Task<TEntity> Insert(TEntity obj)
        {
            if (Context.ValidateEntity)
                Validator?.Validate(obj, options =>
                {
                    options.ThrowOnFailures();
                    options.IncludeRuleSets("Insert");
                });

            return await Repository.Insert(obj);
        }

        public virtual async Task<IEnumerable<TEntity>> InsertRange(IEnumerable<TEntity> obj)
        {
            foreach (var item in obj)
            {
                if (Context.ValidateEntity)
                    Validator?.Validate(item, options =>
                    {
                        options.ThrowOnFailures();
                        options.IncludeRuleSets("Insert");
                    });
            }

            return await Repository.InsertRange(obj);
        }

        public virtual async Task<TEntity> Update(TEntity obj)
        {
            if (Context.ValidateEntity)
                Validator?.Validate(obj, options =>
                {
                    options.ThrowOnFailures();
                    options.IncludeRuleSets("Update");
                });

            return await Repository.Update(obj);
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> obj)
        {
            foreach (var item in obj)
            {
                if (Context.ValidateEntity)
                    Validator?.Validate(item, options =>
                    {
                        options.ThrowOnFailures();
                        options.IncludeRuleSets("Update");
                    });
            }

            return await Repository.UpdateRange(obj);
        }

        public virtual async Task Delete(Guid id)
        {
            await Repository.Delete(id);
        }

        public virtual async Task DeleteRange(IEnumerable<TEntity> obj)
        {
            await Repository.DeleteRange(obj);
        }

        public void showError(string propertyType, string errorMenssage)
        {
            throw new ValidationException(new ValidationFailure[] { new ValidationFailure(propertyType, errorMenssage) });
        }

        public virtual async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.GetList(predicate);
        }

        public virtual async Task<TEntity> GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.GetFirst(predicate);
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return await Repository.Any(predicate);
        }

        public void Dispose()
        {
            Repository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

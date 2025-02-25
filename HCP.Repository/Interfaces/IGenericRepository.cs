using HCP.Repository.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Interfaces
{
    public interface IGenericRepository<T>
        where T : BaseEntity
    {
        Task<T?> GetEntityByIdAsync(Guid id);
        Task<IReadOnlyList<T>> ListAllAsync();
        IQueryable<T> GetAll();
        T? GetById(Guid id);
        T? Find(Expression<Func<T, bool>> match);
        Task<T?> FindAsync(Expression<Func<T, bool>> match);
        T? GetByIdAsDetached(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);
        bool Exists(Guid id);
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<T>> ListAsync();
        Task<IEnumerable<T>> ListAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
        );

        Task<IEnumerable<T>> ListAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProperties = null
        );
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
    }
}

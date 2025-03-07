using HCP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : BaseEntity;

        Task<int> Complete();
        int CompleteV2();

        Task<int> SaveChangesAsync();
    }
}

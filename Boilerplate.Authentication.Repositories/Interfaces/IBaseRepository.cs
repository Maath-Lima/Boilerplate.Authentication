using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Repositories.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        Task Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task<TEntity> GetById(int id);
        Task<IList<TEntity>> GetAll();
    }
}

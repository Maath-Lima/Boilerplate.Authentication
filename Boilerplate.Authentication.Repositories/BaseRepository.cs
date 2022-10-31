using Boilerplate.Authentication.Data;
using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DataContext DataContext;
        protected readonly DbSet<TEntity> DbSet;

        public BaseRepository(DataContext dataContext)
        {
            DataContext = dataContext;
            DbSet = DataContext.Set<TEntity>();
        }

        public async Task<IList<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await DbSet.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task Create(TEntity entity)
        {
            IsEntityNull(entity);
            await DbSet.AddAsync(entity);
            await SaveChanges();
        }

        public async Task Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            await SaveChanges();
        }

        public async Task Update(TEntity entity)
        {
            IsEntityNull(entity);
            DbSet.Update(entity);
            await SaveChanges();
        }

        private async Task SaveChanges()
        {
            await DataContext.SaveChangesAsync();
        }

        private void IsEntityNull(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException("Entity was null");
            }

            return;
        }
    }
}

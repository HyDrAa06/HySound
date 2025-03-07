using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HySound.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        HySoundContext _context;
        DbSet<T> _dbSet;

        public Repository(HySoundContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            if(entity != null)
            {
                await _dbSet.AddAsync(entity);
                await SaveAsync();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            if(entity != null)
            {
                _dbSet.Remove(entity);
                await SaveAsync();
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id);
            if(entity != null)
            {
                _dbSet.Remove(entity);
                await SaveAsync();
            }
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }
        
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> GetAllQuery(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> list = _dbSet.Where(filter);
            return list;
        }

        public IQueryable<T> GetAllQuery()
        {
            IQueryable<T> list = _dbSet;
            return list;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            T entity = await _dbSet.FirstOrDefaultAsync(filter);
            return entity;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            T entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
             _dbSet.Update(entity);
             await SaveAsync();
        }
    }
}

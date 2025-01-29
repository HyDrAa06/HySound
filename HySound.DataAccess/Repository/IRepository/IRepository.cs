using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
        Task SaveAsync();
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        IQueryable<T> GetAllQuery(Expression<Func<T, bool>> filter);
        IQueryable<T> GetAllQuery();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
    }
}

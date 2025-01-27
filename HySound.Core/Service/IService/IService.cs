using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IService<T> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
        Task SaveAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
    }
}

using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;
        public Service(IRepository<T> repository)
        {
            _repository = repository;
        }
        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _repository.GetAsync(filter);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task SaveAsync()
        {
            await _repository.SaveAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}

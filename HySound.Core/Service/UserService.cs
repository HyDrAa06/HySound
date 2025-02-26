using HySound.Core.Service.IService;
using HySound.DataAccess.Repository;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service
{
    public class UserService : IUserService
    {
        IRepository<User> _userRepository;
        public UserService(IRepository<User> repo)
        {
            _userRepository = repo;
        }
        public IQueryable<User> GetAll()
        {
            return _userRepository.GetAll();
        }
        public async Task AddUserAsync(User entity)
        {
            await _userRepository.AddAsync(entity);
        }

        public IQueryable<User> AllWithInclude(params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> query = _userRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteUserAsync(User entity)
        {
            await _userRepository.DeleteAsync(entity);
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            await _userRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>> filter)
        {
            return await _userRepository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserAsync(Expression<Func<User, bool>> filter)
        {
            return await _userRepository.GetAsync(filter);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task UpdateUserAsync(User entity)
        {
            await _userRepository.UpdateAsync(entity);
        }

        public async Task<IEnumerable<string>> GetAllUserNamesAsync()
        {
            List<string> names = new List<string>();
            var users = GetAll().ToList();

            users.ForEach(x => names.Add(x.Username));
            return names.ToList();
        }
    }
}

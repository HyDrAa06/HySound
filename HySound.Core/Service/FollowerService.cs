using HySound.Core.Service.IService;
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
    public class FollowerService : IFollowerService
    {
        IRepository<Followed> _followerRepository;

        public FollowerService(IRepository<Followed> followerRepository)
        {
            _followerRepository = followerRepository;
        }
        public async Task AddFollowerAsync(Followed entity)
        {
            await _followerRepository.AddAsync(entity);
        }

        public IQueryable<Followed> AllWithInclude(params Expression<Func<Followed, object>>[] includes)
        {
            IQueryable<Followed> query = _followerRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteFollowerAsync(Followed entity)
        {
            await _followerRepository.DeleteAsync(entity);
        }

        public async Task DeleteFollowerByIdAsync(int id)
        {
            await _followerRepository.DeleteByIdAsync(id);
        }

        public IQueryable<Followed> GetAll()
        {
            return _followerRepository.GetAll();
        }

        public async Task<IEnumerable<Followed>> GetAllFollowersAsync(Expression<Func<Followed, bool>> filter)
        {
            return await _followerRepository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Followed>> GetAllFollowersAsync()
        {
            return await _followerRepository.GetAllAsync();
        }

        public async Task<Followed> GetFollowerAsync(Expression<Func<Followed, bool>> filter)
        {
            return await _followerRepository.GetAsync(filter);
        }

        public async Task<Followed> GetFollowerByIdAsync(int id)
        {
            return await _followerRepository.GetByIdAsync(id);
        }

        public async Task UpdateFollowerAsync(Followed entity)
        {
            await _followerRepository.UpdateAsync(entity);
        }
    }
}

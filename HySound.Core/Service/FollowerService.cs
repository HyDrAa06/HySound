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
        IRepository<Follower> _followerRepository;

        public FollowerService(IRepository<Follower> followerRepository)
        {
            _followerRepository = followerRepository;
        }
        public async Task AddFollowerAsync(Follower entity)
        {
            await _followerRepository.AddAsync(entity);
        }

        public IQueryable<Follower> AllWithInclude(params Expression<Func<Follower, object>>[] includes)
        {
            IQueryable<Follower> query = _followerRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteFollowerAsync(Follower entity)
        {
            await _followerRepository.DeleteAsync(entity);
        }

        public async Task DeleteFollowerByIdAsync(int id)
        {
            await _followerRepository.DeleteByIdAsync(id);
        }

        public IQueryable<Follower> GetAll()
        {
            return _followerRepository.GetAll();
        }

        public async Task<IEnumerable<Follower>> GetAllFollowersAsync(Expression<Func<Follower, bool>> filter)
        {
            return await _followerRepository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Follower>> GetAllFollowersAsync()
        {
            return await _followerRepository.GetAllAsync();
        }

        public async Task<Follower> GetFollowerAsync(Expression<Func<Follower, bool>> filter)
        {
            return await _followerRepository.GetAsync(filter);
        }

        public async Task<Follower> GetFollowerByIdAsync(int id)
        {
            return await _followerRepository.GetByIdAsync(id);
        }

        public async Task UpdateFollowerAsync(Follower entity)
        {
            await _followerRepository.UpdateAsync(entity);
        }
    }
}

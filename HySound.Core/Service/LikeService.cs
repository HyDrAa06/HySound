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
    public class LikeService : ILikeService
    {
        IRepository<Like> _likeService;
        public LikeService(IRepository<Like> likeService)
        {
            _likeService = likeService;
        }
        public async Task AddLikeAsync(Like entity)
        {
            await _likeService.AddAsync(entity);
        }

        public IQueryable<Like> AllWithInclude(params Expression<Func<Like, object>>[] includes)
        {
            IQueryable<Like> query = _likeService.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteLikeAsync(Like entity)
        {
            await _likeService.DeleteAsync(entity);
        }

        public async Task DeleteLikeByIdAsync(int id)
        {
            await _likeService.DeleteByIdAsync(id);
        }

        public IQueryable<Like> GetAll()
        {
            return _likeService.GetAll();
        }

        public async Task<IEnumerable<Like>> GetAllLikesAsync(Expression<Func<Like, bool>> filter)
        {
            return await _likeService.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Like>> GetAllLikesAsync()
        {
            return await _likeService.GetAllAsync();
        }

        public async Task<Like> GetLikeAsync(Expression<Func<Like, bool>> filter)
        {
            return await _likeService.GetAsync(filter);
        }

        public async Task<Like> GetLikeByIdAsync(int id)
        {
            return await _likeService.GetByIdAsync(id);
        }

        public async Task UpdateLikeAsync(Like entity)
        {
            await _likeService.UpdateAsync(entity);
        }
    }
}

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
    public class TrackService : ITrackService
    {
        IRepository<Track> _repository;
        public TrackService(IRepository<Track> repo)
        {
            _repository = repo;
        }
        public async Task AddTrackAsync(Track entity)
        {
            await _repository.AddAsync(entity);
        }
        public IQueryable<Track> GetAll()
        {
            return _repository.GetAll();
        }
        public IQueryable<Track> AllWithInclude(params Expression<Func<Track, object>>[] includes)
        {
            IQueryable<Track> query = _repository.GetAllQuery();
            foreach(var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteTrackAsync(Track entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task DeleteTrackByIdAsync(int id)
        {
            await _repository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Track>> GetAllTracksAsync(Expression<Func<Track, bool>> filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Track>> GetAllTracksAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Track> GetTrackAsync(Expression<Func<Track, bool>> filter)
        {
            return await _repository.GetAsync(filter);
        }

        public async Task<Track> GetTrackByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);

        }

        public async Task UpdateTrackAsync(Track entity)
        {
            await _repository.UpdateAsync(entity);
        }

    
    }
}

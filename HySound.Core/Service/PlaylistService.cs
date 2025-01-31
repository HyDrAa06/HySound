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
    public class PlaylistService : IPlaylistService
    {
        IRepository<Playlist> _playlistRepository;

        public PlaylistService(IRepository<Playlist> repository)
        {
            _playlistRepository = repository;
        }
        public async Task AddPlaylistAsync(Playlist entity)
        {
            await _playlistRepository.AddAsync(entity);
        }

        public IQueryable<Playlist> AllWithInclude(params Expression<Func<Playlist, object>>[] includes)
        {
            IQueryable<Playlist> query = _playlistRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeletePlaylistAsync(Playlist entity)
        {
            await _playlistRepository.DeleteAsync(entity);
        }

        public async Task DeletePlaylistByIdAsync(int id)
        {
            await _playlistRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistsAsync(Expression<Func<Playlist, bool>> filter)
        {
            return await _playlistRepository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            return await _playlistRepository.GetAllAsync();
        }

        public async Task<Playlist> GetPlaylistAsync(Expression<Func<Playlist, bool>> filter)
        {
            return await _playlistRepository.GetAsync(filter);
        }

        public async Task<Playlist> GetPlaylistByIdAsync(int id)
        {
            return await _playlistRepository.GetByIdAsync(id);
        }

        public async Task UpdatePlaylistAsync(Playlist entity)
        {
            await _playlistRepository.UpdateAsync(entity);
        }
    }
}

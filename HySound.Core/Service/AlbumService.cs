using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service
{
    public class AlbumService : IAlbumService
    {
        IRepository<Album> _albumService;

        public AlbumService(IRepository<Album> albumRepository)
        {
            _albumService = albumRepository;
        }
        
        public async Task AddAlbumAsync(Album entity)
        {
            await _albumService.AddAsync(entity);
        }
        public IQueryable<Album> GetAll()
        {
            return _albumService.GetAll();
        }
        public IQueryable<Album> AllWithInclude(params Expression<Func<Album, object>>[] includes)
        {
            IQueryable<Album> query = _albumService.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteAlbumAsync(Album entity)
        {
            await _albumService.DeleteAsync(entity);
        }

        public async Task DeleteAlbumByIdAsync(int id)
        {
            await _albumService.DeleteByIdAsync(id);
        }

        public async Task<Album> GetAlbumAsync(Expression<Func<Album, bool>> filter)
        {
            return await _albumService.GetAsync(filter);
        }

        public async Task<Album> GetAlbumByIdAsync(int id)
        {
            return await _albumService.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Album>> GetAllAlbumAsync(Expression<Func<Album, bool>> filter)
        {
            return await _albumService.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Album>> GetAllAlbumAsync()
        {
            return await _albumService.GetAllAsync();
        }

        public async Task UpdateAlbumAsync(Album entity)
        {
            await _albumService.UpdateAsync(entity);
        }

        public async Task<IEnumerable<string>> GetAllAlbumNamesAsync()
        {
            List<string> names = new List<string>();

            var albums = GetAll().ToList();

            albums.ForEach(x => names.Add(x.Title));

            return names;
        }
    }
}

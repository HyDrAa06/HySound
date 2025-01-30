using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IAlbumService
    {
        public IQueryable<Album> GetAll();
        
        Task AddAlbumAsync(Album entity);
        Task UpdateAlbumAsync(Album entity);
        Task DeleteAlbumAsync(Album entity);
        Task DeleteAlbumByIdAsync(int id);
        IQueryable<Album> AllWithInclude(params Expression<Func<Album, object>>[] includes);
        Task<Album> GetAlbumByIdAsync(int id);
        Task<Album> GetAlbumAsync(Expression<Func<Album, bool>> filter);
        Task<IEnumerable<Album>> GetAllAlbumAsync(Expression<Func<Album, bool>> filter);
        Task<IEnumerable<Album>> GetAllAlbumAsync();
    }
}

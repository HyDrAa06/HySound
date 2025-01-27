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
        Album AddAlbumAsync(Album entity);
        Album UpdateAlbumAsync(Album entity);
        Album DeleteAlbumAsync(Album entity);
        Album DeleteAlbumByIdAsync(int id);
        IQueryable<Album> AllWithInclude(params Expression<Func<Album, object>>[] includes);
        Task<Album> GetAlbumByIdAsync(int id);
        Task<Album> GetAlbumAsync(Expression<Func<Album, bool>> filter);
        Task<IEnumerable<Album>> GetAllAlbumAsync(Expression<Func<Album, bool>> filter);
        Task<IEnumerable<Album>> GetAllAlbumAsync();
    }
}

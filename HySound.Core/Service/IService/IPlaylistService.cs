using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IPlaylistService
    {
        Task AddPlaylistAsync(Playlist entity);
        Task UpdatePlaylistAsync(Playlist entity);
        Task DeletePlaylistAsync(Playlist entity);
        Task DeletePlaylistByIdAsync(int id);
        IQueryable<Playlist> AllWithInclude(params Expression<Func<Playlist, object>>[] includes);
        Task<Playlist> GetPlaylistByIdAsync(int id);
        Task<Playlist> GetPlaylistAsync(Expression<Func<Playlist, bool>> filter);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync(Expression<Func<Playlist, bool>> filter);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
    }
}

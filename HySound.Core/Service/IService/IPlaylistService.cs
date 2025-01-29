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
        Playlist AddPlaylistAsync(Playlist entity);
        Playlist UpdatePlaylistAsync(Playlist entity);
        Playlist DeletePlaylistAsync(Playlist entity);
        Playlist DeletePlaylistByIdAsync(int id);
        IQueryable<Playlist> AllWithInclude(params Expression<Func<Playlist, object>>[] includes);
        Task<Playlist> GetPlaylistByIdAsync(int id);
        Task<Playlist> GetPlaylistAsync(Expression<Func<Playlist, bool>> filter);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync(Expression<Func<Playlist, bool>> filter);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
    }
}

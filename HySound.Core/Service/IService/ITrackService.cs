using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HySound.Core.Service.IService
{
    public interface ITrackService
    {

        Task AddTrackAsync(Track entity);
        Task UpdateTrackAsync(Track entity);
        Task DeleteTrackAsync(Track entity);
        Task DeleteTrackByIdAsync(int id);


        IQueryable<Track> AllWithInclude(params Expression<Func<Track, object>>[] includes);
        Task<Track> GetTrackByIdAsync(int id);


        Task<Track> GetTrackAsync(Expression<Func<Track, bool>> filter);
        Task<IEnumerable<Track>> GetAllTracksAsync(Expression<Func<Track, bool>> filter);
        Task<IEnumerable<Track>> GetAllTracksAsync();

        IQueryable<Track> GetAll();

    }
}

using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IFollowerService
    {
        Task AddFollowerAsync(Followed entity);
        Task UpdateFollowerAsync(Followed entity);
        Task DeleteFollowerAsync(Followed entity);
        IQueryable<Followed> GetAll();

        Task DeleteFollowerByIdAsync(int id);
        IQueryable<Followed> AllWithInclude(params Expression<Func<Followed, object>>[] includes);
        Task<Followed> GetFollowerByIdAsync(int id);
        Task<Followed> GetFollowerAsync(Expression<Func<Followed, bool>> filter);
        Task<IEnumerable<Followed>> GetAllFollowersAsync(Expression<Func<Followed, bool>> filter);
        Task<IEnumerable<Followed>> GetAllFollowersAsync();
        Task DeleteAllFollowersAndFollowing(int userId);

    }
}

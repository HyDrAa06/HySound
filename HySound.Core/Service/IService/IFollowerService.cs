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
        Task AddFollowerAsync(Follower entity);
        Task UpdateFollowerAsync(Follower entity);
        Task DeleteFollowerAsync(Follower entity);
        IQueryable<Follower> GetAll();

        Task DeleteFollowerByIdAsync(int id);
        IQueryable<Follower> AllWithInclude(params Expression<Func<Follower, object>>[] includes);
        Task<Follower> GetFollowerByIdAsync(int id);
        Task<Follower> GetFollowerAsync(Expression<Func<Follower, bool>> filter);
        Task<IEnumerable<Follower>> GetAllFollowersAsync(Expression<Func<Follower, bool>> filter);
        Task<IEnumerable<Follower>> GetAllFollowersAsync();
    }
}

using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface ILikeService
    {
        public IQueryable<Like> GetAll();

        Task AddLikeAsync(Like entity);
        Task UpdateLikeAsync(Like entity);
        Task DeleteLikeAsync(Like entity);
        Task DeleteLikeByIdAsync(int id);
        IQueryable<Like> AllWithInclude(params Expression<Func<Like, object>>[] includes);
        Task<Like> GetLikeByIdAsync(int id);
        Task<Like> GetLikeAsync(Expression<Func<Like, bool>> filter);
        Task<IEnumerable<Like>> GetAllLikesAsync(Expression<Func<Like, bool>> filter);
        Task<IEnumerable<Like>> GetAllLikesAsync();
    }
}

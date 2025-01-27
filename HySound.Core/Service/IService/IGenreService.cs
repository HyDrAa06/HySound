using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IGenreService
    {
        IQueryable<Genre> AllWithInclude(params Expression<Func<Genre, object>>[] includes);
        Task<Genre> GetGenreByIdAsync(int id);
        Task<Genre> GetGenreAsync(Expression<Func<Genre, bool>> filter);
        Task<IEnumerable<Genre>> GetAllGenreAsync(Expression<Func<Genre, bool>> filter);
        Task<IEnumerable<Genre>> GetAllGenreAsync();
    }
}

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
        IQueryable<Genre> GetAll();

        Task<Genre> GetGenreAsync(Expression<Func<Genre, bool>> filter);
        Task<IEnumerable<Genre>> GetAllGenresAsync(Expression<Func<Genre, bool>> filter);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}

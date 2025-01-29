using HySound.Core.Service.IService;
using HySound.DataAccess.Repository;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service
{
    public class GenreService : IGenreService
    {
        IRepository<Genre> _genreRepository;

        public GenreService(IRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public IQueryable<Genre> GetAll()
        {
            return _genreRepository.GetAll();
        }
        public IQueryable<Genre> AllWithInclude(params Expression<Func<Genre, object>>[] includes)
        {
            IQueryable<Genre> query = _genreRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync(Expression<Func<Genre, bool>> filter)
        {
            return await _genreRepository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre> GetGenreAsync(Expression<Func<Genre, bool>> filter)
        {
            Genre genre = await _genreRepository.GetAsync(filter);
            return genre;
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            Genre genre = await _genreRepository.GetByIdAsync(id);
            return genre;
        }
    }
}

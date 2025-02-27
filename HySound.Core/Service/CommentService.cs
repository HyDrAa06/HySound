using HySound.Core.Service.IService;
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
    public class CommentService : ICommentService
    {
        IRepository<Comment> _commentRepository;

        public CommentService(IRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task AddCommentAsync(Comment entity)
        {
            await _commentRepository.AddAsync(entity);
        }

        public IQueryable<Comment> AllWithInclude(params Expression<Func<Comment, object>>[] includes)
        {
            IQueryable<Comment> query = _commentRepository.GetAllQuery();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task DeleteCommentAsync(Comment entity)
        {
            await _commentRepository.DeleteAsync(entity);
        }

        public async Task DeleteCommentByIdAsync(int id)
        {
            await _commentRepository.DeleteByIdAsync(id);
        }

        public IQueryable<Comment> GetAll()
        {
            return _commentRepository.GetAll();
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync(Expression<Func<Comment, bool>> filter)
        {
            var comments = await _commentRepository.GetAllAsync(filter);
            return comments;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return comments;
        }

        public async Task<Comment> GetCommentAsync(Expression<Func<Comment, bool>> filter)
        {
            var comment = await _commentRepository.GetAsync(filter);
            return comment;
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment;
        }

        public async Task UpdateCommentAsync(Comment entity)
        {
            await _commentRepository.UpdateAsync(entity);
        }
    }
}

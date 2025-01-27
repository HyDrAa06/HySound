using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface ICommentService
    {
        Comment AddCommentAsync(Comment entity);
        Comment UpdateCommentAsync(Comment entity);
        Comment DeleteCommentAsync(Comment entity);
        Comment DeleteCommentByIdAsync(int id);
        IQueryable<Comment> AllWithInclude(params Expression<Func<Comment, object>>[] includes);
        Task<Comment> GetCommentByIdAsync(int id);
        Task<Comment> GetCommentAsync(Expression<Func<Comment, bool>> filter);
        Task<IEnumerable<Comment>> GetAllCommentAsync(Expression<Func<Comment, bool>> filter);
        Task<IEnumerable<Comment>> GetAllCommentAsync();
    }
}

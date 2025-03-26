using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HySound.Tests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<IRepository<Comment>> _mockCommentRepository;
        private ICommentService _commentService;

        [SetUp]
        public void Setup()
        {
            _mockCommentRepository = new Mock<IRepository<Comment>>();
            _commentService = new CommentService(_mockCommentRepository.Object);
        }

        [Test]
        public async Task AddCommentAsync()
        {
            var comments = new List<Comment>();
            var comment = new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" };
            _mockCommentRepository.Setup(r => r.AddAsync(comment)).Callback(() => comments.Add(comment));

            await _commentService.AddCommentAsync(comment);

            Assert.AreEqual(1, comments.Count);
        }

        [Test]
        public async Task DeleteCommentAsync()
        {
            var comment = new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" };
            var comments = new List<Comment> { comment };
            _mockCommentRepository.Setup(r => r.DeleteAsync(comment)).Callback(() => comments.Remove(comment));

            await _commentService.DeleteCommentAsync(comment);

            Assert.AreEqual(0, comments.Count);
        }

        [Test]
        public async Task DeleteCommentByIdAsync()
        {
            _mockCommentRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _commentService.DeleteCommentByIdAsync(1));
        }

        [Test]
        public async Task GetCommentByIdAsync()
        {
            int commentId = 1;
            var comment = new Comment { Id = commentId, UserId = 1, TrackId = 101, Content = "Great track!" };
            _mockCommentRepository.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);

            var result = await _commentService.GetCommentByIdAsync(commentId);

            Assert.AreEqual(comment, result);
        }

        [Test]
        public async Task GetCommentAsync()
        {
            var comment = new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" };
            _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync(comment);

            var result = await _commentService.GetCommentAsync(c => c.Id == 1);

            Assert.AreEqual(comment, result);
        }

        [Test]
        public void GetAll()
        {
            var comments = new List<Comment>
            {
                new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" },
                new Comment { Id = 2, UserId = 2, TrackId = 102, Content = "Nice song!" }
            }.AsQueryable();

            _mockCommentRepository.Setup(r => r.GetAll()).Returns(comments);

            var result = _commentService.GetAll();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllCommentsAsync()
        {
            var comments = new List<Comment>
            {
                new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" },
                new Comment { Id = 2, UserId = 2, TrackId = 102, Content = "Nice song!" }
            };

            _mockCommentRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(comments);

            var result = await _commentService.GetAllCommentsAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllCommentsAsyncWithFilter()
        {
            var comments = new List<Comment>
            {
                new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" },
                new Comment { Id = 2, UserId = 2, TrackId = 102, Content = "Nice song!" }
            };

            _mockCommentRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync((Expression<Func<Comment, bool>> filter) => comments.Where(filter.Compile()).ToList());

            var result = await _commentService.GetAllCommentsAsync(c => c.UserId == 1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().UserId);
        }

        [Test]
        public async Task UpdateCommentAsync()
        {
            var comment = new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" };
            _mockCommentRepository.Setup(r => r.UpdateAsync(comment)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _commentService.UpdateCommentAsync(comment));
        }

        [Test]
        public void AllWithInclude()
        {
            var comments = new List<Comment>
            {
                new Comment { Id = 1, UserId = 1, TrackId = 101, Content = "Great track!" },
                new Comment { Id = 2, UserId = 2, TrackId = 102, Content = "Nice song!" }
            }.AsQueryable();

            _mockCommentRepository.Setup(r => r.GetAllQuery()).Returns(comments);

            var result = _commentService.AllWithInclude();

            Assert.AreEqual(2, result.Count());
        }
    }
}

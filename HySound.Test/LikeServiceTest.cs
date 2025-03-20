using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HySound.Test
{
    [TestFixture]
    public class LikeServiceTest
    {
        private Mock<IRepository<Like>> _mockLikeRepository;
        private ILikeService _likeService;

        [SetUp]
        public void Setup()
        {
            _mockLikeRepository = new Mock<IRepository<Like>>();
            _likeService = new LikeService(_mockLikeRepository.Object);
        }

        [Test]
        public async Task AddLikeAsync()
        {
            var likes = new List<Like>();
            var like = new Like { Id = 1, UserId = 1, TrackId = 101 };
            _mockLikeRepository.Setup(r => r.AddAsync(like)).Callback(() => likes.Add(like));

            await _likeService.AddLikeAsync(like);

            Assert.AreEqual(1, likes.Count);
        }

        [Test]
        public async Task DeleteLikeAsync()
        {
            var like = new Like { Id = 1, UserId = 1, TrackId = 101 };
            var likes = new List<Like> { like };
            _mockLikeRepository.Setup(r => r.DeleteAsync(like)).Callback(() => likes.Remove(like));

            await _likeService.DeleteLikeAsync(like);

            Assert.AreEqual(0, likes.Count);
        }

        [Test]
        public async Task DeleteLikeByIdAsync()
        {
            _mockLikeRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _likeService.DeleteLikeByIdAsync(1));
        }

        [Test]
        public async Task GetLikeByIdAsync()
        {
            int likeId = 1;
            var like = new Like { Id = likeId, UserId = 1, TrackId = 101 };
            _mockLikeRepository.Setup(r => r.GetByIdAsync(likeId)).ReturnsAsync(like);

            var result = await _likeService.GetLikeByIdAsync(likeId);

            Assert.AreEqual(like, result);
        }

        [Test]
        public async Task GetLikeAsync()
        {
            var like = new Like { Id = 1, UserId = 2, TrackId = 101 };
            _mockLikeRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync(like);

            var result = await _likeService.GetLikeAsync(l => l.Id == 1);

            Assert.AreEqual(like, result);
        }

        [Test]
        public void GetAll()
        {
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, TrackId = 101 },
                new Like { Id = 2, UserId = 2, TrackId = 102 }
            }.AsQueryable();

            _mockLikeRepository.Setup(r => r.GetAll()).Returns(likes);

            var result = _likeService.GetAll();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllLikesAsync()
        {
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, TrackId = 101 },
                new Like { Id = 2, UserId = 2, TrackId = 102 }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(likes);

            var result = await _likeService.GetAllLikesAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllLikesAsyncWithFilter()
        {
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, TrackId = 101 },
                new Like { Id = 2, UserId = 2, TrackId = 102 }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync((Expression<Func<Like, bool>> filter) => likes.Where(filter.Compile()).ToList());

            var result = await _likeService.GetAllLikesAsync(l => l.UserId == 1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().UserId);
        }

        [Test]
        public async Task UpdateLikeAsync()
        {
            var like = new Like { Id = 1, UserId = 1, TrackId = 101 };
            _mockLikeRepository.Setup(r => r.UpdateAsync(like)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _likeService.UpdateLikeAsync(like));
        }
    }
}

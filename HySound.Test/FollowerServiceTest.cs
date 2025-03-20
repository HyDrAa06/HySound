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

namespace HySound.Test
{
    [TestFixture]
    public class FollowerServiceTest
    {
        private Mock<IRepository<Followed>> _mockFollowerRepository;
        private IFollowerService _followerService;

        [SetUp]
        public void Setup()
        {
            _mockFollowerRepository = new Mock<IRepository<Followed>>();
            _followerService = new FollowerService(_mockFollowerRepository.Object);
        }

        [Test]
        public async Task AddFollowerAsync()
        {
            var followers = new List<Followed>();
            var follow = new Followed { FollowedById = 1, FollowedId = 2 };
            _mockFollowerRepository.Setup(r => r.AddAsync(follow)).Callback(() => followers.Add(follow));

            await _followerService.AddFollowerAsync(follow);

            Assert.AreEqual(1, followers.Count);
        }

        [Test]
        public async Task DeleteFollowerAsync()
        {
            var follow = new Followed { FollowedById = 1, FollowedId = 2 };
            var followers = new List<Followed> { follow };
            _mockFollowerRepository.Setup(r => r.DeleteAsync(follow)).Callback(() => followers.Remove(follow));

            await _followerService.DeleteFollowerAsync(follow);

            Assert.AreEqual(0, followers.Count);
        }

        [Test]
        public async Task DeleteFollowerByIdAsync()
        {
            _mockFollowerRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _followerService.DeleteFollowerByIdAsync(1));
        }

        [Test]
        public async Task GetFollowerByIdAsync()
        {
            int followerId = 1;
            var follow = new Followed { FollowedById = 1, FollowedId = 2 };
            _mockFollowerRepository.Setup(r => r.GetByIdAsync(followerId)).ReturnsAsync(follow);

            var result = await _followerService.GetFollowerByIdAsync(followerId);

            Assert.AreEqual(follow, result);
        }

        [Test]
        public async Task GetFollowerAsync()
        {
            var follow = new Followed { FollowedById = 1, FollowedId = 2 };
            _mockFollowerRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Followed, bool>>>()))
                .ReturnsAsync(follow);

            var result = await _followerService.GetFollowerAsync(f => f.FollowedById == 1);

            Assert.AreEqual(follow, result);
        }

        [Test]
        public void GetAll()
        {
            var followers = new List<Followed>
            {
                new Followed { FollowedById = 1, FollowedId = 2 },
                new Followed { FollowedById = 2, FollowedId = 3 }
            }.AsQueryable();

            _mockFollowerRepository.Setup(r => r.GetAll()).Returns(followers);

            var result = _followerService.GetAll();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllFollowersAsync()
        {
            var followers = new List<Followed>
            {
                new Followed { FollowedById = 1, FollowedId = 2 },
                new Followed { FollowedById = 2, FollowedId = 3 }
            };

            _mockFollowerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(followers);

            var result = await _followerService.GetAllFollowersAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllFollowersAsyncWithFilter()
        {
            var followers = new List<Followed>
            {
                new Followed { FollowedById = 1, FollowedId = 2 },
                new Followed { FollowedById = 2, FollowedId = 3 }
            };

            _mockFollowerRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Followed, bool>>>()))
                .ReturnsAsync((Expression<Func<Followed, bool>> filter) => followers.Where(filter.Compile()).ToList());

            var result = await _followerService.GetAllFollowersAsync(f => f.FollowedById == 1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().FollowedById);
        }

        [Test]
        public async Task UpdateFollowerAsync()
        {
            var follow = new Followed { FollowedById = 1, FollowedId = 2 };
            _mockFollowerRepository.Setup(r => r.UpdateAsync(follow)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _followerService.UpdateFollowerAsync(follow));
        }

        [Test]
        public void AllWithInclude()
        {
            var followers = new List<Followed>
            {
                new Followed { FollowedById = 1, FollowedId = 2 },
                new Followed { FollowedById = 2, FollowedId = 3 }
            }.AsQueryable();

            _mockFollowerRepository.Setup(r => r.GetAllQuery()).Returns(followers);

            var result = _followerService.AllWithInclude();

            Assert.AreEqual(2, result.Count());
        }
    }
}

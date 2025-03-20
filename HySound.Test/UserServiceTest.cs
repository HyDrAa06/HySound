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
    public class UserServiceTest
    {
        private Mock<IRepository<User>> _mockUserRepository;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<User>>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Test]
        public async Task AddUserAsync()
        {
            var users = new List<User>();
            var user = new User { Id = 1, Username = "User1" };
            _mockUserRepository.Setup(r => r.AddAsync(user)).Callback(() => users.Add(user));

            await _userService.AddUserAsync(user);

            Assert.AreEqual(1, users.Count);
        }

        [Test]
        public async Task DeleteUserAsync()
        {
            var user = new User { Id = 1, Username = "User1" };
            var users = new List<User> { user };
            _mockUserRepository.Setup(r => r.DeleteAsync(user)).Callback(() => users.Remove(user));

            await _userService.DeleteUserAsync(user);

            Assert.AreEqual(0, users.Count);
        }

        [Test]
        public async Task DeleteUserByIdAsync()
        {
            _mockUserRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _userService.DeleteUserByIdAsync(1));
        }

        [Test]
        public async Task GetUserByIdAsync()
        {
            int userId = 1;
            var user = new User { Id = userId, Username = "User1" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task GetUserByIdAsyncShouldReturnNullWhenItDoesNotExist()
        {
            User user = null;
            _mockUserRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllUsersAsync()
        {
            var users = new List<User> { new User { Id = 1, Username = "User1" }, new User { Id = 2, Username = "User2" } };
            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            Assert.AreEqual(users.Count, result.Count());
        }

        [Test]
        public async Task GetAllUsersAsyncWithFilter()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" }
            };

            _mockUserRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((Expression<Func<User, bool>> filter) => users.Where(filter.Compile()).ToList());

            var result = await _userService.GetAllUsersAsync(u => u.Username == "User1");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("User1", result.First().Username);
        }

        [Test]
        public async Task UpdateUserAsync()
        {
            var user = new User { Id = 1, Username = "User1" };
            _mockUserRepository.Setup(r => r.UpdateAsync(user)).Callback(() => user.Username = "UpdatedUser");

            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);

            user.Username = "UpdatedUser";
            await _userService.UpdateUserAsync(user);

            User result = await _userService.GetUserByIdAsync(user.Id);

            Assert.AreEqual("UpdatedUser", result.Username);
        }

        [Test]
        public void GetAll()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" }
            }.AsQueryable();

            _mockUserRepository.Setup(r => r.GetAll()).Returns(users);

            var result = _userService.GetAll();

            Assert.AreEqual(users.Count(), result.Count());
            Assert.AreEqual("User1", result.First().Username);
        }

        [Test]
        public void AllWithInclude()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" }
            }.AsQueryable();

            _mockUserRepository.Setup(r => r.GetAllQuery()).Returns(users);

            var result = _userService.AllWithInclude(u => u.Following);

            Assert.AreEqual(users.Count(), result.Count());
        }

        [Test]
        public async Task GetAllUserNamesAsync()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" }
            };

            _mockUserRepository.Setup(r => r.GetAll()).Returns(users.AsQueryable());

            var result = await _userService.GetAllUserNamesAsync();

            Assert.AreEqual(2, result.Count());
            Assert.Contains("User1", result.ToList());
            Assert.Contains("User2", result.ToList());
        }
    }
}

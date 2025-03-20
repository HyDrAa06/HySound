using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HySound.Test
{
    [TestFixture]
    public class ArtistRequestServiceTest
    {
        private Mock<IRepository<ArtistRequest>> _mockRequestRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private IArtistRequestService _artistRequestService;

        [SetUp]
        public void Setup()
        {
            _mockRequestRepository = new Mock<IRepository<ArtistRequest>>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _artistRequestService = new ArtistRequestService(
                _mockUserManager.Object,
                _mockUserRepository.Object,
                _mockRequestRepository.Object);
        }

        [Test]
        public async Task AddAsync()
        {
            var request = new ArtistRequest { Id = 1, ArtistUsername = "Artist1" };
            _mockRequestRepository.Setup(r => r.AddAsync(request)).Returns(Task.CompletedTask);

            var result = await _artistRequestService.AddAsync(request);

            Assert.AreEqual(request, result);
        }

        [Test]
        public async Task ApproveRequestAsync()
        {
            var request = new ArtistRequest { Id = 1, Status = "pending", IdentityUserId = "user1" };
            var identityUser = new IdentityUser { Id = "user1" };
            _mockRequestRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
            _mockUserManager.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(identityUser);
            _mockUserManager.Setup(um => um.IsInRoleAsync(identityUser, "Artist")).ReturnsAsync(false);
            _mockUserManager.Setup(um => um.IsInRoleAsync(identityUser, "User")).ReturnsAsync(true);
            _mockUserManager.Setup(um => um.AddToRoleAsync(identityUser, "Artist")).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.RemoveFromRoleAsync(identityUser, "User")).ReturnsAsync(IdentityResult.Success);
            _mockRequestRepository.Setup(r => r.UpdateAsync(request)).Returns(Task.CompletedTask);

            await _artistRequestService.ApproveRequestAsync(1, 2);

            Assert.AreEqual("approved", request.Status);
            Assert.AreEqual(2, request.AdminId);
        }

        [Test]
        public async Task DenyRequestAsync()
        {
            var request = new ArtistRequest { Id = 1, Status = "pending" };
            _mockRequestRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
            _mockRequestRepository.Setup(r => r.UpdateAsync(request)).Returns(Task.CompletedTask);

            await _artistRequestService.DenyRequestAsync(1, 2);

            Assert.AreEqual("denied", request.Status);
            Assert.AreEqual(2, request.AdminId);
        }

       

       

        [Test]
        public async Task SubmitArtistRequestAsync()
        {
            _mockRequestRepository.Setup(r => r.AddAsync(It.IsAny<ArtistRequest>())).Returns(Task.CompletedTask);

            var result = await _artistRequestService.SubmitArtistRequestAsync(1, "user1", "pfp.jpg", "Artist1", "artist@example.com", "Bio", "password123");

            Assert.AreEqual("Artist1", result.ArtistUsername);
            Assert.AreEqual("pending", result.Status);
        }

        [Test]
        public async Task UpdateAsync()
        {
            var request = new ArtistRequest { Id = 1, ArtistUsername = "Artist1" };
            _mockRequestRepository.Setup(r => r.UpdateAsync(request)).Returns(Task.CompletedTask);

            await _artistRequestService.UpdateAsync(request);

            Assert.IsNotNull(request);
        }
    }
}
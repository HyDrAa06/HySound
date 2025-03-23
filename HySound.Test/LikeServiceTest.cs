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
        private Mock<IRepository<Playlist>> _mockPlaylistRepository;
        private Mock<IRepository<PlaylistTrack>> _mockPlaylistTrackRepository;
        private Mock<IRepository<Track>> _mockTrackRepository;
        private Mock<IRepository<Like>> _mockLikeRepository;
        private IPlaylistService _playlistService;
        private ILikeService _likeService;

        [SetUp]
        public void Setup()
        {
            _mockPlaylistRepository = new Mock<IRepository<Playlist>>();
            _mockPlaylistTrackRepository = new Mock<IRepository<PlaylistTrack>>();
            _mockTrackRepository = new Mock<IRepository<Track>>();
            _mockLikeRepository = new Mock<IRepository<Like>>();
            _playlistService = new PlaylistService(_mockTrackRepository.Object, _mockPlaylistTrackRepository.Object, _mockPlaylistRepository.Object);
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
            var likes = new List<Like> { new Like { Id = 1, UserId = 1, TrackId = 101 } };
            _mockLikeRepository.Setup(r => r.DeleteByIdAsync(1)).Callback(() => likes.RemoveAt(0));
            _mockLikeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Like)null);

            await _likeService.DeleteLikeByIdAsync(1);
            var result = await _likeService.GetLikeByIdAsync(1);

            Assert.IsNull(result);
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
            _mockLikeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(like);

            await _likeService.UpdateLikeAsync(like);
            var updatedLike = await _likeService.GetLikeByIdAsync(1);

            Assert.AreEqual(like, updatedLike);
        }


        [Test]
        public async Task DeleteAllLikesByTracks()
        {
            int trackId = 101;
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, TrackId = trackId },
                new Like { Id = 2, UserId = 2, TrackId = trackId }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync(x => x.TrackId == trackId)).ReturnsAsync(likes);
            _mockLikeRepository.Setup(r => r.DeleteAsync(It.IsAny<Like>())).Returns(Task.CompletedTask);
            _mockLikeRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync((Expression<Func<Like, bool>> filter) => new List<Like>()); // After deletion, no likes remain

            await _likeService.DeleteAllLikesByTracks(trackId);
            var remainingLikes = await _likeService.GetAllLikesAsync(l => l.TrackId == trackId);

            Assert.AreEqual(0, remainingLikes.Count());
        }

        [Test]
        public async Task DeleteAllLikesByPlaylist()
        {
            int playlistId = 201;
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, PlaylistId = playlistId },
                new Like { Id = 2, UserId = 2, PlaylistId = playlistId }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync(x => x.PlaylistId == playlistId)).ReturnsAsync(likes);
            _mockLikeRepository.Setup(r => r.DeleteAsync(It.IsAny<Like>())).Returns(Task.CompletedTask);
            _mockLikeRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync((Expression<Func<Like, bool>> filter) => new List<Like>()); // After deletion, no likes remain

            await _likeService.DeleteAllLikesByPlaylist(playlistId);
            var remainingLikes = await _likeService.GetAllLikesAsync(l => l.PlaylistId == playlistId);

            Assert.AreEqual(0, remainingLikes.Count());
        }

        [Test]
        public async Task DeleteAllLikesByAlbum()
        {
            int albumId = 301;
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = 1, AlbumId = albumId },
                new Like { Id = 2, UserId = 2, AlbumId = albumId }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync(x => x.AlbumId == albumId)).ReturnsAsync(likes);
            _mockLikeRepository.Setup(r => r.DeleteAsync(It.IsAny<Like>())).Returns(Task.CompletedTask);
            _mockLikeRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync((Expression<Func<Like, bool>> filter) => new List<Like>()); // After deletion, no likes remain

            await _likeService.DeleteAllLikesByAlbum(albumId);
            var remainingLikes = await _likeService.GetAllLikesAsync(l => l.AlbumId == albumId);

            Assert.AreEqual(0, remainingLikes.Count());
        }

        [Test]
        public async Task DeleteAllLikesByUsers()
        {
            int userId = 1;
            var likes = new List<Like>
            {
                new Like { Id = 1, UserId = userId, TrackId = 101 },
                new Like { Id = 2, UserId = userId, PlaylistId = 201 }
            };

            _mockLikeRepository.Setup(r => r.GetAllAsync(x => x.UserId == userId)).ReturnsAsync(likes);
            _mockLikeRepository.Setup(r => r.DeleteAsync(It.IsAny<Like>())).Returns(Task.CompletedTask);
            _mockLikeRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Like, bool>>>()))
                .ReturnsAsync((Expression<Func<Like, bool>> filter) => new List<Like>()); // After deletion, no likes remain

            await _likeService.DeleteAllLikesByUsers(userId);
            var remainingLikes = await _likeService.GetAllLikesAsync(l => l.UserId == userId);

            Assert.AreEqual(0, remainingLikes.Count());
        }
    }
}
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
    public class PlaylistServiceTest
    {
        private Mock<IRepository<Playlist>> _mockPlaylistRepository;
        private Mock<IRepository<PlaylistTrack>> _mockPlaylistTrackRepository;
        private Mock<IRepository<Track>> _mockTrackRepository;
        private IPlaylistService _playlistService;

        [SetUp]
        public void Setup()
        {
            _mockPlaylistRepository = new Mock<IRepository<Playlist>>();
            _mockPlaylistTrackRepository = new Mock<IRepository<PlaylistTrack>>();
            _mockTrackRepository = new Mock<IRepository<Track>>();
            _playlistService = new PlaylistService(_mockTrackRepository.Object, _mockPlaylistTrackRepository.Object, _mockPlaylistRepository.Object);
        }

        [Test]
        public async Task AddPlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.AddAsync(playlist)).Returns(Task.CompletedTask);

            await _playlistService.AddPlaylistAsync(playlist);

            _mockPlaylistRepository.Verify(r => r.AddAsync(playlist), Times.Once);
        }

        [Test]
        public async Task DeletePlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.DeleteAsync(playlist)).Returns(Task.CompletedTask);

            await _playlistService.DeletePlaylistAsync(playlist);

            _mockPlaylistRepository.Verify(r => r.DeleteAsync(playlist), Times.Once);
        }

        [Test]
        public async Task DeletePlaylistByIdAsync()
        {
            int playlistId = 1;
            _mockPlaylistRepository.Setup(r => r.DeleteByIdAsync(playlistId)).Returns(Task.CompletedTask);

            await _playlistService.DeletePlaylistByIdAsync(playlistId);

            _mockPlaylistRepository.Verify(r => r.DeleteByIdAsync(playlistId), Times.Once);
        }

        [Test]
        public async Task GetPlaylistByIdAsync()
        {
            int playlistId = 1;
            var playlist = new Playlist { Id = playlistId, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.GetByIdAsync(playlistId)).ReturnsAsync(playlist);

            var result = await _playlistService.GetPlaylistByIdAsync(playlistId);

            Assert.AreEqual(playlist, result);
        }

        [Test]
        public async Task GetPlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Playlist, bool>>>()))
                .ReturnsAsync(playlist);

            var result = await _playlistService.GetPlaylistAsync(p => p.Id == 1);

            Assert.AreEqual(playlist, result);
        }


        [Test]
        public void GetAll()
        {
            var playlists = new List<Playlist>
            {
                new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 },
                new Playlist { Id = 2, Title = "Playlist 2", CoverImage = "cover2.jpg", Description = "Description 2", UserId = 1 }
            }.AsQueryable();

            _mockPlaylistRepository.Setup(r => r.GetAll()).Returns(playlists);

            var result = _playlistService.GetAll();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllPlaylistsAsync()
        {
            var playlists = new List<Playlist>
            {
                new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 },
                new Playlist { Id = 2, Title = "Playlist 2", CoverImage = "cover2.jpg", Description = "Description 2", UserId = 1 }
            };

            _mockPlaylistRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(playlists);

            var result = await _playlistService.GetAllPlaylistsAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllPlaylistsAsyncWithFilter()
        {
            var playlists = new List<Playlist>
            {
                new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 },
                new Playlist { Id = 2, Title = "Playlist 2", CoverImage = "cover2.jpg", Description = "Description 2", UserId = 1 }
            };

            _mockPlaylistRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Playlist, bool>>>()))
                .ReturnsAsync((Expression<Func<Playlist, bool>> filter) => playlists.Where(filter.Compile()).ToList());

            var result = await _playlistService.GetAllPlaylistsAsync(p => p.UserId == 1);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task AddTrackToPlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1" };
            var track = new Track { Id = 1, Title = "Track 1" };

            _mockPlaylistTrackRepository.Setup(r => r.AddAsync(It.IsAny<PlaylistTrack>())).Returns(Task.CompletedTask);

            await _playlistService.AddTrackToPlaylistAsync(playlist, track);

            _mockPlaylistTrackRepository.Verify(r => r.AddAsync(It.IsAny<PlaylistTrack>()), Times.Once);
        }

        [Test]
        public async Task GetTracksOfPlaylist()
        {
            int playlistId = 1;
            var playlistTracks = new List<PlaylistTrack>
            {
                new PlaylistTrack { PlaylistId = playlistId, TrackId = 1 },
                new PlaylistTrack { PlaylistId = playlistId, TrackId = 2 }
            };

            var tracks = new List<Track>
            {
                new Track { Id = 1, Title = "Track 1" },
                new Track { Id = 2, Title = "Track 2" }
            };

            _mockPlaylistTrackRepository.Setup(r => r.GetAll()).Returns(playlistTracks.AsQueryable());
            _mockTrackRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => tracks.FirstOrDefault(t => t.Id == id));

            var result = await _playlistService.GetTracksOfPlaylist(playlistId);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task UpdatePlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.UpdateAsync(playlist)).Returns(Task.CompletedTask);

            await _playlistService.UpdatePlaylistAsync(playlist);

            _mockPlaylistRepository.Verify(r => r.UpdateAsync(playlist), Times.Once);
        }

        [Test]
        public void AllWithInclude()
        {
            var playlists = new List<Playlist>
            {
                new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 },
                new Playlist { Id = 2, Title = "Playlist 2", CoverImage = "cover2.jpg", Description = "Description 2", UserId = 1 }
            }.AsQueryable();

            _mockPlaylistRepository.Setup(r => r.GetAllQuery()).Returns(playlists);

            var result = _playlistService.AllWithInclude(p => p.User);

            Assert.AreEqual(2, result.Count());
        }
    }
}

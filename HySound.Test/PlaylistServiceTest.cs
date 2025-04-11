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
            _mockPlaylistRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(playlist);

            await _playlistService.AddPlaylistAsync(playlist);

            var retrievedPlaylist = await _playlistService.GetPlaylistByIdAsync(1);
            Assert.AreEqual(playlist, retrievedPlaylist);
        }

        [Test]
        public async Task DeletePlaylistAsync()
        {
            var playlist = new Playlist { Id = 1, Title = "Playlist 1", CoverImage = "cover1.jpg", Description = "Description 1", UserId = 1 };
            _mockPlaylistRepository.Setup(r => r.DeleteAsync(playlist)).Returns(Task.CompletedTask);
            _mockPlaylistRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Playlist)null); // After deletion, it should be null

            await _playlistService.DeletePlaylistAsync(playlist);
            var result = await _playlistService.GetPlaylistByIdAsync(1);

            Assert.IsNull(result);
        }

        [Test]
        public async Task DeletePlaylistByIdAsync()
        {
            int playlistId = 1;
            _mockPlaylistRepository.Setup(r => r.DeleteByIdAsync(playlistId)).Returns(Task.CompletedTask);
            _mockPlaylistRepository.Setup(r => r.GetByIdAsync(playlistId)).ReturnsAsync((Playlist)null); // After deletion, it should be null

            await _playlistService.DeletePlaylistByIdAsync(playlistId);
            var result = await _playlistService.GetPlaylistByIdAsync(playlistId);

            Assert.IsNull(result);
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
            var playlist = new Playlist { Id = 1 };
            var track = new Track { Id = 2 };
            var playlistTrack = new PlaylistTrack { PlaylistId = playlist.Id, TrackId = track.Id };

            _mockPlaylistTrackRepository.Setup(repo => repo.AddAsync(It.IsAny<PlaylistTrack>()))
                .Returns(Task.CompletedTask)
                .Callback<PlaylistTrack>(pt => playlistTrack = pt);

            await _playlistService.AddTrackToPlaylistAsync(playlist, track);

            Assert.AreEqual(playlist.Id, playlistTrack.PlaylistId);
            Assert.AreEqual(track.Id, playlistTrack.TrackId);
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
            _mockPlaylistRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(playlist);

            await _playlistService.UpdatePlaylistAsync(playlist);
            var updatedPlaylist = await _playlistService.GetPlaylistByIdAsync(1);

            Assert.AreEqual(playlist, updatedPlaylist);
        }

        [Test]
        public async Task DeleteTrackFromAllPlaylistsAsync()
        {
            int trackId = 1;
            var playlistTracks = new List<PlaylistTrack>
            {
                new PlaylistTrack { TrackId = trackId, PlaylistId = 1 },
                new PlaylistTrack { TrackId = trackId, PlaylistId = 2 }
            };

            _mockPlaylistTrackRepository.Setup(r => r.GetAllAsync(x => x.TrackId == trackId)).ReturnsAsync(playlistTracks);
            _mockPlaylistTrackRepository.Setup(r => r.DeleteAsync(It.IsAny<PlaylistTrack>())).Returns(Task.CompletedTask);
            _mockPlaylistTrackRepository.Setup(r => r.GetAll()).Returns(new List<PlaylistTrack>().AsQueryable());

            await _playlistService.DeleteTrackFromAllPlaylistsAsync(trackId);
            var tracks = await _playlistService.GetTracksOfPlaylist(1);

            Assert.AreEqual(0, tracks.Count);
        }

        [Test]
        public async Task DeleteAllPlaylistsOfUser()
        {
            int userId = 1;
            var playlists = new List<Playlist>
            {
                new Playlist { Id = 1, UserId = userId },
                new Playlist { Id = 2, UserId = userId }
            };

            _mockPlaylistRepository.Setup(r => r.GetAllAsync(x => x.UserId == userId)).ReturnsAsync(playlists);
            _mockPlaylistRepository.Setup(r => r.DeleteAsync(It.IsAny<Playlist>())).Returns(Task.CompletedTask);
            _mockPlaylistRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Playlist>());

            await _playlistService.DeleteAllPlaylistsOfUser(userId);
            var remainingPlaylists = await _playlistService.GetAllPlaylistsAsync();

            Assert.AreEqual(0, remainingPlaylists.Count());
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
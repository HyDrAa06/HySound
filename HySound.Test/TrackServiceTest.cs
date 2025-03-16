using HySound.Core.Service;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
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
    public class TrackServiceTest
    {
        private Mock<IRepository<Track>> _mockTrackRepository;
        private TrackService _trackService;

        [SetUp]
        public void Setup()
        {
            _mockTrackRepository = new Mock<IRepository<Track>>();
            _trackService = new TrackService(_mockTrackRepository.Object);
        }

        [Test]
        public async Task AddTrackAsync()
        {
            var tracks = new List<Track>();
            var track = new Track { Id = 1, Title = "Song" };
            _mockTrackRepository.Setup(x => x.AddAsync(track)).Callback(() => tracks.Add(track));

            await _trackService.AddTrackAsync(track);

            Assert.AreEqual(1, tracks.Count);
        }

        [Test]
        public async Task GetTrackByIdAsync()
        {
            var track = new Track { Id = 1, Title = "Song" };

            _mockTrackRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(track);

            var result = await _trackService.GetTrackByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Song", result.Title);
        }

        [Test]
        public async Task GetTrackByIdAsyncAndReturnNullWhenTrackDoesNotExist()
        {
            Track track = null;

            _mockTrackRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync(track);

            var result = await _trackService.GetTrackByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllTracksAsync()
        {
            var tracks = new List<Track>
            {
                new Track { Id = 1, Title = "Song" },
                new Track { Id = 2, Title = "Song2" }
            };

            _mockTrackRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(tracks);

            var result = await _trackService.GetAllTracksAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Song", result.First().Title);
        }

        [Test]
        public async Task DeleteTrackByIdAsync()
        {
            var track = new Track { Id = 1, Title = "Song" };
            var tracks = new List<Track> { track };
            _mockTrackRepository.Setup(r => r.DeleteAsync(track)).Callback(() => tracks.Remove(track));

            await _trackService.DeleteTrackAsync(track);

            Assert.AreEqual(0, tracks.Count);
        }

        [Test]
        public async Task UpdateTrackAsync()
        {
            var track = new Track { Id = 1, Title = "Song" };
            _mockTrackRepository.Setup(r => r.UpdateAsync(track)).Callback(() => track.Title = "Song2");

            _mockTrackRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(track);

            track.Title = "Song2";
            await _trackService.UpdateTrackAsync(track);

            Track result = await _trackService.GetTrackByIdAsync(track.Id);

            Assert.AreEqual("Song2", result.Title);
        }
    }
}

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
    public class TrackServiceTest
    {
        private Mock<IRepository<Track>> _mockTrackRepository;
        private ITrackService _trackService;

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
            var track = new Track { Id = 1, Title = "Track1" };
            _mockTrackRepository.Setup(r => r.AddAsync(track)).Callback(() => tracks.Add(track));

            await _trackService.AddTrackAsync(track);

            Assert.AreEqual(1, tracks.Count);
        }

        [Test]
        public async Task DeleteTrackAsync()
        {
            var track = new Track { Id = 1, Title = "Track1" };
            var tracks = new List<Track> { track };
            _mockTrackRepository.Setup(r => r.DeleteAsync(track)).Callback(() => tracks.Remove(track));

            await _trackService.DeleteTrackAsync(track);

            Assert.AreEqual(0, tracks.Count);
        }

        [Test]
        public async Task DeleteTrackByIdAsync()
        {
            _mockTrackRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _trackService.DeleteTrackByIdAsync(1));
        }

        [Test]
        public async Task GetTrackByIdAsync()
        {
            int trackId = 1;
            var track = new Track { Id = trackId, Title = "Track1" };
            _mockTrackRepository.Setup(r => r.GetByIdAsync(trackId)).ReturnsAsync(track);

            var result = await _trackService.GetTrackByIdAsync(trackId);

            Assert.AreEqual(track, result);
        }

        [Test]
        public async Task GetTrackByIdAsyncShouldReturnNullWhenItDoesNotExist()
        {
            Track track = null;
            _mockTrackRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync(track);

            var result = await _trackService.GetTrackByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllTracksAsync()
        {
            var tracks = new List<Track> { new Track { Id = 1, Title = "Track1" }, new Track { Id = 2, Title = "Track2" } };
            _mockTrackRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tracks);

            var result = await _trackService.GetAllTracksAsync();

            Assert.AreEqual(tracks.Count, result.Count());
        }

        [Test]
        public async Task GetAllTracksAsyncWithFilter()
        {
            var tracks = new List<Track>
            {
                new Track { Id = 1, Title = "Track1" },
                new Track { Id = 2, Title = "Track2" }
            };
            _mockTrackRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Track, bool>>>()))
                .ReturnsAsync((Expression<Func<Track, bool>> filter) => tracks.Where(filter.Compile()).ToList());

            var result = await _trackService.GetAllTracksAsync(t => t.Title == "Track1");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Track1", result.First().Title);
        }

        [Test]
        public async Task UpdateTrackAsync()
        {
            var track = new Track { Id = 1, Title = "Track1" };
            _mockTrackRepository.Setup(r => r.UpdateAsync(track)).Callback(() => track.Title = "UpdatedTrack");

            _mockTrackRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(track);

            track.Title = "UpdatedTrack";
            await _trackService.UpdateTrackAsync(track);

            Track result = await _trackService.GetTrackByIdAsync(track.Id);

            Assert.AreEqual("UpdatedTrack", result.Title);
        }

        [Test]
        public void GetAll()
        {
            var tracks = new List<Track>
            {
                new Track { Id = 1, Title = "Track1" },
                new Track { Id = 2, Title = "Track2" }
            }.AsQueryable();

            _mockTrackRepository.Setup(r => r.GetAll()).Returns(tracks);

            var result = _trackService.GetAll();

            Assert.AreEqual(tracks.Count(), result.Count());
            Assert.AreEqual("Track1", result.First().Title);
        }

        [Test]
        public void AllWithInclude()
        { 
            var tracks = new List<Track>
            {
                new Track { Id = 1, Title = "Track1" },
                new Track { Id = 2, Title = "Track2" }
            }.AsQueryable();

            _mockTrackRepository.Setup(r => r.GetAllQuery()).Returns(tracks);

            var result = _trackService.AllWithInclude(t => t.User);

            Assert.AreEqual(tracks.Count(), result.Count());
        }
    }
}

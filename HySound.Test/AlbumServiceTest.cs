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
    public class AlbumServiceTest
    {
        private Mock<IRepository<Album>> _mockAlbumRepository;
        private IAlbumService _albumService;

        [SetUp]
        public void Setup()
        {
            _mockAlbumRepository = new Mock<IRepository<Album>>();
            _albumService = new AlbumService(_mockAlbumRepository.Object);
        }

        [Test]
        public async Task AddAlbumAsync()
        {
            var albums = new List<Album>();
            var album = new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg", };
            _mockAlbumRepository.Setup(r => r.AddAsync(album)).Callback(() => albums.Add(album));

            await _albumService.AddAlbumAsync(album);

            Assert.AreEqual(1, albums.Count);
        }

        [Test]
        public async Task DeleteAlbumAsync()
        {
            var album = new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg"};
            var albums = new List<Album> { album };
            _mockAlbumRepository.Setup(r => r.DeleteAsync(album)).Callback(() => albums.Remove(album));

            await _albumService.DeleteAlbumAsync(album);

            Assert.AreEqual(0, albums.Count);
        }

        [Test]
        public async Task DeleteAlbumByIdAsync()
        {
            _mockAlbumRepository.Setup(r => r.DeleteByIdAsync(1)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _albumService.DeleteAlbumByIdAsync(1));
        }

        [Test]
        public async Task GetAlbumByIdAsync()
        {
            int albumId = 1;
            var album = new Album { Id = albumId, Title = "Album 1", CoverImage = "image1.jpg" };
            _mockAlbumRepository.Setup(r => r.GetByIdAsync(albumId)).ReturnsAsync(album);

            var result = await _albumService.GetAlbumByIdAsync(albumId);

            Assert.AreEqual(album, result);
        }

        [Test]
        public async Task GetAlbumAsync()
        {
            var album = new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg" };
            _mockAlbumRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Album, bool>>>()))
                .ReturnsAsync(album);

            var result = await _albumService.GetAlbumAsync(a => a.Id == 1);

            Assert.AreEqual(album, result);
        }

        [Test]
        public void GetAll()
        {
            var albums = new List<Album>
            {
                new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg"},
                new Album { Id = 2, Title = "Album 2", CoverImage = "image2.jpg"}
            }.AsQueryable();

            _mockAlbumRepository.Setup(r => r.GetAll()).Returns(albums);

            var result = _albumService.GetAll();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllAlbumsAsync()
        {
            var albums = new List<Album>
            {
                new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg"},
                new Album { Id = 2, Title = "Album 2", CoverImage = "image2.jpg" }
            };

            _mockAlbumRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(albums);

            var result = await _albumService.GetAllAlbumAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllAlbumsAsyncWithFilter()
        {
            var albums = new List<Album>
            {
                new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg"},
                new Album { Id = 2, Title = "Album 2", CoverImage = "image2.jpg" }
            };

            _mockAlbumRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Album, bool>>>()))
                .ReturnsAsync((Expression<Func<Album, bool>> filter) => albums.Where(filter.Compile()).ToList());

            var result = await _albumService.GetAllAlbumAsync(a => a.Id == 1);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Id);
        }

        [Test]
        public async Task UpdateAlbumAsync()
        {
            var album = new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg" };
            _mockAlbumRepository.Setup(r => r.UpdateAsync(album)).Returns(Task.CompletedTask);

            Assert.DoesNotThrowAsync(async () => await _albumService.UpdateAlbumAsync(album));
        }

        [Test]
        public void AllWithInclude()
        {
            var albums = new List<Album>
            {
                new Album { Id = 1, Title = "Album 1", CoverImage = "image1.jpg"},
                new Album { Id = 2, Title = "Album 2", CoverImage = "image2.jpg"}
            }.AsQueryable();

            _mockAlbumRepository.Setup(r => r.GetAllQuery()).Returns(albums);

            var result = _albumService.AllWithInclude();

            Assert.AreEqual(2, result.Count());
        }
    }
}

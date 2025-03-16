using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HySound.Core.Service;
using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;

namespace HySound.Tests
{
    [TestFixture]
    public class GenreServiceTest
    {
        private Mock<IRepository<Genre>> _mockGenreRepository;
        private IGenreService _genreService;

        [SetUp]
        public void Setup()
        {
            _mockGenreRepository = new Mock<IRepository<Genre>>();
            _genreService = new GenreService(_mockGenreRepository.Object);
        }


        [Test]
        public async Task GetAllGenresAsync()
        {
            var genres = new List<Genre> { new Genre { Id = 1, Name = "Rock" }, new Genre { Id = 2, Name = "Pop" } };
            _mockGenreRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(genres);

            var result = await _genreService.GetAllGenresAsync();

            Assert.AreEqual(genres.Count, result.Count());
        }

        [Test]
        public async Task GetGenreByIdAsync()
        {
            int genreId = 1;
            var genre = new Genre { Id = genreId, Name = "Jazz" };
            _mockGenreRepository.Setup(x => x.GetByIdAsync(genreId)).ReturnsAsync(genre);

            var result = await _genreService.GetGenreByIdAsync(genreId);

            Assert.AreEqual(genre, result);
        }

        [Test]
        public async Task GetAllGenresAsyncWithFilter()
        {
            var genres = new List<Genre>
            {
                new Genre { Id = 1, Name = "Rock" },
                new Genre { Id = 2, Name = "Pop" }
            };

            _mockGenreRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync((Expression<Func<Genre, bool>> filter) => genres.Where(filter.Compile()).ToList());

            var result = await _genreService.GetAllGenresAsync(x => x.Name == "Rock");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Rock", result.First().Name);
        }
    }
}

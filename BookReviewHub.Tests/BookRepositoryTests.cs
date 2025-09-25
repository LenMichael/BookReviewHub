using Xunit;
using BookReviewHub.Repositories.Implementations;
using BookReviewHub.Models;
using BookReviewHub.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace BookReviewHub.Tests
{
    public class BookRepositoryTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsBook()
        {
            using var context = GetDbContext();
            var repo = new BookRepository(context);
            var book = new Book { Title = "Test", Author = "Author", PublishedYear = 2020, Genre = "Fiction" };

            await repo.AddAsync(book);

            Assert.Equal(1, context.Books.Count());
            Assert.Equal("Test", context.Books.First().Title);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBooks()
        {
            using var context = GetDbContext();
            context.Books.Add(new Book { Title = "A", Author = "B", PublishedYear = 2020, Genre = "Fiction" });
            context.Books.Add(new Book { Title = "C", Author = "D", PublishedYear = 2021, Genre = "Drama" });
            context.SaveChanges();

            var repo = new BookRepository(context);
            var books = await repo.GetAllAsync();

            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBook_WhenExists()
        {
            using var context = GetDbContext();
            var book = new Book { Title = "A", Author = "B", PublishedYear = 2020, Genre = "Fiction" };
            context.Books.Add(book);
            context.SaveChanges();

            var repo = new BookRepository(context);
            var found = await repo.GetByIdAsync(book.Id);

            Assert.NotNull(found);
            Assert.Equal("A", found.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            using var context = GetDbContext();
            var repo = new BookRepository(context);

            var found = await repo.GetByIdAsync(123);

            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesBook()
        {
            using var context = GetDbContext();
            var book = new Book { Title = "Old", Author = "B", PublishedYear = 2020, Genre = "Fiction" };
            context.Books.Add(book);
            context.SaveChanges();

            var repo = new BookRepository(context);
            book.Title = "New";
            await repo.UpdateAsync(book);

            Assert.Equal("New", context.Books.First().Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBook()
        {
            using var context = GetDbContext();
            var book = new Book { Title = "A", Author = "B", PublishedYear = 2020, Genre = "Fiction" };
            context.Books.Add(book);
            context.SaveChanges();

            var repo = new BookRepository(context);
            await repo.DeleteAsync(book.Id);

            Assert.Empty(context.Books);
        }

        [Fact]
        public async Task GetFilteredAsync_FiltersByGenreYearRating()
        {
            using var context = GetDbContext();
            var book1 = new Book { Title = "A", Author = "B", PublishedYear = 2020, Genre = "Fiction", Reviews = new List<Review> { new Review { Rating = 5 } } };
            var book2 = new Book { Title = "C", Author = "D", PublishedYear = 2021, Genre = "Drama", Reviews = new List<Review> { new Review { Rating = 3 } } };
            context.Books.AddRange(book1, book2);
            context.SaveChanges();

            var repo = new BookRepository(context);
            var filtered = await repo.GetFilteredAsync("Fiction", 2020, 5);

            Assert.Single(filtered);
            Assert.Equal("A", filtered.First().Title);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenExists()
        {
            using var context = GetDbContext();
            var book = new Book { Title = "A", Author = "B", PublishedYear = 2020, Genre = "Fiction" };
            context.Books.Add(book);
            context.SaveChanges();

            var repo = new BookRepository(context);
            var exists = await repo.ExistsAsync(book.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenNotExists()
        {
            using var context = GetDbContext();
            var repo = new BookRepository(context);

            var exists = await repo.ExistsAsync(999);

            Assert.False(exists);
        }
    }
}

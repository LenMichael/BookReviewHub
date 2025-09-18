using BookReviewHub.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReviewHub.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            if (context.Books.Any())
                return;

            var books = new List<Book>
            {
                new Book { Title = "The Name of the Rose", Author = "Umberto Eco", PublishedYear = 1980, Genre = "Novel" },
                new Book { Title = "The Little Prince", Author = "Antoine de Saint-Exupéry", PublishedYear = 1943, Genre = "Children" }
            };
            context.Books.AddRange(books);
            context.SaveChanges();

            var reviews = new List<Review>
            {
                new Review { Content = "Excellent Book!", Rating = 5, BookId = books[0].Id, UserId = "seed-user" },
                new Review { Content = "Very touching and timeless.", Rating = 4, BookId = books[1].Id, UserId = "seed-user" }
            };
            context.Reviews.AddRange(reviews);
            context.SaveChanges();
        }
    }
}

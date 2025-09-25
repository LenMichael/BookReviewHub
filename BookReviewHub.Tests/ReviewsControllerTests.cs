using Xunit;
using Moq;
using BookReviewHub.Controllers;
using BookReviewHub.Repositories.Interfaces;
using BookReviewHub.Models;
using BookReviewHub.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BookReviewHub.Tests
{
    public class ReviewsControllerTests
    {
        private readonly Mock<IReviewRepository> _reviewRepoMock;
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly ReviewsController _controller;

        public ReviewsControllerTests()
        {
            _reviewRepoMock = new Mock<IReviewRepository>();
            _bookRepoMock = new Mock<IBookRepository>();
            _controller = new ReviewsController(_reviewRepoMock.Object, _bookRepoMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithReviews()
        {
            // Arrange
            _reviewRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Review>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Review>>(viewResult.Model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenReviewNotFound()
        {
            _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Review)null);

            var result = await _controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenReviewExists()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(review, viewResult.Model);
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenUserNotLoggedIn()
        {
            var result = await _controller.Create(new ReviewCreateDto());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsViewWithErrors_WhenModelStateInvalid()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user1") }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            _controller.ModelState.AddModelError("Content", "Required");
            var book = new Book { Id = 1 };
            _bookRepoMock.Setup(b => b.GetByIdAsync(1)).ReturnsAsync(book);

            var result = await _controller.Create(new ReviewCreateDto { BookId = 1 });

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Books/Reviews.cshtml", viewResult.ViewName);
            Assert.Equal(book, viewResult.Model);
        }

        [Fact]
        public async Task Create_AddsReviewAndRedirects_WhenModelValid()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user1") }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var dto = new ReviewCreateDto { Content = "test", Rating = 5, BookId = 1 };

            var result = await _controller.Create(dto);

            _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Reviews", redirect.ActionName);
            Assert.Equal("Books", redirect.ControllerName);
            Assert.Equal(1, redirect.RouteValues["id"]);
        }

        [Fact]
        public async Task EditGet_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGet_ReturnsNotFound_WhenReviewNotFound()
        {
            _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Review)null);
            var result = await _controller.Edit(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditGet_ReturnsView_WhenReviewExists()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var result = await _controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(review, viewResult.Model);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFound_WhenIdMismatch()
        {
            var review = new Review { Id = 2 };
            var result = await _controller.Edit(1, review);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditPost_ReturnsNotFound_WhenReviewNotFound()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Review)null);
            var result = await _controller.Edit(1, review);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditPost_ReturnsView_WhenModelStateInvalid()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            _controller.ModelState.AddModelError("Content", "Required");
            var result = await _controller.Edit(1, review);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(review, viewResult.Model);
        }

        [Fact]
        public async Task EditPost_UpdatesReviewAndRedirects_WhenValid()
        {
            var review = new Review { Id = 1, UserId = "user1" };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var updatedReview = new Review { Id = 1, Content = "new", Rating = 5, UserId = "user1" };
            var result = await _controller.Edit(1, updatedReview);
            _reviewRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteGet_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGet_ReturnsNotFound_WhenReviewNotFound()
        {
            _reviewRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Review)null);
            var result = await _controller.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGet_ReturnsView_WhenReviewExists()
        {
            var review = new Review { Id = 1 };
            _reviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var result = await _controller.Delete(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(review, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesReviewAndRedirects()
        {
            var result = await _controller.DeleteConfirmed(1);
            _reviewRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

    }
}

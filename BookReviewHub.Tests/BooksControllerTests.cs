using Xunit;
using Moq;
using BookReviewHub.Controllers;
using BookReviewHub.Repositories.Interfaces;
using BookReviewHub.Models;
using BookReviewHub.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BooksControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewWithBooks()
    {
        // Arrange
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetFilteredAsync(null, null, null))
            .ReturnsAsync(new List<Book> { new Book { Title = "Test Book" } });

        var controller = new BooksController(mockRepo.Object);

        // Act
        var result = await controller.Index(null, null, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnsViewWithModel()
    {
        var mockRepo = new Mock<IBookRepository>();
        var controller = new BooksController(mockRepo.Object);
        controller.ModelState.AddModelError("Title", "Required");

        var dto = new BookCreateDto { Author = "A", PublishedYear = 2024, Genre = "Fiction" };

        var result = await controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(dto, viewResult.Model);
    }

    [Fact]
    public async Task Details_BookNotFound_ReturnsNotFound()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Book)null);
        var controller = new BooksController(mockRepo.Object);

        var result = await controller.Details(123);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_ValidBook_DeletesAndRedirects()
    {
        var mockRepo = new Mock<IBookRepository>();
        var book = new Book { Id = 1, UserId = "user1" };
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var controller = new BooksController(mockRepo.Object);
        var user = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity(
                new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user1") }
            )
        );
        controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user } };

        var result = await controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }


}

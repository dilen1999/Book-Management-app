// tests/BookMS.Tests/Controllers/BooksControllerTests.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookMS.Application.Book.Queries.GetBookById;
using BookMS.Application.Book.Queries.GetBooksPaged;
using BookMS.Application.DTOs;
using BookMS.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using BookMS.Application.Books.Queries.GetBooksCount;
using BookMS.Application.Books.Commands.CreateBook;

namespace BookMS.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mediator = new Mock<IMediator>(MockBehavior.Strict);
            _controller = new BooksController(_mediator.Object);
        }

        [Fact]
        public async Task Get_List_Of_Book()
        {
            var page = 1;
            var pageSize = 2;

            var expected = new List<BookDto>
            {
                new BookDto(Guid.NewGuid(), "Book One", "111", 2020),
                new BookDto(Guid.NewGuid(), "Book Two", "222", 2021)
            } as IReadOnlyList<BookDto>;


            _mediator
                .Setup(m => m.Send<IReadOnlyList<BookDto>>(It.IsAny<GetBooksPagedQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actionResult = await _controller.Get(page, pageSize);

            var ok = actionResult as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);

            var payload = ok.Value as IReadOnlyList<BookDto>;
            payload.Should().NotBeNull();
            payload.Should().BeEquivalentTo(expected);

            _mediator.Verify(m => m.Send<IReadOnlyList<BookDto>>(It.Is<GetBooksPagedQuery>(q => q.Page == page && q.PageSize == pageSize),
                                                                  It.IsAny<CancellationToken>()), Times.Once);
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetById_Book()
        {
            var bookId = Guid.NewGuid();

            _mediator
                .Setup(m => m.Send(It.Is<GetBookByIdQuery>(q => q.Id == bookId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookDto?)null);

            var actionResult = await _controller.GetById(bookId);

            var notFound = actionResult as NotFoundResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);

            _mediator.Verify(m => m.Send(It.Is<GetBookByIdQuery>(q => q.Id == bookId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_Book()
        {
            var cmd = new CreateBookCommand(
                "New Book",
                "999",
                2025,
                new List<Guid>(),     
                new List<Guid>()      
            );


            var created = new BookDto(Guid.NewGuid(), cmd.Title, cmd.Isbn, cmd.PublishedYear);

            _mediator
                .Setup(m => m.Send(cmd, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var actionResult = await _controller.Create(cmd);

            var createdResult = actionResult as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(StatusCodes.Status201Created);

            var payload = createdResult.Value as BookDto;
            payload.Should().NotBeNull();
            payload.Should().BeEquivalentTo(created);

            _mediator.Verify(m => m.Send(cmd, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCount_TotalCount()
        {
            // Arrange
            var expectedCount = 42;

            _mediator
                .Setup(m => m.Send(It.IsAny<GetBooksCountQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCount);

            // Act
            var actionResult = await _controller.GetCount();

            // Assert
            var ok = actionResult.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(StatusCodes.Status200OK);

            // payload is an anonymous type { totalCount = int }
            var payload = ok.Value;
            payload.Should().BeEquivalentTo(new { totalCount = expectedCount });

            _mediator.Verify(m => m.Send(It.IsAny<GetBooksCountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

// tests/BookMS.Tests/Controllers/BooksControllerTests.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookMS.Application.Book.Queries.GetBooksPaged;
using BookMS.Application.DTOs;
using BookMS.WebApi.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

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
        public async Task Get_Returns_200_And_List_Of_BookDto()
        {
            // Arrange
            var page = 1;
            var pageSize = 2;

            var expected = new List<BookDto>
            {
                new BookDto(Guid.NewGuid(), "Book One", "111", 2020),
                new BookDto(Guid.NewGuid(), "Book Two", "222", 2021)
            } as IReadOnlyList<BookDto>;


            // IMPORTANT: specify the generic return type explicitly
            _mediator
                .Setup(m => m.Send<IReadOnlyList<BookDto>>(It.IsAny<GetBooksPagedQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var actionResult = await _controller.Get(page, pageSize);

            // Assert
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
    }
}

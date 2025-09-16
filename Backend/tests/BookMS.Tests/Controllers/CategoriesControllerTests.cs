using System.Threading;
using System.Threading.Tasks;
using BookMS.Application.Categories.Queries.GetCategoryCount;
using BookMS.WebApi.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookMS.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mediator = new Mock<IMediator>(MockBehavior.Strict);
            _controller = new CategoriesController(_mediator.Object);
        }

        [Fact]
        public async Task GetCount_CategoryCount()
        {
            var expectedCount = 5;
            _mediator
                .Setup(m => m.Send(It.IsAny<GetCategoryCountQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCount);

            var actionResult = await _controller.GetCount();

            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var payload = okResult.Value as int?;
            payload.Should().NotBeNull();
            payload.Should().Be(expectedCount);

            _mediator.Verify(m => m.Send(It.IsAny<GetCategoryCountQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.VerifyNoOtherCalls();
        }
    }
}

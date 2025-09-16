using System;
using System.Threading;
using System.Threading.Tasks;
using BookMS.Application.Auth.Commands.GoogleSignIn;
using BookMS.Application.Auth.Commands.Login;
using BookMS.Application.Auth.Commands.Refresh;
using BookMS.Application.Auth.Commands.Register;
using BookMS.Application.Auth.Commands.Revoke;
using BookMS.Application.Auth.Dtos;
using BookMS.WebApi.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookMS.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediator = new Mock<IMediator>(MockBehavior.Strict);
            _controller = new AuthController(_mediator.Object);
        }

        [Fact]
        public async Task Register_User()
        {
            var req = new RegisterRequest { Email = "test@test.com", Name = "Test", Password = "pwd", Role = "User" };
            var expected = new AuthResponseDto(Guid.NewGuid(), req.Email, req.Name, req.Role, "access123", "refresh123");

            _mediator
                .Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.Register(req);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);

            _mediator.Verify(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_Credentials()
        {
            var req = new LoginRequest("test@test.com", "pwd");
            var expected = new AuthResponseDto(Guid.NewGuid(), req.Email, "Test", "User", "access123", "refresh123");

            _mediator
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.Login(req);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);

            _mediator.Verify(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshToken()
        {
            var req = new RefreshRequest { RefreshToken = "" };

            var result = await _controller.Refresh(req);

            var bad = result.Result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);

            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Returns__RefreshToken_Is_Valid()
        {
            var req = new RefreshRequest { RefreshToken = "refresh123" };
            var expected = new AuthResponseDto(Guid.NewGuid(), "test@test.com", "Test", "User", "access123", "refresh123");

            _mediator
                .Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.Refresh(req);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);

            _mediator.Verify(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Logout()
        {
            var req = new RefreshRequest { RefreshToken = "" };

            var result = await _controller.Logout(req);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);

            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Logout_RefreshToken_Is_Valid()
        {
            var req = new RefreshRequest { RefreshToken = "refresh123" };

            _mediator
                .Setup(m => m.Send(It.IsAny<RevokeRefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.Logout(req);

            var noContent = result as NoContentResult;
            noContent.Should().NotBeNull();
            noContent!.StatusCode.Should().Be(204);

            _mediator.Verify(m => m.Send(It.IsAny<RevokeRefreshTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Google_IdToken()
        {
            var req = new GoogleSignInRequest { IdToken = "" };

            var result = await _controller.Google(req);

            var bad = result.Result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);

            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Google_IdToken_Is_Valid()
        {
            var req = new GoogleSignInRequest { IdToken = "id123" };
            var expected = new AuthResponseDto(Guid.NewGuid(), "test@test.com", "Test", "User", "access123", "refresh123");

            _mediator
                .Setup(m => m.Send(It.IsAny<GoogleSignInCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.Google(req);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);

            _mediator.Verify(m => m.Send(It.IsAny<GoogleSignInCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

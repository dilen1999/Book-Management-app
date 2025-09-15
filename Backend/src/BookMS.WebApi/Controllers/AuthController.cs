using BookMS.Application.Auth.Commands.GoogleSignIn;
using BookMS.Application.Auth.Commands.Login;
using BookMS.Application.Auth.Commands.Refresh;
using BookMS.Application.Auth.Commands.Register;
using BookMS.Application.Auth.Commands.Revoke;
using BookMS.Application.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookMS.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequest req)
        {
            var result = await _mediator.Send(new RegisterUserCommand(req));
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequest req)
            => Ok(await _mediator.Send(new LoginCommand(req)));

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { error = "refreshToken is required" });

            var resp = await _mediator.Send(new RefreshTokenCommand(req.RefreshToken));
            return Ok(resp);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { error = "refreshToken is required" });

            await _mediator.Send(new RevokeRefreshTokenCommand(req.RefreshToken));
            return NoContent();
        }

        [HttpPost("google")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Google([FromBody] GoogleSignInRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.IdToken))
                return BadRequest(new { error = "idToken is required" });

            var resp = await _mediator.Send(new GoogleSignInCommand(req));
            return Ok(resp);
        }
    }
}

using BookMS.Application.Book.Queries.GetBookById;
using BookMS.Application.Book.Queries.GetBooksPaged;
using BookMS.Application.Books.Commands.CreateBook;
using BookMS.Application.Books.Queries.GetBooksCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookMS.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BooksController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<object>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetBooksPagedQuery(page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetBookByIdQuery(id));
            if (result is null) return NotFound(); // or let handler throw NotFoundException
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Create([FromBody] CreateBookCommand cmd)
        {
            var dto = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _mediator.Send(new GetBooksCountQuery());
            return Ok(new { totalCount = count });
        }
    }
}
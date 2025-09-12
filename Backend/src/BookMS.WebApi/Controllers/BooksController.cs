using BookMS.Application.Book.Queries.GetBookById;
using BookMS.Application.Book.Queries.GetBooksPaged;
using BookMS.Application.Books.Commands.CreateBook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookMS.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BooksController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(await _mediator.Send(new GetBooksPagedQuery(page, pageSize)));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _mediator.Send(new GetBookByIdQuery(id)));

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookCommand cmd)
            => Ok(await _mediator.Send(cmd));
    }
}
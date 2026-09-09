using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReadTogether.API.InputModels.Bookshelf;
using ReadTogether.API.InputModels.Bookshelves;
using ReadTogether.Application.Features.Bookshelves.AddBookToBookshelf;
using ReadTogether.Application.Features.Bookshelves.CreateBookshelf;
using ReadTogether.Application.Features.Bookshelves.GetBookshelf;
using ReadTogether.Application.Features.Bookshelves.GetBookshelfBooks;
using ReadTogether.Application.Features.Bookshelves.GetBookshelvesByUser;
using ReadTogether.Application.Features.Bookshelves.RemoveBookFromBookshelf;
using ReadTogether.Domain.DTOs;
using Superpower.Model;

namespace ReadTogether.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookshelvesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookshelvesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookshelf(int id)
        {

            var query = new GetBookshelfQuery(id);
            var result = await _mediator.Send(query, HttpContext.RequestAborted);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetBookshelfBooks(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetBookshelfBooksQuery(id, page, pageSize);
            var result = await _mediator.Send(query, HttpContext.RequestAborted);
            return Ok(result.Data);
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetBookshelvesByUser(string username, [FromQuery] string? bookId = null)
        {
            var query = new GetBookshelvesByUserQuery(username, bookId);
            var result = await _mediator.Send(query, HttpContext.RequestAborted);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookshelf([FromBody] CreateBookshelfInputModel inputModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new CreateBookshelfCommand(inputModel.Name, userId);
            var result = await _mediator.Send(command);
            return Created($"/api/bookshelves/{result.Id}", result);
        }

        [HttpDelete("{bookshelfId}")]
        public async Task<IActionResult> DeleteBookshelf(int bookshelfId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new DeleteBookshelfCommand(bookshelfId, userId, cancellationToken);
            var result = await _mediator.Send(command);
            if (result)
            {
                return NoContent();
            }
            else
            {
                throw new Exception("Unexpected error occurred when deleting bookshelf");
            }
        }


        [HttpPost("{bookshelfId}/books/{bookId}")]
        public async Task<IActionResult> AddBookToBookshelf([FromRoute] int bookshelfId, [FromRoute] string bookId, [FromBody] AddBookToBookshelfInputModel inputModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var metadata = new BookMetadataDto
            {
                Id = bookId,
                Title = inputModel.Title,
                AuthorName = inputModel.AuthorName,
                FirstPublishedYear = inputModel.FirstPublishedYear,
                CoverImageUrl = inputModel.CoverImageUrl
            };
            var command = new AddBookToBookshelfCommand(bookshelfId, bookId, metadata, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{bookshelfId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBookFromBookshelf([FromRoute] int bookshelfId, [FromRoute] string bookId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var command = new RemoveBookFromBookshelfCommand(bookshelfId, bookId, userId, cancellationToken);
            var result = await _mediator.Send(command);
            if (result)
            {
                return NoContent();
            }
            else
            {
                throw new Exception("Unexpected error occured when removing book from bookshelf");
            }
        }
    }
}
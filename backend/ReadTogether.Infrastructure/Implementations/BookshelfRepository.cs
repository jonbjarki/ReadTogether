using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReadTogether.Domain.Common;
using ReadTogether.Domain.Contexts;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Exceptions;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Infrastructure.Implementations
{
    public class BookshelfRepository : IBookshelfRepository
    {
        private readonly BooksDbContext _context;

        public BookshelfRepository(BooksDbContext context)
        {
            _context = context;
        }

        public async Task<Bookshelf> CreateBookshelf(string name, string userId, CancellationToken cancellationToken, bool isDefaultShelf = false)
        {
            var newBookshelf = new Bookshelf
            {
                Name = name,
                UserId = userId,
                IsDefaultShelf = isDefaultShelf
            };

            _context.Bookshelves.Add(newBookshelf);
            await _context.SaveChangesAsync(cancellationToken);

            return newBookshelf;
        }

        public async Task<Bookshelf?> GetBookshelfById(int id, CancellationToken cancellationToken)
        {
            return await _context.Bookshelves
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<PagedResponse<BookshelfBookDto>> GetBookshelfBooks(int bookshelfId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var req = _context.BookshelfBooks
                .AsNoTracking()
                .Where(bb => bb.BookshelfId == bookshelfId);

            var count = await req.CountAsync(cancellationToken);

            var books = await req
                .OrderByDescending(bb => bb.Title)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var res = new PagedResponse<BookshelfBookDto>
            {
                Results = books.Select(bb => new BookshelfBookDto
                {
                    Id = bb.VolumeId,
                    Title = bb.Title,
                    CoverImageUrl = bb.ThumbnailUrl
                }).ToList(),
                Page = pageNumber,
                PageSize = pageSize,
                NumPages = (int)Math.Ceiling((double)count / pageSize)
            };

            return res;
        }
        public async Task<List<Bookshelf>> GetBookshelvesByUserId(string userId, CancellationToken cancellationToken)
        {
            return await _context.Bookshelves
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.BookshelfBooks)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<BookshelfBook> AddBookToBookshelf(int bookshelfId, string bookId, string title, string thumbnailUrl, CancellationToken cancellationToken)
        {
            var alreadyExists = await _context.BookshelfBooks
                .AsNoTracking()
                .AnyAsync(bb => bb.BookshelfId == bookshelfId && bb.VolumeId == bookId, cancellationToken);

            if (alreadyExists)
            {
                throw new BookshelfBookConflictException(bookshelfId, bookId);
            }

            var bookshelfBook = new BookshelfBook
            {
                BookshelfId = bookshelfId,
                VolumeId = bookId,
                Title = title,
                ThumbnailUrl = thumbnailUrl
            };

            _context.BookshelfBooks.Add(bookshelfBook);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return bookshelfBook;
            }
            catch (DbUpdateException ex)
            {
                throw new BookshelfBookConflictException(bookshelfId, bookId, ex);
            }
        }

        public async Task<bool> RemoveBookFromBookshelf(int bookshelfId, string bookId, string userId, CancellationToken cancellationToken)
        {
            var book = await _context.BookshelfBooks
            .Include(bb => bb.Bookshelf)
            .FirstOrDefaultAsync(bb => bb.BookshelfId == bookshelfId && bb.VolumeId == bookId, cancellationToken);

            if (book is not null && book.Bookshelf.UserId == userId)
            {
                _context.BookshelfBooks.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            else if (book is not null && book.Bookshelf.UserId != userId)
            {
                throw new AccessDeniedException("You are not allowed to access this resource");
            }
            else if (book is null)
            {
                throw new NotFoundException("Bookshelf Book", $"{bookshelfId}/{bookId}");
            }

            return false;
        }

        public async Task<bool> DeleteBookshelf(int bookshelfId, string userId, CancellationToken cancellationToken)
        {
            var bookshelf = await _context.Bookshelves.FindAsync(bookshelfId, cancellationToken);
            if (bookshelf is null)
            {
                throw new NotFoundException("Bookshelf", bookshelfId.ToString());
            }
            else if (bookshelf is not null && !bookshelf.IsDefaultShelf && bookshelf.UserId == userId)
            {

                _context.Bookshelves.Remove(bookshelf);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            else if (bookshelf is not null && bookshelf.UserId != userId)
            {
                throw new AccessDeniedException("You are not allowed to access this resource");
            }
            else if (bookshelf is not null && bookshelf.IsDefaultShelf)
            {
                throw new AccessDeniedException("Can not delete default bookshelf!");
            }

            return false;
        }

    }
}
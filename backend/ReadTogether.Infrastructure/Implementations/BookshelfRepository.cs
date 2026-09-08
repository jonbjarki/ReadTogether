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
                .Where(bb => bb.BookshelfId == bookshelfId)
                .Include(bb => bb.Book);

            var count = await req.CountAsync(cancellationToken);

            var books = await req
                .OrderByDescending(bb => bb.AddedAt)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var res = new PagedResponse<BookshelfBookDto>
            {
                Results = books.Select(bb => new BookshelfBookDto
                {
                    Id = bb.BookId,
                    Title = bb.Book.Title,
                    CoverImageUrl = bb.Book.CoverImageUrl,
                    AuthorName = bb.Book.AuthorName,
                    FirstPublishedYear = bb.Book.FirstPublishedYear,
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

        public async Task<BookshelfBook> AddBookToBookshelf(int bookshelfId, BookMetadataDto metadata, CancellationToken cancellationToken)
        {
            var bookId = metadata.Id;

            var alreadyExists = await _context.BookshelfBooks
                .AsNoTracking()
                .AnyAsync(bb => bb.BookshelfId == bookshelfId && bb.BookId == bookId, cancellationToken);

            if (alreadyExists)
            {
                throw new BookshelfBookConflictException(bookshelfId, bookId);
            }

            // The local book copy is shared across shelves: create it only the first time any
            // shelf references this volume, and reuse the existing record on subsequent adds.
            var book = await _context.Books.FindAsync([bookId], cancellationToken);
            if (book is null)
            {
                book = new Book
                {
                    Id = bookId,
                    Title = metadata.Title,
                    AuthorName = metadata.AuthorName,
                    FirstPublishedYear = metadata.FirstPublishedYear,
                    CoverImageUrl = metadata.CoverImageUrl
                };
                _context.Books.Add(book);
            }

            var bookshelfBook = new BookshelfBook
            {
                BookshelfId = bookshelfId,
                BookId = bookId
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
            .FirstOrDefaultAsync(bb => bb.BookshelfId == bookshelfId && bb.BookId == bookId, cancellationToken);

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
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
using static ReadTogether.Domain.Common.SortingTypes;

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
                .Include(b => b.BookshelfBooks)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<PagedResponse<BookshelfBookDto>> GetBookshelfBooks(int bookshelfId, int pageNumber, int pageSize, CancellationToken cancellationToken, OrderBy orderBy = OrderBy.DateAdded, OrderDir orderDir = OrderDir.Desc)
        {
            var req = _context.BookshelfBooks
                .AsNoTracking()
                .Where(bb => bb.BookshelfId == bookshelfId)
                .Include(bb => bb.Book);

            var count = await req.CountAsync(cancellationToken);

            IOrderedQueryable<BookshelfBook> books;
            // Order the results as requested
            switch (orderBy)
            {
                case OrderBy.Title:
                    if (orderDir == OrderDir.Desc)
                        books = req.OrderByDescending(bb => bb.Book.Title);
                    else
                        books = req.OrderBy(bb => bb.Book.Title);
                    break;
                case OrderBy.Year:
                    if (orderDir == OrderDir.Desc)
                        books = req.OrderByDescending(bb => bb.Book.FirstPublishedYear);
                    else
                        books = req.OrderBy(bb => bb.Book.FirstPublishedYear);
                    break;
                default:
                    if (orderDir == OrderDir.Desc)
                        books = req.OrderByDescending(bb => bb.AddedAt);
                    else
                        books = req.OrderBy(bb => bb.AddedAt);
                    break;

            }

            var results = await books.Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

            var res = new PagedResponse<BookshelfBookDto>
            {
                Results = results.Select(bb => new BookshelfBookDto
                {
                    Id = bb.BookId,
                    Title = bb.Book.Title,
                    CoverImageUrl = bb.Book.CoverImageUrl,
                    AuthorName = bb.Book.AuthorName,
                    FirstPublishedYear = bb.Book.FirstPublishedYear,
                }).ToList(),
                Page = pageNumber,
                PageSize = pageSize,
                Total = count
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

        public async Task<BookshelfBook> AddBookToBookshelf(int bookshelfId, string bookId, CancellationToken cancellationToken)
        {
            var existing = await _context.BookshelfBooks.FindAsync(bookshelfId, bookId);
            if (existing is not null)
            {
                throw new BookshelfBookConflictException(bookshelfId, bookId);
            }
            var book = await _context.Books.FindAsync(bookId) ?? throw new NotFoundException("Book", bookId);
            var bookshelf = await _context.Bookshelves.FindAsync(bookshelfId) ?? throw new NotFoundException("Bookshelf", bookshelfId.ToString());
            var bookshelfBook = new BookshelfBook
            {
                BookshelfId = bookshelfId,
                BookId = bookId
            };

            _context.BookshelfBooks.Add(bookshelfBook);
            await _context.SaveChangesAsync(cancellationToken);

            return bookshelfBook;

        }

        public async Task<bool> RemoveBooksFromBookshelf(int bookshelfId, string[] bookIds, string userId, CancellationToken cancellationToken)
        {
            var shelf = await _context.Bookshelves.FirstOrDefaultAsync(b => b.Id == bookshelfId);
            if (shelf is null) throw new NotFoundException("Bookshelf", bookshelfId.ToString());
            if (shelf.UserId != userId) throw new AccessDeniedException("You are not allowed to modify this bookshelf!");

            int deletedRows = await _context.BookshelfBooks
            .Include(bb => bb.Bookshelf)
            .Where(bb => bb.Bookshelf.Id == bookshelfId)
            .Where(bb => bookIds.Contains(bb.BookId))
            .ExecuteDeleteAsync(cancellationToken);

            Console.WriteLine("Deleted {0} rows", deletedRows);
            return true;
        }

        public async Task<bool> DeleteBookshelf(int bookshelfId, string userId, CancellationToken cancellationToken)
        {
            var bookshelf = await _context.Bookshelves.FindAsync([bookshelfId, cancellationToken], cancellationToken: cancellationToken);
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
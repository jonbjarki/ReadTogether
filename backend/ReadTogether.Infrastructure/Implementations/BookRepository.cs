using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadTogether.Domain.Contexts;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using ReadTogether.Infrastructure.Interfaces;

namespace ReadTogether.Infrastructure.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly BooksDbContext _context;

        public BookRepository(BooksDbContext context)
        {
            _context = context;
        }

        public async Task<Book> CreateBook(BookDetailsDto bookMetadata, CancellationToken cancellationToken)
        {
            var existing = await _context.Books.FindAsync(bookMetadata.Id);
            if (existing is not null) return existing;

            var book = new Book
            {
                Id = bookMetadata.Id,
                Title = bookMetadata.Title,
                AuthorName = bookMetadata.AuthorName,
                CoverImageUrl = bookMetadata.CoverImageUrl,
                FirstPublishedYear = bookMetadata.FirstPublishedYear
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> GetBook(string id, CancellationToken cancellationToken)
        {
            return await _context.Books.FindAsync(id);
        }
    }
}
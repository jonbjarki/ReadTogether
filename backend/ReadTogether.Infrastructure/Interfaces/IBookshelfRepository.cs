using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadTogether.Domain.Common;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;
using static ReadTogether.Domain.Common.SortingTypes;

namespace ReadTogether.Infrastructure.Interfaces
{
    public interface IBookshelfRepository
    {
        Task<Bookshelf> CreateBookshelf(string name, string userId, CancellationToken cancellationToken, bool isDefaultShelf = false);
        Task<Bookshelf?> GetBookshelfById(int id, CancellationToken cancellationToken);
        Task<PagedResponse<BookshelfBookDto>> GetBookshelfBooks(int bookshelfId, int pageNumber, int pageSize, OrderBy orderBy, OrderDir orderDir, CancellationToken cancellationToken);
        Task<List<Bookshelf>> GetBookshelvesByUserId(string userId, CancellationToken cancellationToken);
        /// <summary>
        /// Adds a book to a bookshelf, creating the shared local <see cref="Book"/> copy from
        /// <paramref name="metadata"/> when it does not exist yet and linking it via a join entry.
        /// </summary>
        /// <param name="bookshelfId">The ID of the bookshelf to add the book to.</param>
        /// <param name="metadata">The book metadata; <see cref="BookMetadataDto.Id"/> identifies the book.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created <see cref="BookshelfBook"/> join entry.</returns>
        /// <exception cref="ReadTogether.Infrastructure.Exceptions.BookshelfBookConflictException">The book is already on the bookshelf.</exception>
        Task<BookshelfBook> AddBookToBookshelf(int bookshelfId, BookMetadataDto metadata, CancellationToken cancellationToken);
        Task<bool> RemoveBookFromBookshelf(int bookshelfId, string bookId, string userId, CancellationToken cancellationToken);
        Task<bool> DeleteBookshelf(int bookshelfId, string userId, CancellationToken cancellationToken);
    }
}
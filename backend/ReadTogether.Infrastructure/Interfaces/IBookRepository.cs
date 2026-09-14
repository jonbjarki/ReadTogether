using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadTogether.Domain.DTOs;
using ReadTogether.Domain.Entities;

namespace ReadTogether.Infrastructure.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetBook(string id, CancellationToken cancellationToken);
        Task<Book> CreateBook(BookDetailsDto bookMetadata, CancellationToken cancellationToken);
    }
}
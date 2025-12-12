using Library.Core.Entities;

namespace Library.Core.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksWithLibraryAsync();
    Task<Book?> GetBookWithDetailsAsync(int id);
    Task<IEnumerable<Book>> GetBooksByLibraryIdAsync(int libraryId);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
}

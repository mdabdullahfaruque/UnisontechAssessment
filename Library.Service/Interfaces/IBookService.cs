using Library.Core.DTOs;
using Library.Core.Models;

namespace Library.Service.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<PagedResult<BookDto>> GetAllBooksPagedAsync(int page, int pageSize);
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    Task<IEnumerable<BookDto>> GetBooksByLibraryIdAsync(int libraryId);
    Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
    Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(int id);
    Task<bool> BookExistsAsync(int id);
}

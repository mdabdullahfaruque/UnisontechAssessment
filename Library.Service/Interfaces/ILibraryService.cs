using Library.Core.DTOs;

namespace Library.Service.Interfaces;

public interface ILibraryService
{
    Task<IEnumerable<LibraryDto>> GetAllLibrariesAsync();
    Task<LibraryDto?> GetLibraryByIdAsync(int id);
    Task<LibraryDto?> GetLibraryWithBooksAsync(int id);
    Task<LibraryDto> CreateLibraryAsync(CreateLibraryDto createLibraryDto);
    Task<LibraryDto?> UpdateLibraryAsync(int id, UpdateLibraryDto updateLibraryDto);
    Task<bool> DeleteLibraryAsync(int id);
    Task<bool> LibraryExistsAsync(int id);
}

using Library.Core.DTOs;
using Library.Core.Interfaces;
using Library.Service.Interfaces;

namespace Library.Service.Services;

public class LibraryService : ILibraryService
{
    private readonly IUnitOfWork _unitOfWork;

    public LibraryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<LibraryDto>> GetAllLibrariesAsync()
    {
        var libraries = await _unitOfWork.Libraries.GetLibrariesWithBooksAsync();
        return libraries.Select(MapToDto);
    }

    public async Task<LibraryDto?> GetLibraryByIdAsync(int id)
    {
        var library = await _unitOfWork.Libraries.GetByIdAsync(id);
        return library != null ? MapToDto(library) : null;
    }

    public async Task<LibraryDto?> GetLibraryWithBooksAsync(int id)
    {
        var library = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(id);
        return library != null ? MapToDto(library) : null;
    }

    public async Task<LibraryDto> CreateLibraryAsync(CreateLibraryDto createLibraryDto)
    {
        // Business logic: Validate that closing time is after opening time
        if (createLibraryDto.ClosingTime <= createLibraryDto.OpeningTime)
        {
            throw new InvalidOperationException("Closing time must be after opening time.");
        }

        var library = new Core.Entities.Library
        {
            Name = createLibraryDto.Name,
            Address = createLibraryDto.Address,
            PhoneNumber = createLibraryDto.PhoneNumber,
            Email = createLibraryDto.Email,
            OpeningTime = createLibraryDto.OpeningTime,
            ClosingTime = createLibraryDto.ClosingTime
        };

        await _unitOfWork.Libraries.AddAsync(library);
        await _unitOfWork.CompleteAsync();

        var createdLibrary = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(library.LibraryId);
        return MapToDto(createdLibrary!);
    }

    public async Task<LibraryDto?> UpdateLibraryAsync(int id, UpdateLibraryDto updateLibraryDto)
    {
        var library = await _unitOfWork.Libraries.GetByIdAsync(id);
        if (library == null)
        {
            return null;
        }

        // Business logic: Validate that closing time is after opening time
        if (updateLibraryDto.ClosingTime <= updateLibraryDto.OpeningTime)
        {
            throw new InvalidOperationException("Closing time must be after opening time.");
        }

        library.Name = updateLibraryDto.Name;
        library.Address = updateLibraryDto.Address;
        library.PhoneNumber = updateLibraryDto.PhoneNumber;
        library.Email = updateLibraryDto.Email;
        library.OpeningTime = updateLibraryDto.OpeningTime;
        library.ClosingTime = updateLibraryDto.ClosingTime;

        _unitOfWork.Libraries.Update(library);
        await _unitOfWork.CompleteAsync();

        var updatedLibrary = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(library.LibraryId);
        return MapToDto(updatedLibrary!);
    }

    public async Task<bool> DeleteLibraryAsync(int id)
    {
        var library = await _unitOfWork.Libraries.GetByIdAsync(id);
        if (library == null)
        {
            return false;
        }

        // Business logic: Check if library has books
        var hasBooks = await _unitOfWork.Libraries.ExistsAsync(l => 
            l.LibraryId == id && l.Books.Any());
        
        if (hasBooks)
        {
            throw new InvalidOperationException("Cannot delete a library that has books. Please remove all books first.");
        }

        _unitOfWork.Libraries.Remove(library);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> LibraryExistsAsync(int id)
    {
        return await _unitOfWork.Libraries.ExistsAsync(l => l.LibraryId == id);
    }

    private static LibraryDto MapToDto(Core.Entities.Library library)
    {
        return new LibraryDto
        {
            LibraryId = library.LibraryId,
            Name = library.Name,
            Address = library.Address,
            PhoneNumber = library.PhoneNumber,
            Email = library.Email,
            OpeningTime = library.OpeningTime,
            ClosingTime = library.ClosingTime,
            TotalBooks = library.Books?.Count ?? 0
        };
    }
}

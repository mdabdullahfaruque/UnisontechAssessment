using Library.Core.DTOs;
using Library.Core.Interfaces;
using Library.Service.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Library.Service.Services;

public class LibraryService : ILibraryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LibraryService> _logger;
    private readonly IMapper _mapper;

    public LibraryService(IUnitOfWork unitOfWork, ILogger<LibraryService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LibraryDto>> GetAllLibrariesAsync()
    {
        var libraries = await _unitOfWork.Libraries.GetLibrariesWithBooksAsync();
        return _mapper.Map<IEnumerable<LibraryDto>>(libraries);
    }

    public async Task<LibraryDto?> GetLibraryByIdAsync(int id)
    {
        var library = await _unitOfWork.Libraries.GetByIdAsync(id);
        return library != null ? _mapper.Map<LibraryDto>(library) : null;
    }

    public async Task<LibraryDto?> GetLibraryWithBooksAsync(int id)
    {
        var library = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(id);
        return library != null ? _mapper.Map<LibraryDto>(library) : null;
    }

    public async Task<LibraryDto> CreateLibraryAsync(CreateLibraryDto createLibraryDto)
    {
        // Business logic: Validate that closing time is after opening time
        if (createLibraryDto.ClosingTime <= createLibraryDto.OpeningTime)
        {
            throw new InvalidOperationException("Closing time must be after opening time.");
        }

        var library = _mapper.Map<Core.Entities.Library>(createLibraryDto);

        await _unitOfWork.Libraries.AddAsync(library);
        await _unitOfWork.CompleteAsync();

        var createdLibrary = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(library.LibraryId);
        return _mapper.Map<LibraryDto>(createdLibrary!);
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

        _mapper.Map(updateLibraryDto, library);

        _unitOfWork.Libraries.Update(library);
        await _unitOfWork.CompleteAsync();

        var updatedLibrary = await _unitOfWork.Libraries.GetLibraryWithBooksAsync(library.LibraryId);
        return _mapper.Map<LibraryDto>(updatedLibrary!);
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
}

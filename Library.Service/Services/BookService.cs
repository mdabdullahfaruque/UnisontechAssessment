using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Core.Models;
using Library.Service.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Library.Service.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookService> _logger;
    private readonly IMapper _mapper;

    public BookService(IUnitOfWork unitOfWork, ILogger<BookService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        _logger.LogInformation("Retrieving all books");
        var books = await _unitOfWork.Books.GetBooksWithLibraryAsync();
        _logger.LogInformation("Retrieved {Count} books", books.Count());
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<PagedResult<BookDto>> GetAllBooksPagedAsync(int page, int pageSize)
    {
        _logger.LogInformation("Retrieving books - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var allBooks = await _unitOfWork.Books.GetBooksWithLibraryAsync();
        var totalCount = allBooks.Count();
        
        var pagedBooks = allBooks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var bookDtos = _mapper.Map<IEnumerable<BookDto>>(pagedBooks);
        
        _logger.LogInformation("Retrieved page {Page} with {Count} books out of {Total} total", 
            page, pagedBooks.Count, totalCount);
        
        return new PagedResult<BookDto>(bookDtos, page, pageSize, totalCount);
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _unitOfWork.Books.GetBookWithDetailsAsync(id);
        return book != null ? _mapper.Map<BookDto>(book) : null;
    }

    public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _unitOfWork.Books.GetAvailableBooksAsync();
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByLibraryIdAsync(int libraryId)
    {
        var books = await _unitOfWork.Books.GetBooksByLibraryIdAsync(libraryId);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        _logger.LogInformation("Creating new book: {Title}", createBookDto.Title);
        
        // Business logic: Validate that AvailableCopies doesn't exceed TotalCopies
        if (createBookDto.AvailableCopies > createBookDto.TotalCopies)
        {
            _logger.LogWarning("Validation failed: Available copies {Available} exceed total copies {Total}", 
                createBookDto.AvailableCopies, createBookDto.TotalCopies);
            throw new InvalidOperationException("Available copies cannot exceed total copies.");
        }

        var book = _mapper.Map<Book>(createBookDto);

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.CompleteAsync();

        var createdBook = await _unitOfWork.Books.GetBookWithDetailsAsync(book.BookId);
        _logger.LogInformation("Book created successfully with ID: {BookId}", book.BookId);
        return _mapper.Map<BookDto>(createdBook!);
    }

    public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
        {
            return null;
        }

        // Business logic: Validate that AvailableCopies doesn't exceed TotalCopies
        if (updateBookDto.AvailableCopies > updateBookDto.TotalCopies)
        {
            throw new InvalidOperationException("Available copies cannot exceed total copies.");
        }

        _mapper.Map(updateBookDto, book);

        _unitOfWork.Books.Update(book);
        await _unitOfWork.CompleteAsync();

        var updatedBook = await _unitOfWork.Books.GetBookWithDetailsAsync(book.BookId);
        return _mapper.Map<BookDto>(updatedBook!);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
        {
            return false;
        }

        // Business logic: Check if book has active borrows
        var hasActiveBorrows = await _unitOfWork.Books.ExistsAsync(b => 
            b.BookId == id && b.Borrows.Any(br => br.ReturnDate == null));
        
        if (hasActiveBorrows)
        {
            throw new InvalidOperationException("Cannot delete a book with active borrows.");
        }

        _unitOfWork.Books.Remove(book);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> BookExistsAsync(int id)
    {
        return await _unitOfWork.Books.ExistsAsync(b => b.BookId == id);
    }
}

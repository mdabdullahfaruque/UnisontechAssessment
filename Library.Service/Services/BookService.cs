using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Service.Interfaces;

namespace Library.Service.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _unitOfWork.Books.GetBooksWithLibraryAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _unitOfWork.Books.GetBookWithDetailsAsync(id);
        return book != null ? MapToDto(book) : null;
    }

    public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _unitOfWork.Books.GetAvailableBooksAsync();
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByLibraryIdAsync(int libraryId)
    {
        var books = await _unitOfWork.Books.GetBooksByLibraryIdAsync(libraryId);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        // Business logic: Validate that AvailableCopies doesn't exceed TotalCopies
        if (createBookDto.AvailableCopies > createBookDto.TotalCopies)
        {
            throw new InvalidOperationException("Available copies cannot exceed total copies.");
        }

        var book = new Book
        {
            Title = createBookDto.Title,
            ISBN = createBookDto.ISBN,
            Author = createBookDto.Author,
            Publisher = createBookDto.Publisher,
            PublicationYear = createBookDto.PublicationYear,
            Genre = createBookDto.Genre,
            TotalCopies = createBookDto.TotalCopies,
            AvailableCopies = createBookDto.AvailableCopies,
            LibraryId = createBookDto.LibraryId
        };

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.CompleteAsync();

        var createdBook = await _unitOfWork.Books.GetBookWithDetailsAsync(book.BookId);
        return MapToDto(createdBook!);
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

        book.Title = updateBookDto.Title;
        book.ISBN = updateBookDto.ISBN;
        book.Author = updateBookDto.Author;
        book.Publisher = updateBookDto.Publisher;
        book.PublicationYear = updateBookDto.PublicationYear;
        book.Genre = updateBookDto.Genre;
        book.TotalCopies = updateBookDto.TotalCopies;
        book.AvailableCopies = updateBookDto.AvailableCopies;
        book.LibraryId = updateBookDto.LibraryId;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.CompleteAsync();

        var updatedBook = await _unitOfWork.Books.GetBookWithDetailsAsync(book.BookId);
        return MapToDto(updatedBook!);
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

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            BookId = book.BookId,
            Title = book.Title,
            ISBN = book.ISBN,
            Author = book.Author,
            Publisher = book.Publisher,
            PublicationYear = book.PublicationYear,
            Genre = book.Genre,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            LibraryId = book.LibraryId,
            LibraryName = book.Library?.Name
        };
    }
}

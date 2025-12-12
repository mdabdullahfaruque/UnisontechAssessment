using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Service.Interfaces;

namespace Library.Service.Services;

public class BorrowService : IBorrowService
{
    private readonly IUnitOfWork _unitOfWork;
    private const decimal FinePerDay = 1.00m; // $1 per day overdue

    public BorrowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BorrowDto>> GetAllBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetBorrowsWithDetailsAsync();
        return borrows.Select(MapToDto);
    }

    public async Task<BorrowDto?> GetBorrowByIdAsync(int id)
    {
        var borrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(id);
        return borrow != null ? MapToDto(borrow) : null;
    }

    public async Task<IEnumerable<BorrowDto>> GetOverdueBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetOverdueBorrowsAsync();
        return borrows.Select(MapToDto);
    }

    public async Task<IEnumerable<BorrowDto>> GetActiveBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetActiveBorrowsAsync();
        return borrows.Select(MapToDto);
    }

    public async Task<IEnumerable<BorrowDto>> GetBorrowsByMemberIdAsync(int memberId)
    {
        var borrows = await _unitOfWork.Borrows.GetBorrowsByMemberIdAsync(memberId);
        return borrows.Select(MapToDto);
    }

    public async Task<BorrowDto> BorrowBookAsync(CreateBorrowDto createBorrowDto)
    {
        // Business logic: Validate book exists
        var book = await _unitOfWork.Books.GetByIdAsync(createBorrowDto.BookId);
        if (book == null)
        {
            throw new InvalidOperationException($"Book with ID {createBorrowDto.BookId} not found.");
        }

        // Business logic: Validate member exists and is active
        var member = await _unitOfWork.Members.GetByIdAsync(createBorrowDto.MemberId);
        if (member == null)
        {
            throw new InvalidOperationException($"Member with ID {createBorrowDto.MemberId} not found.");
        }

        if (!member.IsActive)
        {
            throw new InvalidOperationException("Cannot borrow books. Member account is inactive.");
        }

        // Business logic: Check if book is available
        if (book.AvailableCopies <= 0)
        {
            throw new InvalidOperationException($"Book '{book.Title}' is currently not available.");
        }

        // Business logic: Check if member has overdue books
        var hasOverdue = await _unitOfWork.Borrows.ExistsAsync(b =>
            b.MemberId == createBorrowDto.MemberId &&
            b.ReturnDate == null &&
            b.DueDate < DateTime.UtcNow.Date);

        if (hasOverdue)
        {
            throw new InvalidOperationException("Cannot borrow books. Please return overdue books first.");
        }

        // Create borrow record
        var borrow = new Borrow
        {
            BookId = createBorrowDto.BookId,
            MemberId = createBorrowDto.MemberId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(createBorrowDto.BorrowDurationDays),
            Status = "Active",
            Fine = 0
        };

        await _unitOfWork.Borrows.AddAsync(borrow);

        // Update book available copies
        book.AvailableCopies--;
        _unitOfWork.Books.Update(book);

        await _unitOfWork.CompleteAsync();

        var createdBorrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(borrow.BorrowId);
        return MapToDto(createdBorrow!);
    }

    public async Task<BorrowDto?> ReturnBookAsync(int borrowId, ReturnBookDto returnBookDto)
    {
        var borrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(borrowId);
        if (borrow == null)
        {
            return null;
        }

        // Business logic: Check if book is already returned
        if (borrow.ReturnDate != null)
        {
            throw new InvalidOperationException("This book has already been returned.");
        }

        // Business logic: Validate return date is not in the future
        if (returnBookDto.ReturnDate > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Return date cannot be in the future.");
        }

        // Business logic: Validate return date is not before borrow date
        if (returnBookDto.ReturnDate < borrow.BorrowDate)
        {
            throw new InvalidOperationException("Return date cannot be before borrow date.");
        }

        // Calculate fine if overdue
        borrow.ReturnDate = returnBookDto.ReturnDate;
        if (returnBookDto.ReturnDate.Date > borrow.DueDate.Date)
        {
            var daysOverdue = (returnBookDto.ReturnDate.Date - borrow.DueDate.Date).Days;
            borrow.Fine = daysOverdue * FinePerDay;
            borrow.Status = "Returned Late";
        }
        else
        {
            borrow.Status = "Returned";
        }

        _unitOfWork.Borrows.Update(borrow);

        // Update book available copies
        var book = await _unitOfWork.Books.GetByIdAsync(borrow.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
            _unitOfWork.Books.Update(book);
        }

        await _unitOfWork.CompleteAsync();

        var returnedBorrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(borrow.BorrowId);
        return MapToDto(returnedBorrow!);
    }

    public async Task<bool> BorrowExistsAsync(int id)
    {
        return await _unitOfWork.Borrows.ExistsAsync(b => b.BorrowId == id);
    }

    private static BorrowDto MapToDto(Borrow borrow)
    {
        return new BorrowDto
        {
            BorrowId = borrow.BorrowId,
            BookId = borrow.BookId,
            BookTitle = borrow.Book?.Title ?? string.Empty,
            BookISBN = borrow.Book?.ISBN ?? string.Empty,
            MemberId = borrow.MemberId,
            MemberName = borrow.Member != null ? $"{borrow.Member.FirstName} {borrow.Member.LastName}" : string.Empty,
            MemberEmail = borrow.Member?.Email ?? string.Empty,
            BorrowDate = borrow.BorrowDate,
            DueDate = borrow.DueDate,
            ReturnDate = borrow.ReturnDate,
            Status = borrow.Status,
            Fine = borrow.Fine
        };
    }
}

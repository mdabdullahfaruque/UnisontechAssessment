using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Service.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Library.Service.Services;

public class BorrowService : IBorrowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BorrowService> _logger;
    private readonly IMapper _mapper;
    private const decimal FinePerDay = 1.00m; // $1 per day overdue

    public BorrowService(IUnitOfWork unitOfWork, ILogger<BorrowService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BorrowDto>> GetAllBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetBorrowsWithDetailsAsync();
        return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
    }

    public async Task<BorrowDto?> GetBorrowByIdAsync(int id)
    {
        var borrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(id);
        return borrow != null ? _mapper.Map<BorrowDto>(borrow) : null;
    }

    public async Task<IEnumerable<BorrowDto>> GetOverdueBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetOverdueBorrowsAsync();
        return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
    }

    public async Task<IEnumerable<BorrowDto>> GetActiveBorrowsAsync()
    {
        var borrows = await _unitOfWork.Borrows.GetActiveBorrowsAsync();
        return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
    }

    public async Task<IEnumerable<BorrowDto>> GetBorrowsByMemberIdAsync(int memberId)
    {
        var borrows = await _unitOfWork.Borrows.GetBorrowsByMemberIdAsync(memberId);
        return _mapper.Map<IEnumerable<BorrowDto>>(borrows);
    }

    public async Task<BorrowDto> BorrowBookAsync(CreateBorrowDto createBorrowDto)
    {
        _logger.LogInformation("Processing book borrow request - BookId: {BookId}, MemberId: {MemberId}", 
            createBorrowDto.BookId, createBorrowDto.MemberId);
        
        // Business logic: Validate book exists
        var book = await _unitOfWork.Books.GetByIdAsync(createBorrowDto.BookId);
        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found", createBorrowDto.BookId);
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
        var borrow = _mapper.Map<Borrow>(createBorrowDto);

        await _unitOfWork.Borrows.AddAsync(borrow);

        // Update book available copies
        book.AvailableCopies--;
        _unitOfWork.Books.Update(book);

        await _unitOfWork.CompleteAsync();

        var createdBorrow = await _unitOfWork.Borrows.GetBorrowWithDetailsAsync(borrow.BorrowId);
        return _mapper.Map<BorrowDto>(createdBorrow!);
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
        return _mapper.Map<BorrowDto>(returnedBorrow!);
    }

    public async Task<bool> BorrowExistsAsync(int id)
    {
        return await _unitOfWork.Borrows.ExistsAsync(b => b.BorrowId == id);
    }
}

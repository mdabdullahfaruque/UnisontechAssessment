using Microsoft.EntityFrameworkCore;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Repository.Data;

namespace Library.Repository.Repositories;

public class BorrowRepository : Repository<Borrow>, IBorrowRepository
{
    public BorrowRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Borrow?> GetBorrowWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Book)
                .ThenInclude(book => book.Library)
            .Include(b => b.Member)
            .FirstOrDefaultAsync(b => b.BorrowId == id);
    }

    public async Task<IEnumerable<Borrow>> GetBorrowsWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Book)
                .ThenInclude(book => book.Library)
            .Include(b => b.Member)
            .ToListAsync();
    }

    public async Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Include(b => b.Book)
            .Include(b => b.Member)
            .Where(b => b.ReturnDate == null && b.DueDate < today)
            .ToListAsync();
    }

    public async Task<IEnumerable<Borrow>> GetActiveBorrowsAsync()
    {
        return await _dbSet
            .Include(b => b.Book)
            .Include(b => b.Member)
            .Where(b => b.ReturnDate == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Borrow>> GetBorrowsByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .Include(b => b.Book)
            .Where(b => b.MemberId == memberId)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Borrow>> GetBorrowsByBookIdAsync(int bookId)
    {
        return await _dbSet
            .Include(b => b.Member)
            .Where(b => b.BookId == bookId)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();
    }
}

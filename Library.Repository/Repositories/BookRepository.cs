using Microsoft.EntityFrameworkCore;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Repository.Data;

namespace Library.Repository.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetBooksWithLibraryAsync()
    {
        return await _dbSet
            .Include(b => b.Library)
            .ToListAsync();
    }

    public async Task<Book?> GetBookWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Library)
            .Include(b => b.Borrows)
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task<IEnumerable<Book>> GetBooksByLibraryIdAsync(int libraryId)
    {
        return await _dbSet
            .Where(b => b.LibraryId == libraryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Where(b => b.AvailableCopies > 0)
            .Include(b => b.Library)
            .ToListAsync();
    }
}

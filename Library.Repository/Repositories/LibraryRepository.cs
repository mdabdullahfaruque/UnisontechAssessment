using Microsoft.EntityFrameworkCore;
using Library.Core.Interfaces;
using Library.Repository.Data;

namespace Library.Repository.Repositories;

public class LibraryRepository : Repository<Core.Entities.Library>, ILibraryRepository
{
    public LibraryRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Core.Entities.Library?> GetLibraryWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(l => l.Books)
            .FirstOrDefaultAsync(l => l.LibraryId == id);
    }

    public async Task<IEnumerable<Core.Entities.Library>> GetLibrariesWithBooksAsync()
    {
        return await _dbSet
            .Include(l => l.Books)
            .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Repository.Data;

namespace Library.Repository.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetMemberWithBorrowsAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Borrows)
                .ThenInclude(b => b.Book)
            .FirstOrDefaultAsync(m => m.MemberId == id);
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetMembersByTypeAsync(string membershipType)
    {
        return await _dbSet
            .Where(m => m.MembershipType == membershipType)
            .ToListAsync();
    }

    public async Task<Member?> GetMemberByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Email == email);
    }
}

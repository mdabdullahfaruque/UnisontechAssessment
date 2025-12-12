using Library.Core.Entities;

namespace Library.Core.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    Task<Member?> GetMemberWithBorrowsAsync(int id);
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<IEnumerable<Member>> GetMembersByTypeAsync(string membershipType);
    Task<Member?> GetMemberByEmailAsync(string email);
}

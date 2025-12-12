using Library.Core.Entities;

namespace Library.Core.Interfaces;

public interface IBorrowRepository : IRepository<Borrow>
{
    Task<Borrow?> GetBorrowWithDetailsAsync(int id);
    Task<IEnumerable<Borrow>> GetBorrowsWithDetailsAsync();
    Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync();
    Task<IEnumerable<Borrow>> GetActiveBorrowsAsync();
    Task<IEnumerable<Borrow>> GetBorrowsByMemberIdAsync(int memberId);
    Task<IEnumerable<Borrow>> GetBorrowsByBookIdAsync(int bookId);
}

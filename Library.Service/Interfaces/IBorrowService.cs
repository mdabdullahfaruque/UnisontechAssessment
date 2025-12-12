using Library.Core.DTOs;

namespace Library.Service.Interfaces;

public interface IBorrowService
{
    Task<IEnumerable<BorrowDto>> GetAllBorrowsAsync();
    Task<BorrowDto?> GetBorrowByIdAsync(int id);
    Task<IEnumerable<BorrowDto>> GetOverdueBorrowsAsync();
    Task<IEnumerable<BorrowDto>> GetActiveBorrowsAsync();
    Task<IEnumerable<BorrowDto>> GetBorrowsByMemberIdAsync(int memberId);
    Task<BorrowDto> BorrowBookAsync(CreateBorrowDto createBorrowDto);
    Task<BorrowDto?> ReturnBookAsync(int borrowId, ReturnBookDto returnBookDto);
    Task<bool> BorrowExistsAsync(int id);
}

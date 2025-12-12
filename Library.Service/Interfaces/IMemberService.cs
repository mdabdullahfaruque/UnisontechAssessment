using Library.Core.DTOs;

namespace Library.Service.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetAllMembersAsync();
    Task<MemberDto?> GetMemberByIdAsync(int id);
    Task<MemberDto?> GetMemberWithBorrowsAsync(int id);
    Task<IEnumerable<MemberDto>> GetActiveMembersAsync();
    Task<IEnumerable<MemberDto>> GetMembersByTypeAsync(string membershipType);
    Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto);
    Task<MemberDto?> UpdateMemberAsync(int id, UpdateMemberDto updateMemberDto);
    Task<bool> DeleteMemberAsync(int id);
    Task<bool> MemberExistsAsync(int id);
}

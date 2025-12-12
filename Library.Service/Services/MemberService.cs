using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Service.Interfaces;

namespace Library.Service.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        return members.Select(MapToDto);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        return member != null ? MapToDto(member) : null;
    }

    public async Task<MemberDto?> GetMemberWithBorrowsAsync(int id)
    {
        var member = await _unitOfWork.Members.GetMemberWithBorrowsAsync(id);
        return member != null ? MapToDto(member) : null;
    }

    public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return members.Select(MapToDto);
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByTypeAsync(string membershipType)
    {
        var members = await _unitOfWork.Members.GetMembersByTypeAsync(membershipType);
        return members.Select(MapToDto);
    }

    public async Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto)
    {
        // Business logic: Check if email already exists
        var existingMember = await _unitOfWork.Members.GetMemberByEmailAsync(createMemberDto.Email);
        if (existingMember != null)
        {
            throw new InvalidOperationException($"A member with email '{createMemberDto.Email}' already exists.");
        }

        var member = new Member
        {
            FirstName = createMemberDto.FirstName,
            LastName = createMemberDto.LastName,
            Email = createMemberDto.Email,
            PhoneNumber = createMemberDto.PhoneNumber,
            Address = createMemberDto.Address,
            MembershipDate = DateTime.UtcNow,
            MembershipType = createMemberDto.MembershipType,
            IsActive = true
        };

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        var createdMember = await _unitOfWork.Members.GetMemberWithBorrowsAsync(member.MemberId);
        return MapToDto(createdMember!);
    }

    public async Task<MemberDto?> UpdateMemberAsync(int id, UpdateMemberDto updateMemberDto)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null)
        {
            return null;
        }

        // Business logic: Check if email is being changed to one that already exists
        if (member.Email != updateMemberDto.Email)
        {
            var existingMember = await _unitOfWork.Members.GetMemberByEmailAsync(updateMemberDto.Email);
            if (existingMember != null)
            {
                throw new InvalidOperationException($"A member with email '{updateMemberDto.Email}' already exists.");
            }
        }

        member.FirstName = updateMemberDto.FirstName;
        member.LastName = updateMemberDto.LastName;
        member.Email = updateMemberDto.Email;
        member.PhoneNumber = updateMemberDto.PhoneNumber;
        member.Address = updateMemberDto.Address;
        member.MembershipType = updateMemberDto.MembershipType;
        member.IsActive = updateMemberDto.IsActive;

        _unitOfWork.Members.Update(member);
        await _unitOfWork.CompleteAsync();

        var updatedMember = await _unitOfWork.Members.GetMemberWithBorrowsAsync(member.MemberId);
        return MapToDto(updatedMember!);
    }

    public async Task<bool> DeleteMemberAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        if (member == null)
        {
            return false;
        }

        // Business logic: Check if member has active borrows
        var hasActiveBorrows = await _unitOfWork.Members.ExistsAsync(m => 
            m.MemberId == id && m.Borrows.Any(b => b.ReturnDate == null));
        
        if (hasActiveBorrows)
        {
            throw new InvalidOperationException("Cannot delete a member with active borrows. Please ensure all books are returned first.");
        }

        _unitOfWork.Members.Remove(member);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> MemberExistsAsync(int id)
    {
        return await _unitOfWork.Members.ExistsAsync(m => m.MemberId == id);
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            MemberId = member.MemberId,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            Address = member.Address,
            MembershipDate = member.MembershipDate,
            MembershipType = member.MembershipType,
            IsActive = member.IsActive,
            TotalBorrows = member.Borrows?.Count ?? 0
        };
    }
}

using Library.Core.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Service.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Library.Service.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MemberService> _logger;
    private readonly IMapper _mapper;

    public MemberService(IUnitOfWork unitOfWork, ILogger<MemberService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id);
        return member != null ? _mapper.Map<MemberDto>(member) : null;
    }

    public async Task<MemberDto?> GetMemberWithBorrowsAsync(int id)
    {
        var member = await _unitOfWork.Members.GetMemberWithBorrowsAsync(id);
        return member != null ? _mapper.Map<MemberDto>(member) : null;
    }

    public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByTypeAsync(string membershipType)
    {
        var members = await _unitOfWork.Members.GetMembersByTypeAsync(membershipType);
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<MemberDto> CreateMemberAsync(CreateMemberDto createMemberDto)
    {
        _logger.LogInformation("Creating new member: {Email}", createMemberDto.Email);
        
        // Business logic: Check if email already exists
        var existingMember = await _unitOfWork.Members.GetMemberByEmailAsync(createMemberDto.Email);
        if (existingMember != null)
        {
            _logger.LogWarning("Member creation failed: Email {Email} already exists", createMemberDto.Email);
            throw new InvalidOperationException($"A member with email '{createMemberDto.Email}' already exists.");
        }

        var member = _mapper.Map<Member>(createMemberDto);

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.CompleteAsync();

        var createdMember = await _unitOfWork.Members.GetMemberWithBorrowsAsync(member.MemberId);
        return _mapper.Map<MemberDto>(createdMember!);
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

        _mapper.Map(updateMemberDto, member);

        _unitOfWork.Members.Update(member);
        await _unitOfWork.CompleteAsync();

        var updatedMember = await _unitOfWork.Members.GetMemberWithBorrowsAsync(member.MemberId);
        return _mapper.Map<MemberDto>(updatedMember!);
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
}

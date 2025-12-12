using Microsoft.AspNetCore.Mvc;
using Library.Core.DTOs;
using Library.Service.Interfaces;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Get all members
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    /// <summary>
    /// Get member by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMember(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        
        if (member == null)
        {
            return NotFound(new { message = $"Member with ID {id} not found." });
        }

        return Ok(member);
    }

    /// <summary>
    /// Get member with borrow history
    /// </summary>
    [HttpGet("{id}/borrows")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberWithBorrows(int id)
    {
        var member = await _memberService.GetMemberWithBorrowsAsync(id);
        
        if (member == null)
        {
            return NotFound(new { message = $"Member with ID {id} not found." });
        }

        return Ok(member);
    }

    /// <summary>
    /// Get active members
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetActiveMembers()
    {
        var members = await _memberService.GetActiveMembersAsync();
        return Ok(members);
    }

    /// <summary>
    /// Get members by membership type
    /// </summary>
    [HttpGet("type/{membershipType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembersByType(string membershipType)
    {
        var members = await _memberService.GetMembersByTypeAsync(membershipType);
        return Ok(members);
    }

    /// <summary>
    /// Create a new member
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto createMemberDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var member = await _memberService.CreateMemberAsync(createMemberDto);
            return CreatedAtAction(nameof(GetMember), new { id = member.MemberId }, member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing member
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> UpdateMember(int id, [FromBody] UpdateMemberDto updateMemberDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var member = await _memberService.UpdateMemberAsync(id, updateMemberDto);
            
            if (member == null)
            {
                return NotFound(new { message = $"Member with ID {id} not found." });
            }

            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a member
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMember(int id)
    {
        try
        {
            var result = await _memberService.DeleteMemberAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Member with ID {id} not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

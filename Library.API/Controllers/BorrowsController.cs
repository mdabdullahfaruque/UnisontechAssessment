using Microsoft.AspNetCore.Mvc;
using Library.Core.DTOs;
using Library.Service.Interfaces;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowsController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowsController(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    /// <summary>
    /// Get all borrows
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BorrowDto>>> GetAllBorrows()
    {
        var borrows = await _borrowService.GetAllBorrowsAsync();
        return Ok(borrows);
    }

    /// <summary>
    /// Get borrow by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BorrowDto>> GetBorrow(int id)
    {
        var borrow = await _borrowService.GetBorrowByIdAsync(id);
        
        if (borrow == null)
        {
            return NotFound(new { message = $"Borrow record with ID {id} not found." });
        }

        return Ok(borrow);
    }

    /// <summary>
    /// Get overdue borrows
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BorrowDto>>> GetOverdueBorrows()
    {
        var borrows = await _borrowService.GetOverdueBorrowsAsync();
        return Ok(borrows);
    }

    /// <summary>
    /// Get active borrows
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BorrowDto>>> GetActiveBorrows()
    {
        var borrows = await _borrowService.GetActiveBorrowsAsync();
        return Ok(borrows);
    }

    /// <summary>
    /// Get borrows by member ID
    /// </summary>
    [HttpGet("member/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BorrowDto>>> GetBorrowsByMember(int memberId)
    {
        var borrows = await _borrowService.GetBorrowsByMemberIdAsync(memberId);
        return Ok(borrows);
    }

    /// <summary>
    /// Borrow a book
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BorrowDto>> BorrowBook([FromBody] CreateBorrowDto createBorrowDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var borrow = await _borrowService.BorrowBookAsync(createBorrowDto);
            return CreatedAtAction(nameof(GetBorrow), new { id = borrow.BorrowId }, borrow);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Return a borrowed book
    /// </summary>
    [HttpPut("{id}/return")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BorrowDto>> ReturnBook(int id, [FromBody] ReturnBookDto returnBookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var borrow = await _borrowService.ReturnBookAsync(id, returnBookDto);
            
            if (borrow == null)
            {
                return NotFound(new { message = $"Borrow record with ID {id} not found." });
            }

            return Ok(borrow);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Library.Core.DTOs;
using Library.Service.Interfaces;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibrariesController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    /// <summary>
    /// Get all libraries
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LibraryDto>>> GetAllLibraries()
    {
        var libraries = await _libraryService.GetAllLibrariesAsync();
        return Ok(libraries);
    }

    /// <summary>
    /// Get library by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryDto>> GetLibrary(int id)
    {
        var library = await _libraryService.GetLibraryByIdAsync(id);
        
        if (library == null)
        {
            return NotFound(new { message = $"Library with ID {id} not found." });
        }

        return Ok(library);
    }

    /// <summary>
    /// Get library with books
    /// </summary>
    [HttpGet("{id}/books")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryDto>> GetLibraryWithBooks(int id)
    {
        var library = await _libraryService.GetLibraryWithBooksAsync(id);
        
        if (library == null)
        {
            return NotFound(new { message = $"Library with ID {id} not found." });
        }

        return Ok(library);
    }

    /// <summary>
    /// Create a new library
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LibraryDto>> CreateLibrary([FromBody] CreateLibraryDto createLibraryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var library = await _libraryService.CreateLibraryAsync(createLibraryDto);
            return CreatedAtAction(nameof(GetLibrary), new { id = library.LibraryId }, library);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing library
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryDto>> UpdateLibrary(int id, [FromBody] UpdateLibraryDto updateLibraryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var library = await _libraryService.UpdateLibraryAsync(id, updateLibraryDto);
            
            if (library == null)
            {
                return NotFound(new { message = $"Library with ID {id} not found." });
            }

            return Ok(library);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a library
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibrary(int id)
    {
        try
        {
            var result = await _libraryService.DeleteLibraryAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Library with ID {id} not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

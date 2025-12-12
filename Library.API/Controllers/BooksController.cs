using Microsoft.AspNetCore.Mvc;
using Library.Core.DTOs;
using Library.Core.Models;
using Library.Service.Interfaces;
using Library.API.Models;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Get all books with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BookDto>>> GetAllBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;
        
        var result = await _bookService.GetAllBooksPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get book by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        
        if (book == null)
        {
            return NotFound(new { message = $"Book with ID {id} not found." });
        }

        return Ok(book);
    }

    /// <summary>
    /// Get available books
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAvailableBooks()
    {
        var books = await _bookService.GetAvailableBooksAsync();
        return Ok(books);
    }

    /// <summary>
    /// Get books by library ID
    /// </summary>
    [HttpGet("library/{libraryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByLibrary(int libraryId)
    {
        var books = await _bookService.GetBooksByLibraryIdAsync(libraryId);
        return Ok(books);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<BookDto>.ErrorResponse(
                "Validation failed", 
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
        }

        var book = await _bookService.CreateBookAsync(createBookDto);
        var response = ApiResponse<BookDto>.SuccessResponse(book, "Book created successfully");
        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, response);
    }

    /// <summary>
    /// Update an existing book
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var book = await _bookService.UpdateBookAsync(id, updateBookDto);
            
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var result = await _bookService.DeleteBookAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

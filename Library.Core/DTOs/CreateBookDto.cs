using System.ComponentModel.DataAnnotations;

namespace Library.Core.DTOs;

public class CreateBookDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN is required")]
    [MaxLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    [MaxLength(200, ErrorMessage = "Author cannot exceed 200 characters")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Publisher is required")]
    [MaxLength(200, ErrorMessage = "Publisher cannot exceed 200 characters")]
    public string Publisher { get; set; } = string.Empty;

    [Required(ErrorMessage = "Publication year is required")]
    [Range(1000, 9999, ErrorMessage = "Publication year must be a valid year")]
    public int PublicationYear { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    [MaxLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
    public string Genre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Total copies is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1")]
    public int TotalCopies { get; set; }

    [Required(ErrorMessage = "Available copies is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Available copies cannot be negative")]
    public int AvailableCopies { get; set; }

    [Required(ErrorMessage = "Library ID is required")]
    public int LibraryId { get; set; }
}

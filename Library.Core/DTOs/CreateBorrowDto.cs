using System.ComponentModel.DataAnnotations;

namespace Library.Core.DTOs;

public class CreateBorrowDto
{
    [Required(ErrorMessage = "Book ID is required")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Member ID is required")]
    public int MemberId { get; set; }

    [Range(1, 90, ErrorMessage = "Borrow duration must be between 1 and 90 days")]
    public int BorrowDurationDays { get; set; } = 14; // Default 2 weeks
}

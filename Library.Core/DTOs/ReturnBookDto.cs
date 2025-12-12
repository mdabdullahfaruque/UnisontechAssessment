using System.ComponentModel.DataAnnotations;

namespace Library.Core.DTOs;

public class ReturnBookDto
{
    [Required(ErrorMessage = "Return date is required")]
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
}

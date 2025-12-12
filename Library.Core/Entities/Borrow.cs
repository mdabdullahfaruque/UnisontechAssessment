namespace Library.Core.Entities;

public class Borrow
{
    public int BorrowId { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Fine { get; set; }

    // Navigation properties
    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
}

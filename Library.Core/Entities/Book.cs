namespace Library.Core.Entities;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int LibraryId { get; set; }

    // Navigation properties
    public Library Library { get; set; } = null!;
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}

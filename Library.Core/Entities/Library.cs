namespace Library.Core.Entities;

public class Library
{
    public int LibraryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }

    // Navigation property
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

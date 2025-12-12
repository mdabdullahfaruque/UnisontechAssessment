namespace Library.Core.DTOs;

public class MemberDto
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalBorrows { get; set; }
}

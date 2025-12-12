using Microsoft.EntityFrameworkCore;
using Library.Core.Entities;

namespace Library.Repository.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Core.Entities.Library> Libraries { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Borrow> Borrows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Library configuration
        modelBuilder.Entity<Core.Entities.Library>(entity =>
        {
            entity.HasKey(l => l.LibraryId);
            entity.Property(l => l.Name).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Address).IsRequired().HasMaxLength(500);
            entity.Property(l => l.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(l => l.Email).IsRequired().HasMaxLength(100);
        });

        // Book configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.BookId);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(300);
            entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Publisher).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Genre).IsRequired().HasMaxLength(100);

            entity.HasOne(b => b.Library)
                .WithMany(l => l.Books)
                .HasForeignKey(b => b.LibraryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Member configuration
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(m => m.MemberId);
            entity.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.LastName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
            entity.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(m => m.Address).IsRequired().HasMaxLength(500);
            entity.Property(m => m.MembershipType).IsRequired().HasMaxLength(50);
        });

        // Borrow configuration
        modelBuilder.Entity<Borrow>(entity =>
        {
            entity.HasKey(br => br.BorrowId);
            entity.Property(br => br.Status).IsRequired().HasMaxLength(50);
            entity.Property(br => br.Fine).HasColumnType("decimal(18,2)");

            entity.HasOne(br => br.Book)
                .WithMany(b => b.Borrows)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(br => br.Member)
                .WithMany(m => m.Borrows)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

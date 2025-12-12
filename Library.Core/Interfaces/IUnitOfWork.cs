namespace Library.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    ILibraryRepository Libraries { get; }
    IMemberRepository Members { get; }
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

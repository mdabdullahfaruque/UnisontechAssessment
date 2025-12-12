using Library.Core.Entities;

namespace Library.Core.Interfaces;

public interface ILibraryRepository : IRepository<Entities.Library>
{
    Task<Entities.Library?> GetLibraryWithBooksAsync(int id);
    Task<IEnumerable<Entities.Library>> GetLibrariesWithBooksAsync();
}

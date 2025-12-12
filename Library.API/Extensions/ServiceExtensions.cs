using Microsoft.EntityFrameworkCore;
using Library.Repository.Data;
using Library.Core.Interfaces;
using Library.Repository;
using Library.Service.Interfaces;
using Library.Service.Services;

namespace Library.API.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Register all library management services
    /// </summary>
    public static IServiceCollection AddLibraryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext(configuration);
        services.AddRepositories();
        services.AddApplicationServices();
        services.AddAutoMapperProfiles();

        return services;
    }

    /// <summary>
    /// Register database context
    /// </summary>
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    /// <summary>
    /// Register repositories and unit of work
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Register application services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IBorrowService, BorrowService>();

        return services;
    }

    /// <summary>
    /// Register AutoMapper profiles
    /// </summary>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}

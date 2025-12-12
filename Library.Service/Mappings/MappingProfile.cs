using AutoMapper;
using Library.Core.DTOs;
using Library.Core.Entities;

namespace Library.Service.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Book mappings
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.LibraryName, opt => opt.MapFrom(src => src.Library != null ? src.Library.Name : null));
        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>();

        // Member mappings
        CreateMap<Member, MemberDto>();
        CreateMap<CreateMemberDto, Member>()
            .ForMember(dest => dest.MembershipDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<UpdateMemberDto, Member>();

        // Borrow mappings
        CreateMap<Borrow, BorrowDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : null))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => 
                src.Member != null ? $"{src.Member.FirstName} {src.Member.LastName}" : null));
        CreateMap<CreateBorrowDto, Borrow>()
            .ForMember(dest => dest.BorrowDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(src.BorrowDurationDays)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
            .ForMember(dest => dest.Fine, opt => opt.MapFrom(src => 0));

        // Library mappings
        CreateMap<Library.Core.Entities.Library, LibraryDto>();
        CreateMap<CreateLibraryDto, Library.Core.Entities.Library>();
        CreateMap<UpdateLibraryDto, Library.Core.Entities.Library>();
    }
}

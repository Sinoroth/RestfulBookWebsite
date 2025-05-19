using AutoMapper;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;

namespace RESTfulBookWebsite.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookCreateDTO>().ReverseMap();

            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<Chapter, ChapterDTO>().ReverseMap();

            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
            CreateMap<Author, AuthorWithBooksDTO>().ReverseMap();
            CreateMap<Author, AuthorWithUserDTO>().ReverseMap();
        }
    }
}

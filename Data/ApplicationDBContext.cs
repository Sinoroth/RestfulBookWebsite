using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Migrations;
using RESTfulBookWebsite.Models;

namespace UdemiCourse.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Chapter> Chapters { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public List<Book> GetBooksByGenre(string genre)
        {
            List<Book> books = new List<Book>();

            Enum.TryParse(genre, out Genre actualGenre);

            foreach(var book in Books)
            {
                if(book.Genre == actualGenre)
                {
                    books.Add(book);
                }
            }
            return books;
        }

        public List<Book> GetBooksByAuthor(string authorName)
        {
            List<Book> books = new List<Book>();

            int authorID = 0;

            authorID = Users.FirstOrDefault(u => u.Name == authorName).Id;

            if (authorID == 0) return books;

            foreach (var book in Books)
            {
                if (book.AuthorID == authorID)
                {
                    books.Add(book);
                }
            }
            return books;
        }

        public List<Book> GetBooksByAuthorID(int authorID)
        {
            List<Book> books = new List<Book>();

            foreach (var book in Books)
            {
                if (book.AuthorID == authorID)
                {
                    books.Add(book);
                }
            }
            return books;
        }

        internal List<Review> GetReviewsByAuthorID(int id)
        {
            List<Review> reviews = new List<Review>();

            foreach(var review in Reviews)
            {
                if (review.UserId == id)
                {
                    reviews.Add(review);
                }
            }
            return reviews;
        }


        internal List<Review> GetReviewsByAuthor(string authorName)
        {
            List<Review> reviews = new List<Review>();

            foreach (var review in Reviews)
            {
                if (review.UserId == Users.FirstOrDefault(u=> u.Name == authorName).Id)
                {
                    reviews.Add(review);
                }
            }
            return reviews;
        }

        internal List<Chapter> GetChapterListByBook(string name)
        {
            List<Chapter> chapters = new List<Chapter>();   
            var book = Books.FirstOrDefaultAsync(u => u.Name == name).Result;
            /*foreach(var chapterID in book.ChapterIDs)
            {
                chapters.Add(Chapters.FirstOrDefaultAsync(u => u.Id == chapterID).Result);
            }*/
            return chapters;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    BookId = 1,
                    UserId = 2,
                    Comment = "Great book!",
                    Rating = 5
                },
                new Review
                {
                    Id = 2,
                    BookId = 2,
                    UserId = 1,
                    Comment = "Fantastic book about a boy trying to find  his own place in the world"
                }
                );

            modelBuilder.Entity<Chapter>().HasData(
                new Chapter
                {
                    Id = 1,
                    BookID = 1,
                    Title = "The first step",
                    Content = "Froddo finds the One Ring and meets Gandalf",
                    ReadCount = 1,
                    ChapterNumber = 1
                }, new Chapter
                {
                    Id = 2,
                    BookID = 1,
                    Title = "The Nazgul",
                    Content = "Froddo and Sam have to flee a Nazgul to escape the Shire",
                    ReadCount = 1,
                    ChapterNumber = 2
                }, new Chapter
                {
                    Id = 3,
                    BookID = 1,
                    Title = "The FellowShip of the Ring",
                    Content = "Froddo and Sam meet Aragon",
                    ReadCount = 1,
                    ChapterNumber = 3
                },
                new Chapter
                {
                    Id = 4,
                    BookID = 2,
                    Title = "The Boy who Lived",
                    Content = "Harry gets invited to the Hogwarts School of Wizardy and Magic by Hagrid",
                    ReadCount = 1,
                    ChapterNumber = 1
                }, new Chapter
                {
                    Id = 5,
                    BookID = 1,
                    Title = "The Azure Gem",
                    Content = "Artemis Fowl tricks an old fairy into handing him the secret book of the fairies and decodes it",
                    ReadCount = 1,
                    ChapterNumber = 1
                }, new Chapter
                {
                    Id = 6,
                    BookID = 1,
                    Title = "The Police Capture",
                    Content = "Artemis Fowl and Buttler kidnap Holly Short and demand a randsom from the LFPD for her release",
                    ReadCount = 1,
                    ChapterNumber = 2
                }
                );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "J.R.R Tolkien",
                    Username = "George",
                    Password = "testpass",
                    //BookFavoritedIDs = new List<int> { 2 },
                    //BookReadIDs = new List<int> { 2, 3 },
                    //BookWroteIDs = new List<int> { 1 },
                    //ReviewIDs = new List<int> { 1 }


                },
                new User
                {
                    Id = 2,
                    Name = "J.K. Rowling",
                    Username = "Margaret",
                    Password = "testpass2",
                    /*BookFavoritedIDs = new List<int> { 1, 3 },
                    BookReadIDs = new List<int> { 1, 3 },
                    BookWroteIDs = new List<int> { 2 },
                    ReviewIDs = new List<int> { 2 }*/


                }, new User
                {
                    Id = 3,
                    Name = "Eoin Colfer",
                    Username = "Colfer",
                    Password = "testpass3",
                    /*BookFavoritedIDs = new List<int> { 1 },
                    BookReadIDs = new List<int> { 1 },
                    BookWroteIDs = new List<int> { 3 },
                    ReviewIDs = new List<int> {}*/
                }
            );
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    AuthorID = 1,
                    Name = "Lord of the Rings",
                    Title = "Lord of the Rings",
                    Genre = Genre.Fantasy,
                    Description = "Epic fantasy adventure set in the land of Middle Earth starring Froddo Baggings",
                    //ChapterIDs = new List<int>() { 1, 2, 3 },
                    ReadCount = 0,
                    Rating = 5,

                },
                new Book
                {
                    Id = 2,
                    AuthorID = 2,
                    Name = "Harry Potter",
                    Title = "Harry Potter",
                    Genre = Genre.Action,
                    Description = "Adventures of Harry Potter in a world of Wizardy and Magic",
                    //ChapterIDs = new List<int>() { 4, },
                    ReadCount = 1,
                    Rating = 4,
                },
                new Book
                {
                    Id = 3,
                    AuthorID = 4,
                    Name = "Artemis Fowl",
                    Title = "Artemis Fowl",
                    Genre = Genre.Scifi,
                    Description = "Adventures of Artemis Fowl and Holly Short",
                    //ChapterIDs = new List<int>() { 5, 6 },
                    ReadCount = 2,
                    Rating = 4,
                }
                );
        }
    }
}

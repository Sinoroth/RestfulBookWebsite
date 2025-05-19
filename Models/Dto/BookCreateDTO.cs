using System.ComponentModel.DataAnnotations;

namespace RESTfulBookWebsite.Models.Dto
{
    public class BookCreateDTO
    {
        [Required]
        public int AuthorID { get; set; }
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ReadCount { get; set; }

        public string Genre { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RESTfulBookWebsite.Models.Dto
{
    public class ChapterDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BookID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public int ReadCount { get; set; }
        public int ChapterNumber { get; set; }

    }
}

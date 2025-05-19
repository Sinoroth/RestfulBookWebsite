using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RESTfulBookWebsite.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Author")]
        public int AuthorID { get; set; }
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ReadCount { get; set; }

        public Genre Genre { get; set; }

        [Range(1,5)]
        public int Rating {  get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}

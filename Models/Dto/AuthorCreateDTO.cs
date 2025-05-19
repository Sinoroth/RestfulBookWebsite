using System.ComponentModel.DataAnnotations.Schema;

namespace RESTfulBookWebsite.Models.Dto
{
    public class AuthorCreateDTO
    {
        [ForeignKey("User")]
        public int UserID { get; set; }
        public string Name { get; set; }
    }
}

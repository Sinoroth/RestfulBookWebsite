using System.ComponentModel.DataAnnotations;

namespace RESTfulBookWebsite.Models.Dto
{
    public class UserUpdateDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; }
    }
}

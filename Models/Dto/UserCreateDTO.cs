using System.ComponentModel.DataAnnotations;

namespace RESTfulBookWebsite.Models.Dto
{
    public class UserCreateDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; }
    }
}

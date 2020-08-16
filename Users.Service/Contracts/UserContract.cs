using System.ComponentModel.DataAnnotations;

namespace Users.Contracts
{
    public class UserContract
    {
        [Required]
        public string Username { get; set; }
    }
}
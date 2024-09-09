using System.ComponentModel.DataAnnotations;

namespace AseguradosAPI.Models
{
    public class Users
    {
        [Key]
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}

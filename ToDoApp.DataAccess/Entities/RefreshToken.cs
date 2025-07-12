using System.ComponentModel.DataAnnotations;

namespace ToDoApp.DataAccess.Entities
{
    public class RefreshToken
    {
        [Key]
        public  int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
        public bool IsRevoked { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

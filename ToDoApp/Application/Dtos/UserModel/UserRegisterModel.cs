using ToDoApp.Constant;

namespace ToDoApp.Application.Dtos.UserModel
{
    public class UserRegisterModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public Role Role { get; set; }
    }
}

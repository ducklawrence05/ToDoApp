using ToDoApp.Constant;

namespace ToDoApp.Application.Dtos.UserModel
{
    public class UserLoginModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}

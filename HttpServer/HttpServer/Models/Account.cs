using HttpServer.Attributes;

namespace HttpServer.Models
{
    public class Account
    {
        public int Id { get; set; }
        [DataField]
        public string Login { get; set; }
        [DataField]
        public string Password { get; set; }

        public Account(string login, string password, int id = -1)
        {
            Login = login;
            Password = password;
            Id = id;
        }

        //For ORM
        public Account() { }
    }
}

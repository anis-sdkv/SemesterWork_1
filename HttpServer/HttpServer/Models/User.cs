using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.Models;

[Entity("users")]
public class User
{
    [PrimaryKey("id", true)] public int Id { get; set; }
    [ColumnInfo("username", true, 30)] public string Username { get; set; }
    [ColumnInfo("login", true, 50)] public string Login { get; set; }
    [ColumnInfo("password", true, 20)] public string Password { get; set; }
    [ColumnInfo("registration_date", true)] public DateTime RegistrationDate { get; set; }

    [ColumnInfo("role", true, 1)]
    [Check("CHECK (* IN('u', 'a', 'm')) ")]
    public string Role { get; set; }

    [ColumnInfo("first_name", false, 50)] public string FirstName { get; set; }
    [ColumnInfo("last_name", false, 50)] public string LastName { get; set; }
    [ColumnInfo("patronymic", false, 50)] public string Patronymic { get; set; }
}
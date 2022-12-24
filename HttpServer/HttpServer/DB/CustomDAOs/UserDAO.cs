using HttpServer.DB.ORM;
using HttpServer.DB.ORM.EntityAttributes;
using HttpServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace HttpServer.DB.CustomDAOs;

public class UserDAO : IDAO<User, int>
{
    private static MyORM Orm => MyORM.Instance;

    private UserDAO()
    {
    }

    private static UserDAO? _instance;
    private static readonly object Locker = new();

    public static UserDAO Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                _instance ??= new UserDAO();
            }

            return _instance;
        }
    }

    public Task<List<User>> GetAll() => Orm.Select<User>();
    public async Task<User?> GetEntityByKey(int id) => (await Orm.Select<User>($"id = {id}")).FirstOrDefault();
    public Task<bool> Update(User user) => Orm.Update(user, $"id = {user.Id}");
    public Task<int> Delete(int id) => Orm.Delete<User>($"id = {id}");
    public Task<bool> Create(User user) => Orm.Insert(user);
    public async Task<User?> GetUserByLogin(string login) =>
        (await Orm.Select<User>($"login = '{login}'")).FirstOrDefault();

    public async Task<User?> GetUserByUsername(string username) =>
        (await Orm.Select<User>($"username = '{username}'")).FirstOrDefault();
}
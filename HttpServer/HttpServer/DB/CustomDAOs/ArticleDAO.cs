using HttpServer.DB.ORM;
using HttpServer.Models;

namespace HttpServer.DB.CustomDAOs;

public class ArticleDAO : IDAO<Article, int>
{
    private static MyORM Orm => MyORM.Instance;

    private ArticleDAO()
    {
    }

    private static ArticleDAO? _instance;
    private static readonly object Locker = new();

    public static ArticleDAO Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                _instance ??= new ArticleDAO();
            }

            return _instance;
        }
    }

    public Task<List<Article>> GetAll() => Orm.Select<Article>();
    public async Task<Article?> GetEntityByKey(int id) =>
        (await Orm.Select<Article>($"id = {id}")).FirstOrDefault();

    public Task<bool> Update(Article user)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(int key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(Article article) => Orm.Insert(article);
}
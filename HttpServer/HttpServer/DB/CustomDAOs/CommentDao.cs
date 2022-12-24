using HttpServer.DB.ORM;
using HttpServer.Models;

namespace HttpServer.DB.CustomDAOs;

public class CommentDao : IDAO<Comment, int>
{
    private static MyORM Orm => MyORM.Instance;

    private CommentDao()
    {
    }

    private static CommentDao? _instance;
    private static readonly object Locker = new();

    public static CommentDao Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                _instance ??= new CommentDao();
            }

            return _instance;
        }
    }

    public Task<List<Comment>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Comment?> GetEntityByKey(int key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Comment entity) => 
        Orm.Update(entity, $"id = {entity.Id}");

    public Task<int> Delete(int key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(Comment entity) => Orm.Insert(entity);

    public Task<List<Comment>> GetByArticle(int articleId) => 
        Orm.Select<Comment>($"publication_id = {articleId}");
}
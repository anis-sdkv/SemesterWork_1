using HttpServer.DB.ORM;
using HttpServer.Models;

namespace HttpServer.DB.CustomDAOs;

public class CategoryDao : IDAO<ArticleCategory, int>
{
    private static MyORM Orm => MyORM.Instance;

    private CategoryDao()
    {
    }

    private static CategoryDao? _instance;
    private static readonly object Locker = new();

    public static CategoryDao Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                _instance ??= new CategoryDao();
            }

            return _instance;
        }
    }

    public Task<List<ArticleCategory>> GetAll() =>
        Orm.Select<ArticleCategory>();

    public async Task<ArticleCategory?> GetEntityByKey(int key) =>
        (await Orm.Select<ArticleCategory>($"id = {key}")).FirstOrDefault();

    public Task<bool> Update(ArticleCategory entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(int key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(ArticleCategory entity) =>
        Orm.Insert(entity);

    public async Task<ArticleCategory?> GetByName(string name) =>
        (await Orm.Select<ArticleCategory>($"name = N'{name}'")).FirstOrDefault();

    public void InitCategories()
    {
        if (GetAll().Result.Count != 0)
            return;

        var categories = new List<ArticleCategory>()
        {
            new ArticleCategory() { Name = "Факты" },
            new ArticleCategory() { Name = "Личности" },
            new ArticleCategory() { Name = "События" }
        };
        foreach (var cat in categories)
            Create(cat).Wait();
    }
}
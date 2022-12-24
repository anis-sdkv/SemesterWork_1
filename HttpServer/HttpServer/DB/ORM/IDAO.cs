using System.Runtime.CompilerServices;
using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.DB.ORM;


internal interface IDAO<TEntity, in TKey>
{
    public Task<List<TEntity>> GetAll();
    public Task<TEntity?> GetEntityByKey(TKey key);
    public Task<bool> Update(TEntity entity);
    public Task<int> Delete(TKey key);
    public Task<bool> Create(TEntity entity);
}
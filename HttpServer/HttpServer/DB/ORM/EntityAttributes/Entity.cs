namespace HttpServer.DB.ORM.EntityAttributes;

public class Entity : Attribute
{
    public readonly string TableName;
    //Приоритет создания таблицы. Если таблица имеет внешние ключи, устанавлинвать > 1
    public readonly int Priority;
    public Entity(string tableName, int priority = 1)
    {
        TableName = tableName;
        Priority = priority;
    }
}
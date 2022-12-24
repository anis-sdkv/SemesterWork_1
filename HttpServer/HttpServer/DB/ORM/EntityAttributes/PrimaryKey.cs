namespace HttpServer.DB.ORM.EntityAttributes;

public class PrimaryKey : Attribute
{
    public readonly string Name;
    public readonly bool AutoGenerate;

    public PrimaryKey(string name, bool autoGenerate = true)
    {
        AutoGenerate = autoGenerate;
        Name = name;
    }
}
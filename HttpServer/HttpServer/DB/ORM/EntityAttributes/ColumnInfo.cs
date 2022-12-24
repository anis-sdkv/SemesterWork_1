namespace HttpServer.DB.ORM.EntityAttributes;

public class ColumnInfo : Attribute
{
    public readonly string Name;
    public readonly bool NotNull;
    public readonly int MaxLength;

    public ColumnInfo(string name, bool notNull = true, int maxLength = -1)
    {
        Name = name;
        NotNull = notNull;
        MaxLength = maxLength;
    }
}
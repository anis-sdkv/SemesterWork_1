namespace HttpServer.DB.ORM.EntityAttributes;

public class ForeignKey : Attribute
{
    public readonly string TableName;
    public readonly string FieldName;
    
    public ForeignKey(string tableName, string fieldName)
    {
        TableName = tableName;
        FieldName = fieldName;
    }
}
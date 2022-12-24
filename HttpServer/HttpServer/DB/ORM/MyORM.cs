using System.Data;
using System.Reflection;
using HttpServer.DB.CustomDAOs;
using HttpServer.DB.ORM.EntityAttributes;
using Microsoft.Data.SqlClient;

namespace HttpServer.DB.ORM;

public class MyORM
{
    private readonly string _connectionString;

    private static MyORM? _instance;

    public static MyORM Instance
    {
        get
        {
            if (_instance == null)
                throw new NullReferenceException();
            return _instance;
        }
    }

    public static void Init(string connectionString)
    {
        _instance = new MyORM(connectionString);
        _instance.SetupTables();
    }


    private MyORM(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> ExecuteNonQuery(string query, bool isStoredProc = false)
    {
        var noOfAffectedRows = 0;

        await using var connection = new SqlConnection(_connectionString);
        var command = connection.CreateCommand();
        if (isStoredProc)
            command.CommandType = CommandType.StoredProcedure;

        command.CommandText = query;
        connection.Open();
        noOfAffectedRows = command.ExecuteNonQuery();

        return noOfAffectedRows;
    }


    //TODO 
    public async Task<List<T>> ExecuteQuery<T>(string query, bool isStoredProc = false)
    {
        var result = new List<T>();
        var type = typeof(T);
        if (type.GetCustomAttribute(typeof(Entity)) == null)
            throw new CustomAttributeFormatException($"Entity attribute not defined for {type.Name}");

        await using var connection = new SqlConnection(_connectionString);
        var command = connection.CreateCommand();
        if (isStoredProc)
            command.CommandType = CommandType.StoredProcedure;

        command.CommandText = query;
        connection.Open();

        var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var instance = (T)Activator.CreateInstance(type)!;
            foreach (var p in type.GetProperties())
            {
                var r = p.GetCustomAttribute(typeof(Ignore));
                if (r != null)
                    return result;
                string name;
                if (p.GetCustomAttribute(typeof(PrimaryKey)) != null)
                    name = ((PrimaryKey)p.GetCustomAttribute(typeof(PrimaryKey))!).Name;
                else if (p.GetCustomAttribute(typeof(ColumnInfo)) != null)
                    name = ((ColumnInfo)p.GetCustomAttribute(typeof(ColumnInfo))!).Name;
                else
                    throw new CustomAttributeFormatException($"Attribute is not specified for the property {p.Name}");

                var value = reader[name];
                p.SetValue(instance, value is DBNull ? null : value);
            }

            result.Add(instance);
        }

        return result;
    }

    public async Task<T> ExecuteScalar<T>(string query, bool isStoredProc = false)
    {
        var result = default(T);

        await using var connection = new SqlConnection(_connectionString);
        var command = connection.CreateCommand();
        if (isStoredProc)
            command.CommandType = CommandType.StoredProcedure;

        command.CommandText = query;
        connection.Open();
        result = (T)command.ExecuteScalar();

        return result;
    }

    public Task<List<T>> Select<T>(string? selector = null)
    {
        var tableName = GetTableName(typeof(T));
        var command = $"SELECT * FROM [dbo].[{tableName}]";
        if (selector != null)
            command += $" WHERE {selector};";
        return ExecuteQuery<T>(command);
    }

    public async Task<bool> Insert<T>(T obj)
    {
        var tableName = GetTableName(typeof(T));
        var properties = typeof(T)
            .GetProperties()
            .Where(p => p.IsDefined(typeof(ColumnInfo)) && p.GetValue(obj) != null)
            .ToDictionary(p => ((ColumnInfo)p.GetCustomAttribute(typeof(ColumnInfo))!).Name,
                p => Format(p.GetValue(obj)));
        var command = $"INSERT INTO [dbo].[{tableName}] " +
                      $"({string.Join(',', properties.Keys)}) " +
                      $"VALUES ({string.Join(',', properties.Values)});";
        return await ExecuteNonQuery(command) > 0;
    }

    private string Format(object obj)
    {
        var result = "";
        if (obj is DateTime date)
        {
            var hour = date.Hour < 10 ? "0" + date.Hour : date.Hour.ToString();
            var minutes = date.Minute < 10 ? "0" + date.Minute : date.Minute.ToString();
            var sec = date.Second < 10 ? "0" + date.Second : date.Second.ToString();
            result = $"'{date.Year}-{date.Month}-{date.Day} {hour}:{minutes}:{sec}'";
        }
        else if (obj is string str)
            result = $"N'{str}'";
        else
            result = $"'{obj}'";

        return result;
    }

    public async Task<bool> Update<T>(T obj, string selector)
    {
        var tableName = GetTableName(typeof(T));
        var properties = typeof(T)
            .GetProperties()
            .Where(p => p.IsDefined(typeof(ColumnInfo)))
            .Select(p => $"{p.Name} = '{p.GetValue(obj)}'");
        var command = $"UPDATE [dbo].[{tableName}] " +
                      $"SET ({string.Join(',', properties)}) " +
                      $"WHERE {selector};";
        return await ExecuteNonQuery(command) == 1;
    }

    public async Task<int> Delete<T>(string selector)
    {
        var tableName = GetTableName(typeof(T));
        var command = $"DELETE FROM [dbo].[{tableName}]" +
                      $"WHERE {selector}";
        return await ExecuteNonQuery(command);
    }

    private string GetTableName(Type t)
    {
        var attribute = t.GetCustomAttribute(typeof(Entity));
        if (attribute == null)
            throw new Exception("the class is not marked with an Entity attribute");
        return ((Entity)attribute).TableName;
    }

    private void SetupTables()
    {
        var entities = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(Entity)))
            .OrderBy(t => ((Entity)t.GetCustomAttribute(typeof(Entity))!).Priority);
        foreach (var entity in entities)
        {
            var entityAttribute = (Entity)entity.GetCustomAttribute(typeof(Entity))!;
            if (!TableExist(entityAttribute.TableName).Result)
                CreateTable(entity, entityAttribute).Wait();
            CategoryDao.Instance.InitCategories();
        }
    }

    private async Task<bool> TableExist(string tableName)
    {
        var checkCmd = "SELECT COUNT(*) FROM [information_schema].tables " +
                       $"WHERE table_name = '{tableName}';";
        return await ExecuteScalar<int>(checkCmd) > 0;
    }

    private async Task CreateTable(Type entity, Entity entityAttribute)
    {
        var createCmd = $"CREATE TABLE {entityAttribute.TableName}(";
        try
        {
            var fields = entity.GetProperties().Select(GetPropertyAsSqlString)
                .Where(s => s != "");
            createCmd += string.Join(", ", fields);
            createCmd += ");";
            await ExecuteNonQuery(createCmd);
        }
        catch (Exception e)
        {
            ConsoleHandler.LogE(e);
            ConsoleHandler.LogM($"At {entityAttribute.TableName} table creation.");
            throw;
        }

        ConsoleHandler.LogM($"Table created: {entityAttribute.TableName}");
    }

    private static string GetPropertyAsSqlString(PropertyInfo property)
    {
        if (Attribute.IsDefined(property, typeof(PrimaryKey)))
        {
            var str =
                $"{((PrimaryKey)property.GetCustomAttribute(typeof(PrimaryKey))!).Name} INT PRIMARY KEY NOT NULL";
            var attr = (PrimaryKey)property.GetCustomAttribute(typeof(PrimaryKey))!;
            return attr.AutoGenerate ? str + " IDENTITY(1, 1)" : str;
        }

        if (Attribute.IsDefined(property, typeof(ColumnInfo)))
        {
            var columnInfo = (ColumnInfo)property.GetCustomAttribute(typeof(ColumnInfo))!;
            var str = $"{columnInfo.Name} {GetSqlType(property, columnInfo)}";
            if (columnInfo.NotNull) str += " NOT NULL";
            if (Attribute.IsDefined(property, typeof(Check)))
            {
                var check = (Check)property.GetCustomAttribute(typeof(Check))!;
                str += " " + check.Condition.Replace("*", columnInfo.Name);
            }

            if (Attribute.IsDefined(property, typeof(ForeignKey)))
            {
                var key = (ForeignKey)property.GetCustomAttribute(typeof(ForeignKey))!;
                str += $" REFERENCES {key.TableName}({key.FieldName})";
            }

            return str;
        }

        return "";
    }

    private static string GetSqlType(PropertyInfo property, ColumnInfo columnInfo)
    {
        var type = property.PropertyType.Name;
        if (columnInfo.MaxLength != -1 && type != nameof(String))
            throw new ArgumentException("The maxLength is only for VARCHAR");

        switch (type)
        {
            case nameof(String):
            {
                if (columnInfo.MaxLength == -1)
                    throw new ArgumentException("The maxLength for VARCHAR is not specified.");
                return columnInfo.MaxLength > 8000 ? "NVARCHAR(MAX)" : $"NVARCHAR({columnInfo.MaxLength})";
            }
            case nameof(Int32):
                return "INT";
            case nameof(Char):
                return "NCHAR(1)";
            case nameof(DateTime):
                return "DATETIME2";
            default:
                throw new NotImplementedException();
        }
    }
}
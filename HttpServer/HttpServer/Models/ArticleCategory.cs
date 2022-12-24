using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.Models;

[Entity("categories")]
public class ArticleCategory
{
    [PrimaryKey("id")] public int Id { get; set; }
    [ColumnInfo("name", true, 30)] public string Name { get; set; }
}
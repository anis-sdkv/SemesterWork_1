using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.Models;

[Entity("reports", 2)]
public class Report
{
    [PrimaryKey("id")] public int Id { get; set; }

    [ColumnInfo("user_id")]
    [ForeignKey("users", "id")]
    public int UserId { get; set; }

    [ColumnInfo("content", true, 500)] public string Content { get; set; }
}
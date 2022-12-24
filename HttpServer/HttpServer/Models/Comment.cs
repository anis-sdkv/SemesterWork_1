using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.Models;

[Entity("comments", 3)]
public class Comment
{
    [PrimaryKey("id")] public int Id { get; set; }

    [ColumnInfo("publication_id")]
    [ForeignKey("articles", "id")]
    public int PublicationId { get; set; }

    [ColumnInfo("user_id")]
    [ForeignKey("users", "id")]
    public int UserId { get; set; }

    [ColumnInfo("content", true, 500)] public string Content { get; set; }
    [ColumnInfo("creation_date")]
    public DateTime CreationDate { get; set; }
}
using HttpServer.DB.ORM.EntityAttributes;

namespace HttpServer.Models;

[Entity("articles", 2)]
public class Article
{
    [PrimaryKey("id")] 
    public int Id { get; set; }

    [ColumnInfo("author_id")]
    [ForeignKey("users", "id")]
    public int AuthorId { get; set; }

    [ColumnInfo("creation_time")] 
    public DateTime CreationTime { get; set; }

    [ColumnInfo("status", true, 1)]
    [Check("CHECK (* IN('p', 'm')) ")]
    public string Status { get; set; }

    [ColumnInfo("category_id")]
    [ForeignKey("categories", "id")]
    public int CategoryId { get; set; }

    [ColumnInfo("title", true, 60)] public string Title { get; set; }
    [ColumnInfo("img_path", true, 100)] public string ImagePath { get; set; }
    [ColumnInfo("content", true, 50000)] public string Content { get; set; }
}
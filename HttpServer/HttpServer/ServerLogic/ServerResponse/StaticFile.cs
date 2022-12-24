using System.Net;

namespace HttpServer.ServerLogic.ServerResponse;

public class StaticFile : Response
{
    private const string ResponseTag = "static";
    private readonly string _filePath;

    public StaticFile(string filePath)
    {
        this._filePath = filePath;
    }

    public override async Task SendResponse(HttpListenerContext context)
    {
        context.Response.ContentType = MimeMap.GetMimeType(Path.GetExtension(_filePath));
        await WriteFileAsync(context, _filePath);
        CloseResponse(context,ResponseTag);
    }
}
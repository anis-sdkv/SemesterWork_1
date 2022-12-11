using System.Net;
using System.Text;
using HttpServer.ServerLogic;

namespace HttpServer.ServerResponse;

public class StaticFile : IServerResponse
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
        var buffer = await R.GetBytesAsync(_filePath);
        await CloseResponse(context, buffer, ResponseTag);
    }
}
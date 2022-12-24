using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServer.ServerLogic.ServerResponse;

public class JsonResponse : Response
{
    private const string ResponseTag = "JSON";
    private readonly object _obj;

    public JsonResponse(object obj)
    {
        this._obj = obj;
    }

    public override async Task SendResponse(HttpListenerContext context)
    {
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(context.Response.OutputStream, _obj);
        CloseResponse(context, ResponseTag);
    }
}
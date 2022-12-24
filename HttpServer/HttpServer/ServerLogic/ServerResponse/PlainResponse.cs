using System.Net;
using System.Text;

namespace HttpServer.ServerLogic.ServerResponse;

public class PlainResponse : Response
{
    private const string ResponseTag = "Plain";
    private readonly int _code;
    private readonly string _message;

    public PlainResponse(string message, HttpStatusCode code = HttpStatusCode.OK)
    {
        _message = message;
        _code = (int)code;
    }

    public override async Task SendResponse(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain;charset=UTF-8";
        context.Response.StatusCode = _code;
        var bytes = Encoding.UTF8.GetBytes(_message);
        await WriteBuffer(context, bytes);
        CloseResponse(context, ResponseTag);
    }
}
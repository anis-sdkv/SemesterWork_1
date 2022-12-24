using System.Net;

namespace HttpServer.ServerLogic.ServerResponse;

public class StatusCodeResponse : Response
{
    private const string ResponseTag = "StatusCode";
    private readonly int _code;

    public StatusCodeResponse(HttpStatusCode code)
    {
        _code = (int)code;
    }
    public override Task SendResponse(HttpListenerContext context)
    {
        context.Response.StatusCode = _code;
        CloseResponse(context,ResponseTag);
        return Task.CompletedTask;
    }
}
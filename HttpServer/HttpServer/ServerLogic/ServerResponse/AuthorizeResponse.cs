using System.Net;
using System.Text;
using HttpServer.Sessions;

namespace HttpServer.ServerLogic.ServerResponse;

public class AuthorizeResponse : Response
{
    private const string ResponseTag = "Authorize";
    private readonly Session? _session;

    public AuthorizeResponse(Session? session)
    {
        _session = session;
    }

    public override Task SendResponse(HttpListenerContext context)
    {
        var cookie = new Cookie("SessionId", _session!.Guid.ToString(), "/");
        context.Response.Cookies.Add(cookie);
        var redirectUrl = $"{context.Request.Url.Scheme}://{context.Request.Url.Authority}/";
        context.Response.StatusCode = 303;
        context.Response.Redirect(redirectUrl);
        ConsoleHandler.LogM($"Redirected: {redirectUrl}");
        CloseResponse(context, ResponseTag);
        return Task.CompletedTask;
    }
}
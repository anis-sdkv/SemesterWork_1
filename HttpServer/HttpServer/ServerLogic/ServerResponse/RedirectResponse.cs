using System.Net;

namespace HttpServer.ServerLogic.ServerResponse;

public class RedirectResponse : Response
{
    private const string ResponseTag = "Redirect";
    private readonly string _localPath;
    public RedirectResponse(string localPath)
    {
        _localPath = localPath;
    }

    public override Task SendResponse(HttpListenerContext context)
    {
        var redirectUrl = $"{context.Request.Url.Scheme}://{context.Request.Url.Authority}/{_localPath}";
        context.Response.StatusCode = 303;
        context.Response.Redirect(redirectUrl);
        ConsoleHandler.LogM($"Redirected: {redirectUrl}");
        CloseResponse(context, ResponseTag);
        return Task.CompletedTask;
    }
}
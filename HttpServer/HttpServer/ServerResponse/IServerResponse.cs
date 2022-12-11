using System.Net;
using HttpServer.ServerLogic;

namespace HttpServer.ServerResponse;

public abstract class IServerResponse
{
    public abstract Task SendResponse(HttpListenerContext context);
    protected static async Task CloseResponse(HttpListenerContext context, byte[] buffer, string tag)
    {
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.Close();
        ConsoleHandler.LogM($"Запрос обработан: {tag} url: {context.Request.Url}");
    }
}
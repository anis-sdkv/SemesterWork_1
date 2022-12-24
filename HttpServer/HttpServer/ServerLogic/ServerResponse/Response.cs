using System.Net;

namespace HttpServer.ServerLogic.ServerResponse;

public abstract class Response
{
    public abstract Task SendResponse(HttpListenerContext context);

    protected static ValueTask WriteBuffer(HttpListenerContext context, byte[] buffer)
    {
        context.Response.ContentLength64 = buffer.Length;
        return context.Response.OutputStream.WriteAsync(buffer);
    }

    protected static async Task WriteFileAsync(HttpListenerContext context, string path)
    {
        await using var fs = File.OpenRead(path);
        context.Response.ContentLength64 = fs.Length;
        await fs.CopyToAsync(context.Response.OutputStream);
    }

    protected static void CloseResponse(HttpListenerContext context, string tag)
    {
        context.Response.Close();
        ConsoleHandler.LogM($"Запрос обработан: {tag} url: {context.Request.Url}");
    }
}
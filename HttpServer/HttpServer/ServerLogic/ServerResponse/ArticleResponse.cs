using System.Net;

namespace HttpServer.ServerLogic.ServerResponse;

public class ArticleResponse : Response
{
    public override Task SendResponse(HttpListenerContext context)
    {
        throw new NotImplementedException();
    }
}
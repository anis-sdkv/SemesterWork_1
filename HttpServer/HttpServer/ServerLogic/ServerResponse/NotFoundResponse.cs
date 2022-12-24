using System.Net;
using System.Text;
using Scriban;

namespace HttpServer.ServerLogic.ServerResponse;

public class NotFoundResponse : Response
{
    private const string ResponseTag = "not found";
    public override async Task SendResponse(HttpListenerContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        var page = await R.GetStringAsync(R.NotFound);
        var template = Template.Parse(page);
        var res = await template.RenderAsync(new { routing = R.RoutingBase });
        var buffer = Encoding.UTF8.GetBytes(res);
        await WriteBuffer(context, buffer);
        CloseResponse(context,ResponseTag);
    }
}
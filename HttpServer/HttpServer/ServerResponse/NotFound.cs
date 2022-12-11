using System.Net;
using System.Text;
using Scriban;

namespace HttpServer.ServerResponse;

public class NotFound : IServerResponse
{
    private const string ResponseTag = "not found";
    public override async Task SendResponse(HttpListenerContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        var page = await R.GetStringAsync(R.NotFound);
        var template = Template.Parse(page);
        var res = await template.RenderAsync(new { routing = R.RoutingBase });
        var buffer = Encoding.UTF8.GetBytes(res);
        CloseResponse(context, buffer, ResponseTag);
    }
}
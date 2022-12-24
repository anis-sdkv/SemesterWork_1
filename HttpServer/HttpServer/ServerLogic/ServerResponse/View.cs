using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;
using HttpServer.Sessions;
using Scriban;

namespace HttpServer.ServerLogic.ServerResponse;

public class View : Response
{
    private const string ResponseTag = "view";
    private readonly object _model;
    private readonly string _viewPath;

    public View(string viewPath, object model)
    {
        _viewPath = viewPath;
        _model = model;
    }

    public override async Task SendResponse(HttpListenerContext context)
    {
        var page = await R.GetStringAsync(_viewPath);
        var template = Template.Parse(page);
        var res = await template.RenderAsync(_model);
        var buffer = Encoding.UTF8.GetBytes(res);
        await WriteBuffer(context, buffer);
        CloseResponse(context, ResponseTag);
    }
}
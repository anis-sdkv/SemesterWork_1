using System.Diagnostics;
using HttpServer.Attributes;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using HttpMultipartParser;
using HttpServer.ServerLogic.ServerResponse;
using HttpServer.Sessions;
using HttpMethod = HttpServer.Attributes.HttpMethod;

namespace HttpServer.ServerLogic;

internal class RequestHandler
{
    private string? _dataDir;

    public string DataDirectory
    {
        get
        {
            if (_dataDir == null)
                throw new ArgumentException("Data directory is not set!");
            return _dataDir;
        }
        set => _dataDir = value;
    }

    public async Task Handle(HttpListenerContext context)
    {
        ConsoleHandler.LogM($"Запрос получен: {context.Request.Url}");
        var request = context.Request;

        var staticPath = DataDirectory + request.Url.LocalPath;
        try
        {
            if (!await TrySendStatic(context, staticPath) && !await TryMethodHandleAsync(context))
                await new NotFoundResponse().SendResponse(context);
        }
        catch (Exception ex)
        {
            ConsoleHandler.LogE(ex);
            await new StatusCodeResponse(HttpStatusCode.InternalServerError).SendResponse(context);
        }
    }

    private async Task<bool> TrySendStatic(HttpListenerContext context, string path)
    {
        if (!File.Exists(path))
            return false;
        await new StaticFile(path).SendResponse(context);
        return true;
    }

    private static async Task<bool> TryMethodHandleAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var uri = RoutingMap.MapUri(request.Url!);
        if (uri.Segments.Length != 3) return false;

        var controllerName = uri.Segments[1].Replace("/", "");
        var assembly = Assembly.GetExecutingAssembly();
        var controller = assembly
            .GetTypes()
            .FirstOrDefault(t => Attribute.IsDefined(t, typeof(HttpController)) && Regex.IsMatch(controllerName,
                (t.GetCustomAttribute(typeof(HttpController)) as HttpController)?.ControllerName!));
        if (controller == null) return false;

        var methodUri = uri.Segments[2].Replace("/", "");
        var method = controller
            .GetMethods()
            .FirstOrDefault(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                             && Regex.IsMatch(methodUri, ((HttpMethod)attr).MethodURI)));
        if (method == null) return false;

        var strParams = request.HttpMethod == "GET" ? ParseQuery(uri.Query) : await ParseBody(request);
        if (Attribute.IsDefined(method, typeof(RequireAuth)))
            strParams.Add(context.Request.Cookies["SessionId"]?.Value);
        if (method.GetParameters().Length != strParams.Count) return false;
        var queryParams = GetQueryParams(method, strParams);

        var result =
            (Response)
            await (method.Invoke(Activator.CreateInstance(controller), queryParams) as dynamic)!;

        result.SendResponse(context);
        return true;
    }

    private static object[] GetQueryParams(MethodInfo method, List<string> strParams)
    {
        return method
            .GetParameters()
            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            .ToArray();
    }

    private static List<string> ParseQuery(string query)
    {
        return query.Length == 0
            ? new List<string>()
            : query[1..]
                .Split('&')
                .Select(p => HttpUtility.UrlDecode(p.Split('=')[1]))
                .ToList();
    }

    private static string GetPostBody(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
            return "";
        using var stream = request.InputStream;
        using var sr = new StreamReader(stream);
        return sr.ReadToEnd();
    }

    private static async Task<List<string>> ParseBody(HttpListenerRequest request)
    {
        if (request.ContentType == "application/x-www-form-urlencoded")
        {
            return GetPostBody(request)
                .Split('&')
                .Select(p => HttpUtility.UrlDecode(p.Split('=')[1]))
                .ToList();
        }

        if (request.ContentType!.StartsWith("multipart/form-data;"))
        {
            var resultParameters = new List<string>();
            var parser = await MultipartFormDataParser.ParseAsync(request.InputStream);
            foreach (var file in parser.Files)
            {
                var filePath = "res/loadedImages/" + DateTime.Now.Ticks +
                               Path.GetExtension(file.FileName);
                resultParameters.Add(filePath);
                await using var sw = File.Create("debug/static/" + filePath);
                await file.Data.CopyToAsync(sw);
            }

            resultParameters.AddRange(parser.Parameters.Select(param => param.Data));
            return resultParameters.ToList();
        }

        throw new NotImplementedException();
    }
}
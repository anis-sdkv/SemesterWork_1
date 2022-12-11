using HttpServer.Attributes;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using HttpServer.ServerResponse;
using HttpMethod = HttpServer.Attributes.HttpMethod;

namespace HttpServer.ServerLogic
{
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

            var path = DataDirectory + request.Url.LocalPath;

            try
            {
                if (!TrySendStatic(context, path) && !await TryMethodHandleAsync(context))
                    await new NotFound().SendResponse(context);
            }
            catch (Exception ex)
            {
                ConsoleHandler.LogE(ex);
                // new ResponseBuilder(context).SetStatusCode((int)HttpStatusCode.InternalServerError)
                //     .SendAsync();
            }
        }

        private bool TrySendStatic(HttpListenerContext context, string path)
        {
            if (!File.Exists(path))
                return false;
            Task.Run(() => new StaticFile(path).SendResponse(context));
            return true;
        }

        private async Task<bool> TryMethodHandleAsync(HttpListenerContext context)
        {
            var request = context.Request;
            if (request.Url!.Segments.Length < 2) return false;

            var controllerName = request.Url.Segments[1].Replace("/", "");
            var assembly = Assembly.GetExecutingAssembly();
            var controller = assembly
                .GetTypes()
                .FirstOrDefault(t => Attribute.IsDefined(t, typeof(HttpController))
                                     && ((t.GetCustomAttribute(typeof(HttpController)) as HttpController)
                                         ?.ControllerName == controllerName
                                         || String.Equals(t.Name, controllerName,
                                             StringComparison.CurrentCultureIgnoreCase)));
            if (controller == null) return false;

            var methodUri = request.Url.Segments.Length > 2
                ? request.Url.Segments[2].Replace("/", "")
                : "";
            var method = controller
                .GetMethods()
                .FirstOrDefault(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                                 && Regex.IsMatch(methodUri, ((HttpMethod)attr).MethodURI)));
            if (method == null) return false;

            var strParams = request.Url.Segments
                .Skip(3)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            if (request.HttpMethod == "POST")
                strParams = ParseBody(GetPostBody(request));

            var queryParams = method
                .GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                .ToArray();

            if (queryParams.Length != strParams.Length)
                return false;

            var result =
                (IServerResponse)await (method.Invoke(Activator.CreateInstance(controller), queryParams) as dynamic)!;
            Task.Run(() => result.SendResponse(context));

            return true;
        }

        private static string GetPostBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
                return "";
            using var stream = request.InputStream;
            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        private static string[] ParseBody(string body)
        {
            return body
                .Split('&')
                .Select(p => p.Replace("+", "").Split('=')[1])
                .ToArray();
        }
    }
}
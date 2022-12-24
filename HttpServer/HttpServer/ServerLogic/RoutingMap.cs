namespace HttpServer.ServerLogic;

public static class RoutingMap
{
    private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
    {
        { "/", "/articles/main" },
        { "/article", "/articles/article" },
        { "/join", "/accounts/join" },
        { "/login", "/accounts/login" }
    };

    public static Uri MapUri(Uri requestUri)
    {
        var route = (requestUri.Segments[0] +
                     (requestUri.Segments.Length > 1 ? requestUri.Segments[1].Replace("/", "") : ""))
            .ToLower();
        return Map.ContainsKey(route)
            ? new Uri(
                requestUri.Scheme + "://" + requestUri.Authority + requestUri.LocalPath.Replace(route, Map[route]) +
                requestUri.Query)
            : requestUri;
    }
}
using System.Text;
using Scriban;

namespace HttpServer;

/// <summary>
/// Resource provider.
/// </summary>
public static class R
{
    public const string MainHeader = "debug/static/htmlElements/mainHeader.sbnhtml";
    public const string SecondaryHeader = "debug/static/htmlElements/secondaryHeader.sbnhtml";

    public const string Footer = "debug/static/htmlElements/footer.html";

    public const string ArticlePreview = "debug/static/htmlElements/articleCard.sbnhtml";
    public const string Comment = "debug/static/htmlElements/comment.sbnhtml";

    public const string MainPageTemplate = "debug/Templates/mainPage.sbnhtml";
    public const string LoginPageTemplate = "debug/Templates/login.sbnhtml";
    public const string RegisterPageTemplate = "debug/Templates/reg.sbnhtml";
    public const string ArticlePageTemplate = "debug/Templates/article.sbnhtml";
    public const string ProfilePageTemplate = "debug/Templates/profile.sbnhtml";
    public const string ArticleCreationPageTemplate = "debug/Templates/creation.sbnhtml";

    public const string NotFound = "debug/Templates/notFound.sbnhtml";

    private static string? _routingBase;

    public static string RoutingBase
    {
        get
        {
            if (_routingBase == null)
                throw new ArgumentException("Routing base is not set.");
            return _routingBase;
        }
    }

    public static void SetRoutingBase(string url)
    {
        _routingBase = $"<base href=\"{url}\" />";
    }

    public static async Task<string> GetStringAsync(string resource)
    {
        using var fs = File.OpenText(resource);
        return await fs.ReadToEndAsync();
    }

    public static async ValueTask<string> GetHeader(string resource, bool authenticated)
    {
        var header = await GetStringAsync(resource);
        var template = Template.Parse(header);
        return await template.RenderAsync(new { authenticated });
    }
}
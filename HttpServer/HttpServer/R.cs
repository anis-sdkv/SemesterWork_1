using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    /// <summary>
    /// Resource provider.
    /// </summary>
    public static class R
    {
        // public const string ConnectionString =
        //     "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BolshevikDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public const string Header = "debug/static/htmlElements/header.html";
        public const string Footer = "debug/static/htmlElements/footer.html";

        public const string ArticlePreview = "debug/static/htmlElements/articleCard.html";

        public const string MainPageTemplate = "debug/Templates/mainPage.sbnhtml";
        public const string LoginPageTemplate = "debug/Templates/login.sbnhtml";

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

        public static async Task<string> GetStringAsync(string path)
        {
            using var fs = File.OpenText(path);
            return await fs.ReadToEndAsync();
        }

        public static async Task<byte[]> GetBytesAsync(string path)
        {
            await using var fs = File.OpenRead(path);
            var buffer = new byte[fs.Length];
            await fs.ReadAsync(buffer);
            return buffer;
        }
    }
}
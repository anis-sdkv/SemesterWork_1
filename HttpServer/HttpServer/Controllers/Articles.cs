using HttpServer.Attributes;
using HttpServer.ServerLogic;
using Scriban;
using System.Net;
using System.Text;
using HttpServer.ServerResponse;

namespace HttpServer.Controllers
{
    [HttpController("test")]
    class Articles
    {
        [HttpGET("main")]
        public async Task<View> GetArticles()
        {
            var header = await R.GetStringAsync(R.Header);
            var footer = await R.GetStringAsync(R.Footer);
            var articleCard = await R.GetStringAsync(R.ArticlePreview);

            return new View(R.MainPageTemplate, new
            {
                routing = R.RoutingBase, header, footer, card = articleCard
            });
        }

        [HttpGET(MethodURI = "login")]
        public async Task<View> GetLogin()
        {
            var header = await R.GetStringAsync(R.Header);
            var footer = await R.GetStringAsync(R.Footer);

            return new View(R.LoginPageTemplate, new
            {
                routing = R.RoutingBase, header, footer
            });
        }
    }
}
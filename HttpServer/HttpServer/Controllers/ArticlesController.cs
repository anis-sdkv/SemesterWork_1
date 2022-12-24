using System.Net;
using HttpServer.Attributes;
using HttpServer.DB.CustomDAOs;
using HttpServer.Models;
using HttpServer.ServerLogic.ServerResponse;
using HttpServer.Sessions;
using Scriban;

namespace HttpServer.Controllers;

[HttpController("^articles$")]
class ArticlesController
{
    [RequireAuth]
    [HttpGET("^main$")]
    public async Task<View> GetMain(string? sessionGuid)
    {
        var footer = await R.GetStringAsync(R.Footer);
        var authenticated = sessionGuid != null && SessionManager.Instance.SessionExists(Guid.Parse(sessionGuid));
        var header = await R.GetHeader(R.MainHeader, authenticated);

        var articles = await ArticleDAO.Instance.GetAll();
        var cards = new List<string>();
        foreach (var article in articles)
        {
            cards.Add(await GetArticlePreview(article));
        }

        return new View(R.MainPageTemplate, new
        {
            routing = R.RoutingBase, header, cards, footer
        });
    }

    private static async Task<string> GetArticlePreview(Article article)
    {
        var model = new
        {
            article,
            category = await CategoryDao.Instance.GetEntityByKey(article.CategoryId),
            author = await UserDAO.Instance.GetEntityByKey(article.AuthorId)
        };
        var page = await R.GetStringAsync((R.ArticlePreview));
        var template = Template.Parse(page);
        return await template.RenderAsync(model);
    }

    [RequireAuth]
    [HttpGET("create")]
    public async Task<Response> GetCreatePage(string? sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session == null || (await UserDAO.Instance.GetEntityByKey(session.AccountId))!.Role != "a")
            return new RedirectResponse("login");
        var header = await R.GetHeader(R.SecondaryHeader, true);
        var footer = await R.GetStringAsync(R.Footer);
        return new View(R.ArticleCreationPageTemplate, new
        {
            routing = R.RoutingBase, header, footer
        });
    }

    [RequireAuth]
    [HttpPOST("^add$")]
    public async Task<Response> AddNewArticle(string filePath, string category, string title, string content,
        string? sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session == null || (await UserDAO.Instance.GetEntityByKey(session.AccountId))!.Role != "a")
            return new PlainResponse("Произошла ошибка авторизации, попробуйте еще раз.", HttpStatusCode.Forbidden);
        var article = new Article
        {
            AuthorId = session.AccountId, Status = "m", ImagePath = filePath, Title = title,
            CategoryId = (await CategoryDao.Instance.GetByName(category))!.Id, CreationTime = DateTime.Now,
            Content = content
        };
        await ArticleDAO.Instance.Create(article);
        return new PlainResponse("Статья успешно отправлена на модерацию.", HttpStatusCode.Accepted);
    }

    [RequireAuth]
    [HttpGET("^article$")]
    public async Task<Response> GetArticlePage(int articleId, string? sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        var article = await ArticleDAO.Instance.GetEntityByKey(articleId);
        if (article == null)
            return new NotFoundResponse();
        var author = await UserDAO.Instance.GetEntityByKey(article.AuthorId);
        var footer = await R.GetStringAsync(R.Footer);
        var header = await R.GetHeader(R.SecondaryHeader, session != null);
        var commentsList = await CommentDao.Instance.GetByArticle(articleId);
        var comments = new List<string>();
        foreach (var comment in commentsList)
        {
            comments.Add(await CommentsController.GetCommentView( comment.UserId == session?.AccountId, comment));
        }

        return new View(R.ArticlePageTemplate, new
        {
            routing = R.RoutingBase, header, article, author, comments, footer
        });
    }
}
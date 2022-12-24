using System.Net;
using Azure;
using HttpServer.Attributes;
using HttpServer.DB.CustomDAOs;
using HttpServer.Models;
using HttpServer.ServerLogic.ServerResponse;
using HttpServer.Sessions;
using Scriban;
using Response = HttpServer.ServerLogic.ServerResponse.Response;

namespace HttpServer.Controllers;

[HttpController("^comments")]
public class CommentsController
{
    [RequireAuth]
    [HttpGET("^add$")]
    public async Task<Response> AddComment(int publicationId, string commentContent, string? sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session == null)
            return new StatusCodeResponse(HttpStatusCode.Unauthorized);
        var comment = new Comment
        {
            PublicationId = publicationId, UserId = session.AccountId, Content = commentContent,
            CreationDate = DateTime.Now
        };
        var user = await UserDAO.Instance.GetEntityByKey(session.AccountId);
        await CommentDao.Instance.Create(comment);
        return new View(R.Comment, new
        {
            comment, user
        });
    }

    [RequireAuth]
    [HttpGET(MethodURI = "^update$")]
    public async Task<Response> UpdateComment(int publicationId, int commentId, string commentContent,
        string? sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session == null)
            return new StatusCodeResponse(HttpStatusCode.Unauthorized);
        var comment = new Comment
        {
            Id = commentId,
            PublicationId = publicationId, UserId = session.AccountId, Content = commentContent
        };
        var user = await UserDAO.Instance.GetEntityByKey(session.AccountId);
        await CommentDao.Instance.Update(comment);
        return new View(R.Comment, new
        {
            comment, user
        });
    }

    public static async Task<string> GetCommentView(bool owner, Comment comment)
    {
        var model = new
        {
            owner, comment,
            user = await UserDAO.Instance.GetEntityByKey(comment.UserId)
        };
        var page = await R.GetStringAsync((R.Comment));
        var template = Template.Parse(page);
        return await template.RenderAsync(model);
    }
}
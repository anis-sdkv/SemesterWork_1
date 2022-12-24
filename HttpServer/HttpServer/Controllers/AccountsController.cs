using System.Net;
using HttpServer.Attributes;
using HttpServer.DB.CustomDAOs;
using HttpServer.Models;
using HttpServer.ServerLogic.ServerResponse;
using HttpServer.Sessions;

namespace HttpServer.Controllers;

[HttpController("^accounts$")]
class AccountsController
{
    private static UserDAO Dao => UserDAO.Instance;

    [HttpGET(MethodURI = "^join$")]
    public async Task<View> GetJoinPage()
    {
        var header = await R.GetHeader(R.SecondaryHeader, false);
        var footer = await R.GetStringAsync(R.Footer);

        return new View(R.RegisterPageTemplate, new
        {
            routing = R.RoutingBase, header, footer
        });
    }

    [HttpGET(MethodURI = "^login$")]
    public async Task<View> GetLoginPage()
    {
        var header = await R.GetHeader(R.SecondaryHeader, false);
        var footer = await R.GetStringAsync(R.Footer);
        
        return new View(R.LoginPageTemplate, new
        {
            routing = R.RoutingBase, header, footer
        });
    }

    [HttpGET(MethodURI = "^authorize$")]
    public async Task<Response> Authorize(string login, string password)
    {
       var user = await UserDAO.Instance.GetUserByLogin(login);
       if (user == null)
           return new StatusCodeResponse(HttpStatusCode.NotFound);
       if (user.Password != password) return new StatusCodeResponse(HttpStatusCode.Forbidden);
       var session = SessionManager.Instance.CreateSession(user.Id, user.Login);
       return new AuthorizeResponse(session);
    }

    [RequireAuth]
    [HttpGET(MethodURI = "^profile$")]
    public async Task<Response> GetProfilePage(int profileId, string? sessionGuid)
    {
        var user = await UserDAO.Instance.GetEntityByKey(profileId);
        if (user == null) return new PlainResponse("Пользователь не найден.", HttpStatusCode.BadRequest);
        var authenticated = false;
        var owner = false;
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session != null)
        {
            authenticated = true;
            owner = session.AccountId == profileId;
        }

        var header = await R.GetHeader(R.SecondaryHeader, authenticated);
        var footer = await R.GetStringAsync(R.Footer);

        return new View(R.ProfilePageTemplate, new
        {
            routing = R.RoutingBase, header, footer, user, owner
        });
    }

    [RequireAuth]
    [HttpGET("^me$")]
    public async Task<Response> GetMyProfile(string sessionGuid)
    {
        var session = SessionManager.Instance.GetSession(sessionGuid);
        if (session == null)
            return new RedirectResponse("login");
        return await GetProfilePage(session.AccountId, sessionGuid);
    }

    [HttpGET("^check_username$")]
    public async Task<StatusCodeResponse> CheckUsername(string username)
    {
        var result = await Dao.GetUserByUsername(username) != null;
        return new StatusCodeResponse(result ? HttpStatusCode.Forbidden : HttpStatusCode.Accepted);
    }

    [HttpGET("^check_login$")]
    public async Task<StatusCodeResponse> CheckLogin(string login)
    {
        var result = await Dao.GetUserByLogin(login) != null;
        return new StatusCodeResponse(result ? HttpStatusCode.Forbidden : HttpStatusCode.Accepted);
    }

    [HttpPOST("^register$")]
    public Task<Response> RegisterNewUser(string username, string login, string password, string role)
    {
        var user = new User
        {
            Id = -1, Username = username, Login = login, Password = password, Role = role,
            RegistrationDate = DateTime.Now
        };
        return Register(user);
    }

    [HttpPOST("^register_author$")]
    public Task<Response> RegisterNewAuthor(string username, string login, string password,
        string firstName, string lastName, string patronymic, string role)
    {
        var user = new User
        {
            Id = -1, Username = username, Login = login, Password = password, Role = role,
            RegistrationDate = DateTime.Now, FirstName = firstName, LastName = lastName, Patronymic = patronymic
        };
        return Register(user);
    }

    private static async Task<Response> Register(User user)
    {
        var dao = UserDAO.Instance;
        if (await dao.GetUserByUsername(user.Username) != null || await dao.GetUserByLogin(user.Login) != null)
            return new PlainResponse(
                "Не удалось завершить регистрацию, пользователь с такой почтой или именем пользователя уже зарегистрирован.",
                HttpStatusCode.Conflict);

        await dao.Create(user);
        var session = SessionManager.Instance.CreateSession((await dao.GetUserByLogin(user.Login))!.Id, user.Login);
        return new AuthorizeResponse(session);
    }
}
// using HttpServer;
// using HttpServer.Attributes;
// using HttpServer.Models;
// using HttpServer.DB.DAO;
// using HttpServer.ServerLogic;
// using System.Net;
// using HttpServer.Sessions;
//
// namespace HttpServer.Controllers
// {
//     [HttpController("accounts")]
//     class Accounts
//     {
//         private readonly AccountDAO _accountDAO =
//             new AccountDAO(R.ConnectionString);
//
//         [HttpGET]
//         public ResponseBuilder GetAccounts(HttpListenerContext context)
//         {
//             var builder = new ResponseBuilder(context);
//
//             var sessionId = context.Request.Cookies["SessionId"];
//             if (TryAuthorize(sessionId, builder, out var info))
//                 return builder;
//
//             var accounts = _accountDAO.GetAll();
//             return builder.SetObject(accounts);
//         }
//
//         [HttpGET("\\d+")]
//         public ResponseBuilder GetAccountById(int id, HttpListenerContext context)
//         {
//             var account = _accountDAO.GetEntityByKey(id);
//             return new ResponseBuilder(context)
//                 .SetObject(account);
//         }
//
//         [HttpGET("info")]
//         public ResponseBuilder GetAccountInfo(HttpListenerContext context)
//         {
//             var builder = new ResponseBuilder(context);
//
//             var sessionId = context.Request.Cookies["SessionId"];
//             if (!TryAuthorize(sessionId, builder, out var session))
//                 return builder;
//             return GetAccountById(session.AccountId, context);
//         }
//
//         [HttpPOST("save")]
//         public ResponseBuilder SaveAccount(string login, string password, HttpListenerContext context)
//         {
//             var builder = new ResponseBuilder(context);
//             if (_accountDAO.GetEntityByLogin(login) != null)
//                 return builder.SetMessage("Аккаунт с таким логином уже зарегистрирован!");
//             _accountDAO.Create(new Account(login, password));
//             return builder.SetRedirect("https://steamcommunity.com/");
//         }
//
//         [HttpPOST("login")]
//         public ResponseBuilder Login(string login, string password, HttpListenerContext context)
//         {
//             var builder = new ResponseBuilder(context);
//             var account = _accountDAO.GetEntityByLogin(login);
//             if (account != null && account.Password == password)
//             {
//                 var manager = SessionManager.Instance;
//                 var session = manager.CreateSession(account.Id, account.Login);
//
//                 var cookie = new Cookie("SessionId", session.Guid.ToString());
//                 builder.SetCookie(cookie).SetMessage($"Вы авторизовались под логином {login}");
//                 return builder;
//             }
//             return builder.SetMessage("Пожалуйста, проверьте свой пароль и имя аккаунта и попробуйте снова.");
//         }
//
//         private bool TryAuthorize(Cookie? sessionId, ResponseBuilder builder, out Session session)
//         {
//             session = null;
//             if (sessionId != null)
//             {
//                 var manager = SessionManager.Instance;
//                 if (manager.TryGetSession(Guid.Parse(sessionId.Value), out session))
//                     return true;
//             }
//             builder.SetStatusCode((int)HttpStatusCode.Unauthorized);
//             return false;
//         }
//     }
// }

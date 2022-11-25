﻿using Azure;
using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM.DAO;
using HttpServer.ServerLogic;
using System.Net;

namespace HttpServer.Controllers
{
    [HttpController("accounts")]
    class Accounts
    {
        private AccountDAO _accountDAO =
            new AccountDAO("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");


        [HttpGET]
        public ResponseBuilder GetAccounts(HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            if (!AuthorizeInfo.Parse(context.Request.Cookies["SessionId"]?.Value).IsAuthorize)
                return builder.SetStatusCode((int)HttpStatusCode.Unauthorized);
            var accounts = _accountDAO.GetAll();
            return builder.SetObject(accounts);
        }

        [HttpGET("\\d+")]
        public ResponseBuilder GetAccountById(int id, HttpListenerContext context)
        {
            var account = _accountDAO.GetEntityByKey(id);
            return new ResponseBuilder(context.Response)
                .SetObject(account);
        }

        [HttpGET("info")]
        public ResponseBuilder GetAccountInfo(HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            var cookie = context.Request.Cookies["SessionId"];
            if (cookie == null)
                return builder.SetStatusCode((int)HttpStatusCode.Unauthorized);
            var info = AuthorizeInfo.Parse(cookie.Value);
            return GetAccountById(info.Id, context);
        }

        [HttpPOST("save")]
        public ResponseBuilder SaveAccount(string login, string password, HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            if (_accountDAO.GetEntityByLogin(login) != null)
                return builder.SetMessage("Аккаунт с таким логином уже зарегистрирован!");
            _accountDAO.Create(new Account(login, password));
            return builder.SetRedirect("https://steamcommunity.com/");
        }

        [HttpPOST("login")]
        public ResponseBuilder Login(string login, string password, HttpListenerContext context)
        {
            var builder = new ResponseBuilder(context.Response);
            var account = _accountDAO.GetEntityByLogin(login);
            if (account != null)
                if (account.Password == password)
                {
                    var id = account.Id;
                    var cookie = new Cookie("SessionId", $"IsAuthorize:true,id={id}");
                    builder.SetCookie(cookie).SetMessage($"Вы авторизовались под логином {login}");
                    return builder;
                }
            return builder.SetMessage("Пожалуйста, проверьте свой пароль и имя аккаунта и попробуйте снова.");
        }
    }
}
using HttpServer.ServerLogic;
using System.Net;
using HttpServer.DB;
using HttpServer.Sessions;

namespace HttpServer;

class Program
{
    static void Main(string[] args)
    {
        var server = new Server();
        ConsoleHandler.Run(server);
    }
}